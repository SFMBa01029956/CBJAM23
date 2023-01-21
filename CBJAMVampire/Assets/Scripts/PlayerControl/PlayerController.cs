using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private InputActionReference movement, interact;
    
    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Interaction Parameters")]
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask obstructionMask;

    private bool canInteract;
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Transform target;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StartCoroutine(FOVroutine());
    }

    private void OnEnable()
    {
        interact.action.performed += Interact;
        //movement.action.performed += onMove;
    }

    void Update()
    {
        movementInput = movement.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (movementInput != Vector2.zero)
        {
            rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
    
    private void Interact(InputAction.CallbackContext obj)
    {
        if (!canInteract) return;
        if (target.CompareTag("NPC"))
        {
            target.GetComponent<NPCController>().Infect();
        }
    }

    private IEnumerator FOVroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        float range = interactRange;
        Collider2D[] rangeChecks = Physics2D.OverlapCircleAll(transform.position, interactRange, interactableLayer);
        
        if (rangeChecks.Length != 0)
        {
            target = rangeChecks[0].transform;
            Vector2 directionToTarget = (target.transform.position - transform.position).normalized;
            
            float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

            if (distanceToTarget <= range)
            {
                if (Physics2D.Raycast(transform.position, directionToTarget, obstructionMask))
                    canInteract = true;
                else
                    canInteract = false;
            }
        }
        else
        {
            canInteract = false;
        }
    }
    
    // FOV gizmo
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);

        if (canInteract)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, (target.transform.position - transform.position).normalized * interactRange);    
        }
    }
}
