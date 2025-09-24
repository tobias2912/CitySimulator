using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car : MonoBehaviour
{
    private TrafficController controller;

    //add destination position with a gizmo
    public GameObject destination;
    private List<RoadNode> _route;


    void Start()
    {
        controller = FindObjectOfType<TrafficController>();
        _route = controller.findRoute(transform.position, destination.transform.position);
        Debug.Log("Route found with " + _route.Count + " nodes.");
    }

    // Update is called once per frame
    void Update()
    {
        //move towards the next node in the route
        if (_route.Count > 0)
        {
            var target = _route[0].WorldPosition;
            //highlight target with a gizmo for 1 sec
            Debug.DrawLine(transform.position, target, Color.red, 2);
            var direction = (target - transform.position).normalized;
            var targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2);
            //move car forwards while turning
            transform.Translate(Vector3.forward * Time.deltaTime * 10);
            //if we are close to the target, remove it from the route
            if (Vector3.Distance(transform.position, target) < 1.0f)
            {
                Debug.Log("Reached node " + _route[0].Name());
                //remove the first node
                _route.RemoveAt(0);
            }
        }
    }
}