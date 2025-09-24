using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
    private GameObject[] _pois;
    private NpcInfo NpcInfo;
    private TimeOfDayController time;

    void Start()
    {
        _pois = GameObject.FindGameObjectsWithTag("POI");
        NpcInfo = GameObject.Find("NPCInfo").GetComponent<NpcInfo>();
        time = GameObject.Find("TimeOfDay").GetComponent<TimeOfDayController>();
    }

    public GameObject getClosestPoi(Vector3 position)
    {
        return _pois.FirstOrDefault();
    }

    public GameObject getClosestPoiOfType(Vector3 position, ActivityType type)
    {
        return _pois.FirstOrDefault(poi =>
            poi.GetComponent<poi>().activityType == type
            && poi.GetComponent<poi>().CanStartInteraction()
        );
    }

    public void SetInspectorInfo(Npc npc)
    {
        NpcInfo.setData(npc);
    }

    public apartment AssignApartment(Npc npc)
    {
        var apartments = FindObjectsOfType<apartment>();
        foreach (var apartment in apartments)
        {
            if (apartment.Npcs.Count < apartment.maximumOccupancy)
            {
                apartment.Npcs.Add(npc);
                return apartment;
            }
        }

        return null;
    }
}