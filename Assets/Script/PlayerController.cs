using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private Animator animator;                //애니매이터

    [SerializeField] private Transform cameraTransform;        //캐릭터를 따라갈 카메라

    [Header("이동 설정")]

    [SerializeField] private float walkSpeed = 3f;             // 걷기 속도

    [SerializeField] private float runSpeed = 6f;              //뛰기 속도

    [SerializeField] private float rotationSpeed = 10f;        // 회전 속도

    [Header("바닥 설정")]

    [SerializeField] private float gravity = -20f;              //중력

    private CharacterController controller;                     // 유니티의 캐릭터 컨트롤러 접근


    private float verticalVeolocity;                            //수평이동의 속도값 정의



    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        //1. WASD 입력
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }
        Vector2 input = Vector2.zero;
        if (keyboard.aKey.isPressed)
            input.x -= 1f;
        if (keyboard.dKey.isPressed)
            input.x += 1f;
        if (keyboard.sKey.isPressed)
            input.y -= 1f;
        if (keyboard.wKey.isPressed)
            input.y += 1f;

        input = Vector2.ClampMagnitude(input, 1f);

        //2. 카메라 앞쪽과 오른쪽 방향 
        Vector3 camerForward = cameraTransform.forward;
        Vector3 camerRight = cameraTransform.right;

        //카메라 위 아래 기울기는 이동에 사용하지 않는다
        camerForward.y = 0f;
        camerRight.y = 0f;

        camerForward.Normalize();
        camerRight.Normalize();

        //3. 카메라 기준 이동 방향 

        Vector3 moveDirection = camerForward * input.y + camerRight * input.x;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        //4. Shift 달리기
        bool isRunning = keyboard.leftShiftKey.isPressed;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        //5. 수평 이동
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);

        //6. 이동방향으로 회전
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        //7. 기본 중력 설정
        if (controller.isGrounded && verticalVeolocity < 0f)
        {
            verticalVeolocity = -2f;
        }
        else
        {
            verticalVeolocity += gravity * Time.deltaTime;
        }
        controller.Move(Vector3.up * verticalVeolocity * Time.deltaTime);   // 컨트롤러에 이동 방향과 속도를 준다.

        //8. Idle, Walk, Run 애니메이션 

        float animationSpeed = 0f;

        if (moveDirection.sqrMagnitude > 0.001f)
        {
            animationSpeed = isRunning ? 1f : 0.5f;


        }
        animator.SetFloat("speed", animationSpeed, 0.1f, Time.deltaTime);
    }
}


