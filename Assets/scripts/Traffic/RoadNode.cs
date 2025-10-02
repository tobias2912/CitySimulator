using System.Collections.Generic;
using Traffic;
using UnityEngine;

public class RoadNode
{
    private readonly List<RoadNode> _nextNodes = new();
    public Vector3 WorldPosition;
    public Road Road;
    public Intersection Intersection { get; set; }
    public bool IsIntersectionEntryPoint { get; }
    public bool IsIntersectionExitPoint { get; }

    private bool _isLocked;

    public RoadNode(bool isIntersectionEntryPoint = false, bool isIntersectionExitPoint = false)
    {
        IsIntersectionEntryPoint = isIntersectionEntryPoint;
        IsIntersectionExitPoint = isIntersectionExitPoint;
        _isLocked = false;
    }

    public void Unlock()
    {
        _isLocked = false;
    }

    public bool IsLocked()
    {
        return _isLocked;
    }

    public string Name()
    {
        if (Road != null) return Road.gameObject.name;
        return "no name";
    }

    public List<RoadNode> GetNextNodes()
    {
        return _nextNodes;
    }

    public void Lock(float f)
    {
        _isLocked = true;
        //unlock after f seconds
        Debug.DrawLine(WorldPosition, WorldPosition + Vector3.up * 5, Color.red, f);
        CoroutineHelper.ExecuteAfterDelay(f, Unlock);
    }
}