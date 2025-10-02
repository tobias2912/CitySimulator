using System.Collections.Generic;
using UnityEngine;

namespace Traffic
{
    public class Intersection : RoadComponent
    {
        //list of exit points
        public List<Vector3> entryPoints = new();

        public List<Vector3> exitPoints = new();

        public readonly List<RoadNode> EntryNodes = new();

        public readonly List<RoadNode> ExitNodes = new();
        private List<LockedNodePair> _lockedNodes = new();

        void Start()
        {
            SetupNodes();
        }

        private void SetupNodes()
        {
            //create list of exit nodes
            foreach (var exit in exitPoints)
            {
                var node = new RoadNode(isIntersectionExitPoint: true)
                {
                    Intersection = this,
                    WorldPosition = transform.TransformPoint(exit)
                };
                ExitNodes.Add(node);
            }

            foreach (var entry in entryPoints)
            {
                var node = new RoadNode(isIntersectionEntryPoint: true)
                {
                    Intersection = this,
                    WorldPosition = transform.TransformPoint(entry),
                };

                // Connect this entry node to all exit points
                foreach (var exit in ExitNodes)
                {
                    // Debug.Log($"Connecting entry {entryPoints.IndexOf(entry)} to exit {ExitNodes.IndexOf(exit)}");
                    node.GetNextNodes().Add(exit);
                    Debug.DrawLine(node.WorldPosition, exit.WorldPosition, Color.yellow, 15f);
                }

                EntryNodes.Add(node);
            }
        }

        public bool IsOpen(RoadNode inNode, RoadNode outNode)
        {
            foreach (var lockedPair in _lockedNodes)
            {
                if (lockedPair.outNode == outNode)
                {
                    //same exit is always locked
                    return false;
                }
                //check if the lines between inNode and outNode intersect
                var a1 = inNode.WorldPosition;
                var a2 = outNode.WorldPosition;
                var b1 = lockedPair.inNode.WorldPosition;
                var b2 = lockedPair.outNode.WorldPosition;
                if (LinesIntersect(a1, a2, b1, b2))
                {
                    return false;
                }
            }

            return true;
        }

        private bool LinesIntersect(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
        {
            var denominator = (a2.x - a1.x) * (b2.z - b1.z) - (a2.z - a1.z) * (b2.x - b1.x);
            if (denominator == 0) return false; // Parallel lines

            var ua = ((b2.x - b1.x) * (a1.z - b1.z) - (b2.z - b1.z) * (a1.x - b1.x)) / denominator;
            var ub = ((a2.x - a1.x) * (a1.z - b1.z) - (a2.z - a1.z) * (a1.x - b1.x)) / denominator;

            return ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1;
        }

        public void LockIntersection(RoadNode inNode, RoadNode outNode)
        {
            var duration = 2f;
            // outNode.Lock(duration);
            Debug.DrawLine(inNode.WorldPosition, outNode.WorldPosition, Color.red, duration);
            var lockedNodePair = new LockedNodePair(inNode, outNode);
            _lockedNodes.Add(lockedNodePair);
            CoroutineHelper.ExecuteAfterDelay(duration, () => Unlock(lockedNodePair));
        }

        private void Unlock(LockedNodePair pair)
        {
            _lockedNodes.Remove(pair);
        }
    }

    internal class LockedNodePair
    {
        public RoadNode inNode;
        public RoadNode outNode;

        public LockedNodePair(RoadNode inNode, RoadNode outNode)
        {
            this.inNode = inNode;
            this.outNode = outNode;
        }
    }
}