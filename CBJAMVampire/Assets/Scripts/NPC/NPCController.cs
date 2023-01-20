using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    
    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float runSpeed = 2f;
    
    [Header("FOV Parameters")]
    [SerializeField] private float viewRadius = 5f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask obstructionMask;
    
    private GameObject player;
    private Transform target;
    private bool canSeePlayer;

    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVroutine());
    }

    private IEnumerator FOVroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        float range = viewRadius;
        Collider2D[] rangeChecks = Physics2D.OverlapCircleAll(transform.position, range, playerMask);

        if (rangeChecks.Length != 0)
        {
            target = rangeChecks[0].transform;
            Vector2 directionToTarget = (target.position - transform.position).normalized;
            
            if (Vector2.Angle(transform.up, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector2.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    Debug.Log("Player in FOV");
                    canSeePlayer = true;
                }
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else
            canSeePlayer = false;
    }
    
    //FOV Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
        
        Vector2 fovLine1 = Quaternion.AngleAxis(viewAngle / 2, transform.forward) * transform.up * viewRadius;
        Vector2 fovLine2 = Quaternion.AngleAxis(-viewAngle / 2, transform.forward) * transform.up * viewRadius;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);
        
        
        if (canSeePlayer)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, (target.transform.position - transform.position).normalized * viewRadius);    
        }
    }
}
