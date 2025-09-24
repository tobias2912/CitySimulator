using UnityEngine;

public class NpcActivity
{
    public string ActivityName { get; private set; }
    public float RemainingDuration { get; set; }
    public float TotalDuration { get; private set; }
    public Vector3? Destination { get; private set; }

    public NpcActivity(string activityName, float remainingDuration, Vector3? destination)
    {
        ActivityName = activityName;
        RemainingDuration = remainingDuration;
        Destination = destination;
        TotalDuration = remainingDuration;
    }


    public void StartActivity(Npc npc, Vector3? target)
    {
        if (target != null) Debug.DrawLine(npc.transform.position, (Vector3)target, Color.red, 5);
        // Debug.Log($"Starting activity: {ActivityName}");
    }

    public void CompleteActivity()
    {
        // Debug.Log($"Finished {ActivityName} completed.");
    }
}