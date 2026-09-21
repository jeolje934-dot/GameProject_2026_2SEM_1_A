using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerPickup : MonoBehaviour
{
    [Header("직접 연결")]

    [SerializeField] private Animator animator;
    
    [SerializeField] private PlayerController playerController;

    [Header("아이템 탐색")]
    
    [SerializeField] private float detectionRadius = 2f;

    [Header("줍기 모션")]
    
    [SerializeField] private float pickupDuration = 1.2f;
    
    [SerializeField] private float pickupMoment = 0.55f;

    private PickupItem targetItem;

    private bool isPickingUP;



    private PickupItem FindClosestltem()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide); //구체의 충돌체를 만든다.
        PickupItem closestltem = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider itemCollider in colliders)
        {
            PickupItem item = itemCollider.GetComponentInParent<PickupItem>();

            if (item == null) continue;

            float distance = Vector3.Distance(transform.position, item.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestltem = item;
            }
            
        }
        return closestltem;
    }
    
    
    
    
    private IEnumerator PickupAnimation()
    {
        isPickingUP = true;

        //현재  찾은 아이템을 저장한다
        PickupItem itemToCollect = targetItem;

        //아이템 방향으로 캐릭터를 돌린다 
        Vector3 itemDirection = itemToCollect.transform.position;
        itemDirection.y = 0;

        if (itemDirection.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(itemDirection);
        }

        //줍기 상태로 변경하여 이동을 막는다.
        playerController.ChangeState(PlayerState.Pickup);
        //전환선 없이 Pickup01 상태로 이동한다.
        animator.CrossFade("Pickup01", 0.1f);
        //손이 아이템에 닿는 시점까지 기다린다.
        yield return new WaitForSeconds(pickupMoment);








        //아이템을 실제로 획득한다.
        if (itemToCollect != null)
        {
            itemToCollect.Collect();
        }

        //남은 애니메이션 시간을 기다립니다.
        yield return new WaitForSeconds(pickupDuration - pickupMoment);

        //이동 Blend Tree로 돌아간다.
        animator.CrossFade("Blend Tree", 0.1f);

        //Normal상태로 돌아가 이동을 허용 한다.
        playerController.ChangeState(PlayerState.Normal);

        targetItem = null;
        isPickingUP = false;

    }
    void Start()
    { 
        
    }

    
    void Update()
    {
        if (isPickingUP) return;

        //Player 주변에서 가장 가까운 아이템을 찾는다.
        targetItem = FindClosestltem();

        if (targetItem == null) return;

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null) return;

        //가까운 아이템이 있을 때 E키로 줍는다.
        if (keyboard.eKey.wasPressedThisFrame)
        {
            StartCoroutine(PickupAnimation());
        }
    }
}
