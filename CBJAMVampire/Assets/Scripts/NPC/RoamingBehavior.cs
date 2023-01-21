using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GetRoamPoint))]
[RequireComponent(typeof(Mover))]
public class RoamingBehavior : MonoBehaviour
{
    [SerializeField] private float radius = 5f;
    [SerializeField] private float roamSpeed = 5f;
    [SerializeField] private float roamWaitTime = 5f;
    private Mover mover;
    private bool lookingForRoamPoint = false;
    private GetRoamPoint getRoamPoint;

    private void Awake()
    {
        getRoamPoint = GetComponent<GetRoamPoint>();
        mover = GetComponent<Mover>();
    }

    void Update()
    {
        HandleRoam();
    }

    private void HandleRoam()
    {
        if (mover.HasPath() || lookingForRoamPoint) return;

        StartCoroutine(Roam());
    }

    private IEnumerator Roam()
    {
        lookingForRoamPoint = true;
        
        yield return new WaitForSeconds(roamWaitTime);
        
        if (mover.HasPath())
        {
            lookingForRoamPoint = false;
            yield break; // if we have a path, we don't need to find a new one
        }
        
        Vector3 destination = transform.position;
        
        while (Vector3.Distance(destination, transform.position) < .5f || destination == getRoamPoint.transform.position)
        {
            destination = getRoamPoint.GetRandomPoint(transform, radius);
        }
        
        
        mover.MoveTo(destination, roamSpeed);

        lookingForRoamPoint = false;
    }
}
