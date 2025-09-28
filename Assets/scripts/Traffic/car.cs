using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car : MonoBehaviour
{
    private TrafficController controller;

    public GameObject destination;
    public bool driveRandomly = true;
    private List<RoadNode> _route = new();


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
            var nextNode = _route[0];
            var target = nextNode.WorldPosition;

            // if (nextNode.IsLocked())
            // {
            //     Debug.Log("Node is locked, waiting...");
            //     //wait 1 second
            //     return;
            // }
            //if intersection, lock next node

            if (Vector3.Distance(transform.position, target) < 3.0f)
            {
                Debug.Log("Reached node " + nextNode.Name());
                //if next node is intersection entry point, check if node is locked
                if (nextNode.IsIntersectionEntryPoint && nextNode.IsLocked())
                {
                    Debug.Log("Intersection entry point is locked, waiting...");
                    return;
                }

                _route.RemoveAt(0);
                //draw line to next node
                if (_route.Count > 0) Debug.DrawLine(transform.position, nextNode.WorldPosition, Color.green, 1);
                if (nextNode.IsIntersectionExitPoint)
                {
                    nextNode.Lock(3.0f); // Lock for 3 seconds
                    Debug.Log("Locked intersection node for 3 seconds");
                }
            }

            var direction = (target - transform.position).normalized;
            var targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.5f);
            //move car forwards while turning
            var turnFactor = Quaternion.Angle(transform.rotation, targetRotation) / 110.0f; // Normalize turn angle
            var speed = Mathf.Lerp(10, 3, turnFactor); // Reduce speed based on turn angle
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
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
}