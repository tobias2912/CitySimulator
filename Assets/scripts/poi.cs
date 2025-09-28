using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider))] // Requires any type of Collider
public class poi : MonoBehaviour
{
    public string poiName;
    public ActivityType activityType;
    [SerializeField] private float durationHours = 1;
    public float restoreAmount = 50f;
    public int maximumOccupancy = 1;
    
    private float _duration;
    private int _currentOccupancy = 0;
    [SerializeField]
    protected bool hideNPC = false;


    public void Start()
    {
        var colliderComponent = GetComponent<Collider>();
        colliderComponent.isTrigger = true;
        if (!gameObject.CompareTag("POI"))
        {
            gameObject.tag = "POI";
        }

        _duration = durationHours * 60;
    }

    public bool CanStartInteraction()
    {
        return _currentOccupancy < maximumOccupancy;
    }
    public void StartPoiInteraction(Npc npc)
    {
        Debug.Log("Starting interaction " + activityType + " for duration " + _duration);
        //destination is null to avoid repeat triggering
        npc.StartNewActivity(new NpcActivity(activityType.ToString(), _duration, null), this);
        _currentOccupancy++;
        if (hideNPC)
        {
            npc.HideNpcMesh();
        }
    }
    public void EndPoiInteraction(Npc npc)
    {
        Debug.Log("Ending interaction " + activityType);
        _currentOccupancy--;
        switch (activityType)
        {
            case ActivityType.Eat:
                npc.Stats.RestoreHunger(restoreAmount);
                break;
            case ActivityType.Recreation:
                npc.Stats.RestoreRecreation(restoreAmount);
                break;
            case ActivityType.Sleep:
                npc.Stats.RestoreSleep(restoreAmount);
                break;
        }
        if (hideNPC)
        {
            npc.ShowNpcMesh();
        }
    }
}