using System.Collections.Generic;
using UnityEngine;

namespace Traffic
{
    public class Intersection : MonoBehaviour
    {
        // Start is called before the first frame update
        //list of entry points
        public List<Vector3> entryPoints = new();

        //list of exit points
        public List<Vector3> exitPoints = new();

        public readonly List<RoadNode> EntryNodes = new();
        
        public readonly List<RoadNode> ExitNodes = new();
        void Start()
        {
            SetupNodes();
        }

        private void SetupNodes()
        {
            Debug.Log("setup intersection nodes");
            //create list of exit nodes
            foreach (var exit in exitPoints)
            {
                var node = new RoadNode();
                node.WorldPosition = transform.TransformPoint(exit);
                ExitNodes.Add(node);
            }

            foreach (var entry in entryPoints)
            {
                var node = new RoadNode();
                node.WorldPosition = transform.TransformPoint(entry);
                node.Road = null; // Entry points do not belong to a specific road

                // Connect this entry node to all exit points
                foreach (var exit in ExitNodes)
                {
                    Debug.Log($"Connecting entry {entryPoints.IndexOf(entry)} to exit {ExitNodes.IndexOf(exit)}");
                    node.GetNextNodes().Add(exit);
                    Debug.DrawLine(node.WorldPosition, exit.WorldPosition, Color.yellow, 15f);
                }
                EntryNodes.Add(node);
            }
        }
    }
}