using System.Collections;
using System.Collections.Generic;
using Traffic;
using UnityEngine;

public class car : MonoBehaviour
{
    private TrafficController controller;

    public GameObject destination;
    public bool driveRandomly = true;
    private List<RoadNode> _route = new();
    private RoadNode _previousNode;


    void Start()
    {
        controller = FindObjectOfType<TrafficController>();
        if (destination != null) StartCoroutine(FindRouteWithDelay());
    }
    
    private IEnumerator FindRouteWithDelay()
    {
        yield return new WaitForSeconds(1.5f); // Delay of 0.5 seconds
        _route = controller.findRoute(transform.position, destination.transform.position);
        Debug.Log("start route found with " + _route.Count + " nodes.");
    }

    private Road GetRandomRoad()
    {
        var roads = FindObjectsOfType<Road>();
        return roads[Random.Range(0, roads.Length)];
    }

    void Update()
    {
        //move towards the next node in the route
        if (_route.Count > 0)
        {
            var targetNode = _route[0];
            var target = targetNode.WorldPosition;

            if (Vector3.Distance(transform.position, target) < 3.0f)
            {
                if (_route.Count > 1)
                {
                    if (targetNode.IsIntersectionEntryPoint)
                    {
                        if (!targetNode.Intersection.IsOpen(targetNode, _route[1]))
                        {
                            return;
                        }
                    }
                }

                _route.RemoveAt(0);
                if (_route.Count == 0) return;
                var newTarget = _route[0];

                _previousNode = targetNode;
                if (_route.Count > 0) Debug.DrawLine(transform.position, newTarget.WorldPosition, Color.green, 1);
                
                if (newTarget.IsIntersectionExitPoint)
                {
                    targetNode.Intersection.LockIntersection(_previousNode, newTarget);
                    // controller.LockIntersection(_previousNode, newTarget);
                }
            }
            
            MoveCarForward(target);
        }
        else
        {
            if (!driveRandomly) return;
            //find new route
            Debug.Log("Route complete, finding new route");
            var road = GetRandomRoad();
            _route = controller.findRoute(transform.position, road.transform.position);
        }
    }

    private void MoveCarForward(Vector3 target)
    {
        var direction = (target - transform.position).normalized;
        var targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.5f);
        //move car forwards while turning
        var turnFactor = Quaternion.Angle(transform.rotation, targetRotation) / 110.0f; // Normalize turn angle
        var speed = Mathf.Lerp(10, 3, turnFactor); // Reduce speed based on turn angle
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }
}