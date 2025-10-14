using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class Npc : MonoBehaviour
{
    public NpcHealthStats Stats;
    public apartment apartment;

    private GameController _gameController;

    private NpcActivity _currentActivity;
    private NpcMovement _npcMovement;
    private readonly List<string> _logMessages = new();

    [CanBeNull] private poi _currentPoi;
    private float _maxWalkingDuration;

    private void Start()
    {
        Stats = new NpcHealthStats();
        _gameController = GameObject.Find("GameController").GetComponent<GameController>();
        var trafficController = GameObject.Find("TrafficController").GetComponent<TrafficController>();
        _npcMovement = new NpcMovement(this, trafficController);
        apartment = _gameController.AssignApartment(this);
        //set a random color of child mesh renderer
        var meshRenderer = GetComponentInChildren<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.material.color =
                new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        }
        else
        {
            Debug.LogWarning("No MeshRenderer found on NPC " + gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter " + other.gameObject.name);
    }

    private void Update()
    {
        Stats.UpdateStats(0.1f, 0.1f, 0.1f);
        if (_currentActivity == null)
        {
            ChooseNewActivity();
            return;
        }

        _npcMovement.Update();

        if (_npcMovement.IsAtDestination())
        {
            // Debug.Log("Reached " + _currentPoi?.activityType);
            _currentActivity.CompleteActivity();
            if (_currentPoi != null)
            {
                _currentPoi.StartPoiInteraction(this);
            }
            else
            {
                _currentActivity = null;
            }

            return;
        }

        // check if duration for current activity is over
        if (_currentActivity is { RemainingDuration: <= 0 })
        {
            if (_currentPoi != null && _currentActivity.Destination == null)
            {
                //PoI has no destination and is finished
                _currentPoi.EndPoiInteraction(this);
            }

            _currentActivity.CompleteActivity();
            _currentActivity = null;
            _currentPoi = null;
        }
        else if (_currentActivity != null)
        {
            _currentActivity.RemainingDuration -= Time.deltaTime;
        }
    }

    public void StartNewActivity(NpcActivity npcActivity, [CanBeNull] poi poi, bool hideNPC = false)
    {
        _currentActivity = npcActivity;
        _currentActivity.StartActivity(this, npcActivity.Destination);
        if (npcActivity.Destination != null)
        {
            TravelToPosition((Vector3)npcActivity.Destination);
        }

        if (poi != null) _currentPoi = poi.GetComponent<poi>();
        if (hideNPC) HideNpcMesh();
    }

    private void OnMouseDown()
    {
        _gameController.SetInspectorInfo(this);
    }

    private void ChooseNewActivity()
    {
        _maxWalkingDuration = 30f;
        if (Stats.Hunger < 70f)
        {
            var closestPoi = _gameController.getClosestPoiOfType(transform.position, ActivityType.Eat);
            if (closestPoi != null)
            {
                var poiPos = closestPoi.transform.position;
                StartNewActivity(new NpcActivity("Finding something to eat", _maxWalkingDuration, poiPos),
                    closestPoi.GetComponent<poi>());
                return;
            }

            Debug.Log("Nowhere to Eat");
        }

        if (Stats.Sleep < 70f)
        {
            if (apartment != null)
            {
                var apartmentPos = apartment.transform.position;
                StartNewActivity(new NpcActivity("going home", _maxWalkingDuration, apartmentPos),
                    apartment.GetComponent<poi>());
                return;
            }
        }

        if (Stats.Recreation < 70f)
        {
            var poi = _gameController.getClosestPoiOfType(transform.position, ActivityType.Recreation);
            if (poi != null)
            {
                var poiPos = poi.transform.position;
                StartNewActivity(new NpcActivity("walking to " + poi.name, _maxWalkingDuration, poiPos),
                    poi.GetComponent<poi>());
            }
        }
        else
        {
            //loiter
            var randomDirection = UnityEngine.Random.insideUnitSphere * 7f;
            randomDirection += transform.position;
            randomDirection.y = transform.position.y; // keep the same height
            StartNewActivity(new NpcActivity("loitering", 30f, randomDirection), null);
        }
    }

    private void TravelToPosition(Vector3 destination)
    {
        _npcMovement.SetDestination(destination);
    }

    public NpcActivity GetCurrentActivity()
    {
        return _currentActivity;
    }

    public void HideNpcMesh()
    {
        var meshRenderer = GetComponentInChildren<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
    }

    public void ShowNpcMesh()
    {
        var meshRenderer = GetComponentInChildren<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
        }
    }

    public void LogMessage(string message)
    {
        _logMessages.Add($"{DateTime.Now:mm:ss} - {message}");
        if (_logMessages.Count > 10)
        {
            _logMessages.RemoveAt(0);
        }
    }


    public IEnumerable GetLogMessages()
    {
        return _logMessages;
    }
}