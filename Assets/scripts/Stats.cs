using UnityEngine;

public class NpcHealthStats
{
    public float Hunger { get; private set; }
    public float Sleep { get; private set; }
    public float Recreation { get; private set; }

    private const float MaxStatValue = 100f;
    private const float MinStatValue = 0f;

    public NpcHealthStats()
    {
        //start with random stats
        Hunger = Random.Range(50f, MaxStatValue);
        Sleep = Random.Range(50f, MaxStatValue);
        Recreation = Random.Range(50f, MaxStatValue);
    }

    public void UpdateStats(float hungerDecay, float sleepDecay, float recreationDecay)
    {
        Hunger = Mathf.Clamp(Hunger - hungerDecay * Time.deltaTime, MinStatValue, MaxStatValue);
        Sleep = Mathf.Clamp(Sleep - sleepDecay * Time.deltaTime, MinStatValue, MaxStatValue);
        Recreation = Mathf.Clamp(Recreation - recreationDecay * Time.deltaTime, MinStatValue, MaxStatValue);
    }

    public void RestoreHunger(float amount)
    {
        Hunger = Mathf.Clamp(Hunger + amount, MinStatValue, MaxStatValue);
        Debug.Log("hunger restored to "+Hunger);
    }

    public void RestoreSleep(float amount)
    {
        Sleep = Mathf.Clamp(Sleep + amount, MinStatValue, MaxStatValue);
    }

    public void RestoreRecreation(float amount)
    {
        Recreation = Mathf.Clamp(Recreation + amount, MinStatValue, MaxStatValue);
    }

    public bool IsExhausted()
    {
        return Hunger <= MinStatValue || Sleep <= MinStatValue || Recreation <= MinStatValue;
    }
}