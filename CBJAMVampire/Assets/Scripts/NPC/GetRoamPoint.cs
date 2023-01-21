using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GetRoamPoint : MonoBehaviour
{
    [SerializeField] private float range = 5f;

    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }


    public Vector3 GetRandomPoint(Transform point = null, float radius = 0)
    {
        if (RandomPoint(point == null ? transform.position : point.transform.position,
                radius == 0 ? this.range : radius, out var _point))
        {
            Debug.DrawRay(_point, Vector3.up, Color.red, 1f);
            print("Returning new position");
            return _point;
        }

        if (point == null)
        {
            return transform.position;
        }
                
        return point.position;
    }
}
