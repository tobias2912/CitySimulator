using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

internal class NpcMovement
{
    private readonly Npc _npc;
    private readonly TrafficController _trafficController;
    
    private Vector3? _finalDestination;
    private readonly NavMeshAgent _navMeshAgent;
    private MovementState _movementState = MovementState.Idle;
    private float _triggerDistance;
    private List<RoadNode> _selectedCarRoute;

    public NpcMovement(Npc npc, TrafficController trafficController)
    {
        _npc = npc;
        _trafficController = trafficController;
        _navMeshAgent = _npc.GetComponent<NavMeshAgent>();
    }

    public void Update()
    {
        _triggerDistance = 0.5f;
        switch (_movementState)
        {
            case MovementState.Driving:
                //check if close to last route node
                if (_selectedCarRoute is { Count: > 0 } &&
                    Vector3.Distance(_npc.transform.position, _selectedCarRoute.Last().WorldPosition) < 5.0f)
                {
                    //car reached destination
                    _movementState = MovementState.Walking;
                    _navMeshAgent.SetDestination((Vector3)_finalDestination);
                    //despawn car
                    var car = _npc.GetComponentInParent<car>();
                    _npc.transform.SetParent(null);
                    if (car != null)
                    {
                        Object.Destroy(car.gameObject, 2.0f);
                    }
                }
                return;
            case MovementState.WalkingToCar when _navMeshAgent.remainingDistance < _triggerDistance:
            {
                //reached car, start driving
                _movementState = MovementState.Driving;
                _navMeshAgent.ResetPath();
                SpawnCar();

                break;
            }
            case MovementState.Idle:
                break;
            case MovementState.Walking:
                break;
        }
    }

    private void SpawnCar()
    {
        var carPrefab = _trafficController.carPrefabs[UnityEngine.Random.Range(0, _trafficController.carPrefabs.Count)];
        var carObject = Object.Instantiate(carPrefab, _npc.transform.position, Quaternion.identity);
        var car = carObject.GetComponent<car>();
        car.driveRandomly = false;
        car.setRoute(_selectedCarRoute);
        car.destination = new GameObject("CarDestination");
        car.destination.transform.position = (Vector3)_finalDestination;
        //set npc as child of car
        _npc.transform.SetParent(car.transform);
        //move npc to passenger seat
        // var passengerSeat = carObject.transform.Find("PassengerSeat");
    }

    public void SetDestination(Vector3 destination)
    {
        _finalDestination = destination;
        // use traffic controller to find route
        var route = _trafficController.findCarRoute(_npc.transform.position, destination);
        if (route.Count > 5)
        {
            //start Car pickup
            _selectedCarRoute = route;
            _navMeshAgent.SetDestination(route.First().WorldPosition);
            _movementState = MovementState.WalkingToCar;
        }
        else
        {
            //move towards target using navigation mesh agent
            _navMeshAgent.SetDestination(destination);
            _movementState = MovementState.Walking;
        }
    }

    public bool IsAtDestination()
    {
        return _finalDestination != null && Vector3.Distance(_npc.transform.position, (Vector3)_finalDestination) < 2.1f;
    }
}

internal enum MovementState
{
    Idle,
    Walking,
    WalkingToCar,
    Driving
}