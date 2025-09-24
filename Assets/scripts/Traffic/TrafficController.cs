using System.Collections.Generic;
using System.Linq;
using Traffic;
using UnityEngine;

public class TrafficController : MonoBehaviour
{
    void Start()
    {
        ConnectCloseNodes();
    }

    public List<RoadNode> findRoute(Vector3 start, Vector3 end)
    {
        var roads = FindObjectsOfType<Road>();
        RoadNode closestStartNode = null;
        RoadNode closestEndNode = null;
        var closestStartDistance = float.MaxValue;
        var closestEndDistance = float.MaxValue;

        // Find the closest nodes to the start and end positions
        foreach (var road in roads)
        {
            var firstNode = road.getFirstNode();
            var lastNode = road.getLastNode();
            if (firstNode == null && lastNode == null) continue;

            CheckIfNodeIsCloser(start, end, firstNode, ref closestStartDistance, ref closestStartNode,
                ref closestEndDistance, ref closestEndNode);
            CheckIfNodeIsCloser(start, end, lastNode, ref closestStartDistance, ref closestStartNode,
                ref closestEndDistance, ref closestEndNode);
        }

        if (closestStartNode == null || closestEndNode == null)
        {
            Debug.LogWarning("No valid start or end node found for the route.");
            return new List<RoadNode>();
        }

        //log names of start and end nodes
        Debug.Log("Closest start node: " + closestStartNode.Name() + " at distance " +
                  Vector3.Distance(start, closestStartNode.WorldPosition));
        Debug.Log("Closest end node: " + closestEndNode.Name() + " at distance " +
                  Vector3.Distance(end, closestEndNode.WorldPosition));

        // If we found valid start and end nodes, build the route
        var route = new List<RoadNode>();
        var currentNode = closestStartNode;

        // Simple breadth-first search to find a path from start to end node
        var visited = new HashSet<RoadNode>();
        var queue = new Queue<RoadNode>();
        var cameFrom = new Dictionary<RoadNode, RoadNode>();
        queue.Enqueue(currentNode);
        visited.Add(currentNode);
        var pathFound = false;
        while (queue.Count > 0)
        {
            currentNode = queue.Dequeue();
            if (currentNode == closestEndNode)
            {
                pathFound = true;
                break;
            }

            if (currentNode.GetNextNodes() == null) continue;
            var nextNodes = currentNode.GetNextNodes();

            foreach (var nextNode in nextNodes.Where(nextNode => !visited.Contains(nextNode)))
            {
                visited.Add(nextNode);
                queue.Enqueue(nextNode);
                cameFrom[nextNode] = currentNode;
            }
        }

        if (pathFound)
        {
            // Reconstruct the path
            var node = closestEndNode;
            while (node != null)
            {
                route.Add(node);
                cameFrom.TryGetValue(node, out node);
            }

            route.Reverse();
        }
        else
        {
            Debug.LogWarning("No path found from start to end node.");
            return new List<RoadNode>();
        }


        //draw the route in the editor for 5 seconds
        for (var i = 0; i < route.Count - 1; i++)
        {
            Debug.DrawLine(route[i].WorldPosition, route[i + 1].WorldPosition + new Vector3(0.1f, 0.1f), Color.magenta,
                15);
        }

        foreach (var node in route)
        {
            //draw a sphere at the node position
            Debug.DrawLine(node.WorldPosition, node.WorldPosition + new Vector3(0, 1, 0), Color.yellow, 15);
        }

        return route;
    }

    private static void CheckIfNodeIsCloser(Vector3 start, Vector3 end, RoadNode node, ref float closestStartDistance,
        ref RoadNode closestStartNode, ref float closestEndDistance, ref RoadNode closestEndNode)
    {
        float distanceToStart = Vector3.Distance(start, node.WorldPosition);
        if (distanceToStart < closestStartDistance)
        {
            closestStartDistance = distanceToStart;
            closestStartNode = node;
        }

        float distanceToEnd = Vector3.Distance(end, node.WorldPosition);
        if (distanceToEnd < closestEndDistance)
        {
            closestEndDistance = distanceToEnd;
            closestEndNode = node;
        }
    }

    private void ConnectCloseNodes()
    {
        var roads = FindObjectsOfType<Road>();
        foreach (var road in roads)
        {
            //check if road has any nodes
            if (road.getFirstNode() == null || road.getLastNode() == null) continue;
            //check if any other road's first or last node is within 5 units of this road's first or last node
            foreach (var otherRoad in roads)
            {
                if (otherRoad == road) continue;
                //check if other road has any nodes
                if (otherRoad.getFirstNode() == null || otherRoad.getLastNode() == null) continue;
                if (Vector3.Distance(road.getLastNode().WorldPosition, otherRoad.getFirstNode().WorldPosition) < 5)
                {
                    road.getLastNode().GetNextNodes().Add(otherRoad.getFirstNode());
                    Debug.DrawLine(road.getLastNode().WorldPosition, otherRoad.getFirstNode().WorldPosition, Color.cyan,
                        15);
                }

                if (Vector3.Distance(road.getLastNode().WorldPosition, otherRoad.getLastNode().WorldPosition) < 5)
                {
                    road.getLastNode().GetNextNodes().Add(otherRoad.getLastNode());
                    Debug.DrawLine(road.getLastNode().WorldPosition, otherRoad.getLastNode().WorldPosition, Color.cyan,
                        15);
                }
            }
        }

        //connect intersection nodes to nearby road nodes
        var intersections = FindObjectsOfType<Intersection>();
        foreach (var intersection in intersections)
        {
            //for each entry point, find the closest road node within 5 units and connect it
            foreach (var entryNode in intersection.EntryNodes)
            {
                // var entryWorldPos = intersection.transform.TransformPoint(entryNode.WorldPosition);
                var entryWorldPos = entryNode.WorldPosition;
                var closestDistance = float.MaxValue;
                var closestNode = GetClosestNode(roads, entryWorldPos, closestDistance, null);

                if (closestNode != null)
                {
                    closestNode.GetNextNodes().Add(entryNode);
                    Debug.DrawLine(entryNode.WorldPosition, closestNode.WorldPosition, Color.cyan, 15);
                }
            }

            //for each exit point, find the closest road node within 5 units and connect it
            foreach (var exit in intersection.ExitNodes)
            {
                // var exitWorldPos = intersection.transform.TransformPoint(exit.WorldPosition);
                var exitWorldPos = exit.WorldPosition;
                var closestDistance = float.MaxValue;
                Debug.Log("Finding closest node to exit at " + exitWorldPos+" with road count "+roads.Length);
                var closestNode = ClosestNode(roads, exitWorldPos, closestDistance, null);

                if (closestNode == null)
                {
                    Debug.LogWarning("No close road node found for exit point at " + exitWorldPos);
                    continue;
                }

                if (Vector3.Distance(exitWorldPos, closestNode.WorldPosition) > 5)
                {
                    Debug.LogWarning("Exit node too far from any road node: " +
                                     Vector3.Distance(exitWorldPos, closestNode.WorldPosition));
                    continue;
                }

                exit.GetNextNodes().Add(closestNode);
                Debug.Log("Connected exit point at " + exitWorldPos + " to road node " + closestNode.Name() +
                          " at distance " + Vector3.Distance(exitWorldPos, closestNode.WorldPosition));
                Debug.DrawLine(closestNode.WorldPosition, exit.WorldPosition, Color.cyan, 15);
            }
        }
    }

    private static RoadNode ClosestNode(IEnumerable<Road> roads, Vector3 exitWorldPos, float closestDistance,
        RoadNode closestNode)
    {
        foreach (var road in roads)
        {
            if (road.getFirstNode() == null || road.getLastNode() == null) continue;
            var firstNode = road.getFirstNode();
            var lastNode = road.getLastNode();
            var distanceToFirst = Vector3.Distance(exitWorldPos, firstNode.WorldPosition);
            if (distanceToFirst < closestDistance && distanceToFirst < 5)
            {
                closestDistance = distanceToFirst;
                closestNode = firstNode;
            }

            var distanceToLast = Vector3.Distance(exitWorldPos, lastNode.WorldPosition);
            if (distanceToLast < closestDistance && distanceToLast < 5)
            {
                closestDistance = distanceToLast;
                closestNode = lastNode;
            }
        }

        return closestNode;
    }

    private static RoadNode GetClosestNode(IEnumerable<Road> roads, Vector3 entryWorldPos, float closestDistance,
        RoadNode closestNode)
    {
        foreach (var road in roads)
        {
            if (road.getFirstNode() == null || road.getLastNode() == null) continue;
            var firstNode = road.getFirstNode();
            var lastNode = road.getLastNode();
            var distanceToFirst = Vector3.Distance(entryWorldPos, firstNode.WorldPosition);
            if (distanceToFirst < closestDistance && distanceToFirst < 5)
            {
                closestDistance = distanceToFirst;
                closestNode = firstNode;
            }

            var distanceToLast = Vector3.Distance(entryWorldPos, lastNode.WorldPosition);
            if (distanceToLast < closestDistance && distanceToLast < 5)
            {
                closestDistance = distanceToLast;
                closestNode = lastNode;
            }
        }

        return closestNode;
    }
}