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
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void MoveTo(Vector3 destination, float speedFraction)
    {
        //transform.LookAt(new Vector2(destination.x, destination.y));
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
