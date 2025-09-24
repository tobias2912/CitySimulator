using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Road))]
public class RoadEditor : Editor
{
    private Road _road;
    private const float HandleSize = 0.3f;

    private void OnSceneGUI()
    {
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
        DrawDefaultInspector();

        if (GUILayout.Button("Add Waypoint"))
        {
            Undo.RecordObject(_road, "Add Waypoint");
            _road.waypoints.Add(new Vector3(3, 0));
        }
    }
}