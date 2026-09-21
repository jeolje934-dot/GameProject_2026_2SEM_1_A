using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("따라갈 대상")]

    [SerializeField] private Transform target;

    [Header("카메라 위치")]

    [SerializeField] private float distanace = 7f;

    [SerializeField] private float height = 1.5f;

    [Header("마우스 회전")]

    [SerializeField] private float mouseSensitiviity = 0.12f;

    [SerializeField] private float minVerticalAngle = -30f;

    [SerializeField] private float maxVertiaclAngle = 60f;

    [Header("마우스 스크롤")]

    [SerializeField] private float zoomSpeed = 1f;

    [SerializeField] private float minDistance = 2f;

    [SerializeField] private float maxDistance = 10f;

    private float yaw;

    private float pitch = 15;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        Mouse mouse = Mouse.current;
        
        if(mouse == null)
        {
            return;
        }
        //마우스 회전
        Vector2 mouseDelta = mouse.delta.ReadValue();

        yaw += mouseDelta.x * mouseSensitiviity;
        pitch -= mouseDelta.y * mouseSensitiviity;
        pitch = Mathf.Clamp(distanace, minDistance, maxDistance);

    }

    private void LateUpdate()
    {
        if(target == null)
        {
            return;
        }
        //카메라가 바라볼 위치
        Vector3 lookInput = target.position + Vector3.up * height;

        //Yaw가 pitch를 실제 회전 값으로 변환
        Quaternion oribitRotation = Quaternion.Euler(pitch, yaw, 0f);

        //회전 방향을 기준으로 Player 뒤쪽 위치 먼저 계산 
        Vector3 camera0fSet = oribitRotation * new Vector3(0f, 0f, -distanace);

        //Player 주변의 계산된 위치로 이동 
        transform.position = lookInput + camera0fSet;

        //Player 중심을 바라본다
        transform.LookAt(lookInput);

    }
}


