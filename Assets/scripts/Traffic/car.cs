using System.Collections;
using System.Collections.Generic;
using Traffic;
using Unity.VisualScripting;
using UnityEngine;

public class car : MonoBehaviour
{
    private TrafficController controller;

    public GameObject destination;
    public bool driveRandomly = false;
    private List<RoadNode> _route = new();
    private RoadNode _previousNode;
    private float _maxDistance = 6.0f;

    void Start()
    {
        controller = FindObjectOfType<TrafficController>();
        if (destination != null) StartCoroutine(FindRouteWithDelay());
        //multiply max distance by length of collider
        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            _maxDistance *= collider.bounds.size.z/1f;
            // maxDistance -= 15;
        }
    }

    public void setRoute(List<RoadNode> route)
    {
        _route = route;
    }

    private IEnumerator FindRouteWithDelay()
    {
        yield return new WaitForSeconds(1.5f); // Delay of 0.5 seconds
        _route = controller.findCarRoute(transform.position, destination.transform.position);
        Debug.Log("start route found with " + _route.Count + " nodes.");
    }

    private Road GetRandomRoad()
    {
        var roads = FindObjectsOfType<Road>();
        return roads[Random.Range(0, roads.Length)];
    }

    void Update()
    {
        if (CollisionCheck()) return;

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
                            MoveCarForward(target, true);
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
                }
            }

            MoveCarForward(target);
        }
        else
        {
            if (!driveRandomly) return;
            // Find new route
            Debug.Log("Route complete, finding new route");
            var road = GetRandomRoad();
            _route = controller.findCarRoute(transform.position, road.transform.position);
        }
    }

    private RaycastHit[] _hits = new RaycastHit[3]; // Pre-allocated array

    private bool CollisionCheck()
    {
        //do one raycast slightly to the left, one to the right and one in the middle
        var leftOffset = transform.position - transform.right * 0.8f;
        var rightOffset = transform.position + transform.right * 0.8f;
        Debug.DrawRay(leftOffset, transform.forward * _maxDistance, Color.blue);
        Debug.DrawRay(rightOffset, transform.forward * _maxDistance, Color.blue);
        var size = Physics.RaycastNonAlloc(leftOffset, transform.forward, _hits, _maxDistance, LayerMask.GetMask("Vehicles"));
        for (var i = 0; i < size; i++)
        {
            var hit = _hits[i];
            if (hit.collider.gameObject == gameObject) continue;
            MoveCarForward(Vector3.zero, true);
            return true;
        }

        size = Physics.RaycastNonAlloc(rightOffset, transform.forward, _hits, _maxDistance, LayerMask.GetMask("Vehicles"));
        for (var i = 0; i < size; i++)
        {
            var hit = _hits[i];
            if (hit.collider.gameObject == gameObject) continue;
            MoveCarForward(Vector3.zero, true);
            return true;
        }
        return false;
    }

    private float _currentSpeed;
    private float _targetSpeed = 10f;
    private float _accelerationRate = 1.1f; // Controls how quickly the car accelerates

    private void MoveCarForward(Vector3 target, bool isBreaking = false)
    {
        if (isBreaking)
        {
            _targetSpeed = 0f;
            _accelerationRate = 8.0f; // Increase deceleration rate when braking
        }
        else
        {
            var direction = (target - transform.position).normalized;
            var targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);
            var turnFactor = Quaternion.Angle(transform.rotation, targetRotation) / 110.0f;
            _targetSpeed = Mathf.Lerp(10, 3, turnFactor);
            _accelerationRate = 1.5f; // Normal acceleration rate
        }

        // Adjust target speed based on turn angle
        _currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, Time.deltaTime * _accelerationRate);

        // Move the car forward
        transform.Translate(Vector3.forward * Time.deltaTime * _currentSpeed);
    }
}