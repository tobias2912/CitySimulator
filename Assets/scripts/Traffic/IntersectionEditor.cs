using Traffic;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace.Traffic
{
    [CustomEditor(typeof(Intersection))]
    public class IntersectionEditor : Editor
    {
        public bool hideHandles = true;
        private Intersection _intersection;
        private const float HandleSize = 0.3f;

        private void OnSceneGUI()
        {
            _intersection = (Intersection)target;

            if (hideHandles) return;
            // Draw and edit entry points
            Handles.color = Color.red;
            for (var i = 0; i < _intersection.entryPoints.Count; i++)
            {
                EditorGUI.BeginChangeCheck();

                var newPoint = Handles.PositionHandle(_intersection.entryPoints[i], Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_intersection, "Move Entry Point");
                    _intersection.entryPoints[i] = newPoint;
                }
            }

            // Draw and edit exit points
            Handles.color = Color.blue;
            for (var i = 0; i < _intersection.exitPoints.Count; i++)
            {
                EditorGUI.BeginChangeCheck();
                var newPoint =
                    Handles.PositionHandle(_intersection.transform.TransformPoint(_intersection.exitPoints[i]),
                        Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_intersection, "Move Exit Point");
                    _intersection.exitPoints[i] = _intersection.transform.InverseTransformPoint(newPoint);
                }
            }

            // Draw lines between entry and exit points
            Handles.color = Color.white;
            foreach (var entry in _intersection.entryPoints)
            {
                foreach (var exit in _intersection.exitPoints)
                {
                    //draw arrow from entry to exit
                    Handles.ArrowHandleCap(0, _intersection.transform.TransformPoint(entry),
                        Quaternion.LookRotation(_intersection.transform.TransformPoint(exit) -
                                                _intersection.transform.TransformPoint(entry)), HandleSize * 5,
                        EventType.Repaint);
                    //draw line from entry to exit
                    Handles.DrawLine(_intersection.transform.TransformPoint(entry),
                        _intersection.transform.TransformPoint(exit));
                }
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Entry Points Count: " + _intersection.entryPoints.Count);
            EditorGUILayout.LabelField("Exit Points Count: " + _intersection.exitPoints.Count);
            if (GUILayout.Button("Add Entry Waypoint"))
            {
                Undo.RecordObject(_intersection, "Add Entry Waypoint");
                _intersection.entryPoints.Add(Vector3.zero);
            }

            if (GUILayout.Button("Add Exit Waypoint"))
            {
                Undo.RecordObject(_intersection, "Add Exit Waypoint");
                _intersection.exitPoints.Add(Vector3.zero);
            }

            if (GUILayout.Button(hideHandles ? "Show Handles" : "Hide Handles"))
            {
                Undo.RecordObject(_intersection, "Toggle Handles");
                hideHandles = !hideHandles;
            }
            SceneView.RepaintAll();
        }
    }
}