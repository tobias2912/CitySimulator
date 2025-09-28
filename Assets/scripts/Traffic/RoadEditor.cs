using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Road))]
public class RoadEditor : Editor
{
    private Road _road;
    private const float HandleSize = 0.3f;
    private bool _showEditor = false; // Flag to toggle editor visibility

    private void OnSceneGUI()
    {
        if (!_showEditor) return; // Only show editor if the flag is true

        _road = (Road)target;

        // Draw and edit right lane waypoints
        Handles.color = Color.green;
        for (var i = 0; i < _road.waypoints.Count; i++)
        {
            EditorGUI.BeginChangeCheck();

            var newPoint =
                Handles.PositionHandle(_road.transform.TransformPoint(_road.waypoints[i]), Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_road, "Move Right Lane Waypoint");
                _road.waypoints[i] = _road.transform.InverseTransformPoint(newPoint);
            }
        }


        // Draw lines between waypoints
        Handles.color = Color.green;
        for (int i = 0; i < _road.waypoints.Count - 1; i++)
        {
            Handles.color = Color.white;
            Handles.ArrowHandleCap(0, _road.transform.TransformPoint(_road.waypoints[i]),
                Quaternion.LookRotation(_road.transform.TransformPoint(_road.waypoints[i + 1]) -
                                        _road.transform.TransformPoint(_road.waypoints[i])), HandleSize * 10,
                EventType.Repaint);
        }
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label("Road Editor", EditorStyles.boldLabel);
        if (_road?.waypoints != null)
        {
            GUILayout.Label($"Waypoint Count: {_road.waypoints.Count}");
        }

        if (GUILayout.Button("Edit Node Positions"))
        {
            _showEditor = !_showEditor; // Toggle the flag
            SceneView.RepaintAll(); // Refresh the scene view to show/hide handles
        }

        if (GUILayout.Button("Add Waypoint"))
        {
            Undo.RecordObject(_road, "Add Waypoint");
            _road.waypoints.Add(new Vector3(3, 0));
        }

        SceneView.RepaintAll();
    }
}