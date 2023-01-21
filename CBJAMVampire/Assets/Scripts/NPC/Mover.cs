using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 3f;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        // agent.updateRotation = false;
        // agent.updateUpAxis = false;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
    }

    public void MoveTo(Vector3 destination, float speedFraction = 2f)
    {
        transform.LookAt(destination);

        agent.SetDestination(destination);
        agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
        agent.isStopped = false;
    }
    
    public void Cancel()
    {
        agent.isStopped = true;
    }
    
    public bool HasPath()
    {
        return agent.hasPath;
    }
}
