using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    
    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 3f;
    
    [Header("FOV Parameters")]
    [SerializeField] private float viewRadius = 5f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask obstructionMask;
    [SerializeField] private float minimumDistanceToPlayer = 1f;

    [SerializeField] private AudioSource screamSoundEffect;
    
    private GameObject player;
    private Mover mover;
    private Transform target;
    private bool canSeePlayer;
    private bool sawPlayer;
    private bool isInfected;
    private Animator animator;

    public enum NPCState
    {
        Idle,
        RunningAway,
        Following
    }
    
    [SerializeField] private NPCState currentState;

    private void Awake()
    {
        mover = GetComponent<Mover>();
        animator = GetComponent<Animator>();
        sawPlayer = false;
        isInfected = false;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVroutine());
    }

    private void FixedUpdate()
    {
        HandleStates();
        HandleBehaviour();
    }

    private void HandleStates()
    {
        if (isInfected)
            currentState = NPCState.Following;
        else if ((sawPlayer || canSeePlayer) && !isInfected)
            currentState = NPCState.RunningAway;
        else
            currentState = NPCState.Idle;
    }
    
    private void HandleBehaviour()
    {
        if (currentState == NPCState.Idle)
            return;
        else if (currentState == NPCState.RunningAway)
        {
            RunAwayFromPlayer();
        }
        else if (currentState == NPCState.Following)
            FollowPlayer();
    }

    private IEnumerator FOVroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.3f);
        while (!sawPlayer && !isInfected)
        {
            yield return wait;
            FieldOfViewCheck();
        }
        
        yield return null;
    }

    private void FieldOfViewCheck()
    {
        float range = viewRadius;
        Collider2D[] rangeChecks = Physics2D.OverlapCircleAll(transform.position, range, playerMask);

        if (rangeChecks.Length != 0)
        {
            target = rangeChecks[0].transform;
            
            Vector2 directionToTarget = (target.position - transform.position).normalized;
            
            if (Vector2.Angle(transform.right, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector2.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                    sawPlayer = true;
                    if (!screamSoundEffect.isPlaying && !isInfected)
                    {
                        screamSoundEffect.Play();
                    }
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

    private void FollowPlayer()
    {
        if (Vector2.Distance(transform.position, player.transform.position) > minimumDistanceToPlayer)
        {
            mover.MoveTo(player.transform.position, runSpeed);
        }
        else if (Vector2.Distance(transform.position, player.transform.position) < minimumDistanceToPlayer - .5f)
        {
            RunAwayFromPlayer();
        }
        else
        {
            mover.Cancel();
        }
    }
    
    private void RunAwayFromPlayer()
    {
        Vector2 direction = (transform.position - player.transform.position).normalized;
        Vector3 runAwayPosition = transform.position + new Vector3(direction.x, direction.y, 1f) * 2f;
        mover.MoveTo(runAwayPosition, runSpeed);

    }
    
    public void Infect()
    {
        // GetComponent<SpriteRenderer>().color = Color.red;
        isInfected = true;
        transform.tag = "Infected";
        animator.SetTrigger("Infect");
        
        // Change object layer to Infected
        int infectLayer = LayerMask.NameToLayer("Infected");
        gameObject.layer = infectLayer;
        
        
    }

    public bool GetIsInfected()
    {
        return isInfected;
    }
    
    //FOV Gizmos
    private void OnDrawGizmosSelected()
    {
        if (isInfected) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
        
        Vector2 fovLine1 = Quaternion.AngleAxis(viewAngle / 2, transform.forward) * transform.right * viewRadius;
        Vector2 fovLine2 = Quaternion.AngleAxis(-viewAngle / 2, transform.forward) * transform.right * viewRadius;
        
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
