using System;
using System.Collections;
using System.Collections.Generic;
using Traffic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

public class Road : RoadComponent
{
    public List<Vector3> waypoints = new List<Vector3>();
    private RoadNode firstNode;
    private RoadNode lastNode;

    void Start()
    {
        SetupNodes();
    }

    public RoadNode getFirstNode()
    {
        return firstNode;
    }

    public RoadNode getLastNode()
    {
        return lastNode;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;

        // Draw lines between consecutive points
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(transform.TransformPoint(waypoints[i]), transform.TransformPoint(waypoints[i + 1]));
        }

        // Highlight the first waypoint
        if (waypoints.Count > 0)
        {
            Gizmos.color = Color.white; // Use a different color for the first waypoint
            Gizmos.DrawSphere(transform.TransformPoint(waypoints[0]), 0.3f); // Draw a sphere at the first waypoint
        }
    }

    private void SetupNodes()
    {
        //for each waypoint create a node and set next to the next waypoint
        RoadNode head = null;
        RoadNode current = null;
        foreach (var waypoint in waypoints)
        {
            var node = new RoadNode();
            node.WorldPosition = transform.TransformPoint(waypoint);
            node.Road = this;
            if (head == null)
            {
                head = node;
                current = head;
            }
            else
            {
                current.GetNextNodes().Add(node);
                current = node;
            }

            lastNode = node;
        }

        firstNode = head;
    }
}

public static class CoroutineHelper
{
    private class CoroutineRunner : MonoBehaviour
    {
    }

    private static CoroutineRunner _runner;

    private static CoroutineRunner Runner
    {
        get
        {
            if (_runner == null)
            {
                var runnerObject = new GameObject("CoroutineHelper");
                _runner = runnerObject.AddComponent<CoroutineRunner>();
                GameObject.DontDestroyOnLoad(runnerObject);
            }

            return _runner;
        }
    }

    public static string ExecuteAfterDelay(float delay, Action action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        Runner.StartCoroutine(ExecuteAfterDelayCoroutine(delay, action));
        return "Coroutine started";
    }

    private static IEnumerator ExecuteAfterDelayCoroutine(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }
}