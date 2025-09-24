using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcInfo : MonoBehaviour
{
    private Npc selectedNpc;

    private void Update()
    {
        if (selectedNpc != null)
        {
            //set data every second
            if (Time.frameCount % 60 == 0)
            {
                setData(selectedNpc);
            }
        }
    }

    public void setData(Npc npc)
    {
        selectedNpc = npc;
        var textMeshPro = GetComponent<TMPro.TextMeshProUGUI>();

        var currentActivity = npc.GetCurrentActivity();
        //set text to show name, current activity and stats, rounded to int
        // Set text with colored values
        textMeshPro.text = $"Name: {npc.name}\n" +
                           $"Hunger: <color=green>{Mathf.RoundToInt(npc.Stats.Hunger)}</color>\n" +
                           $"Sleep: <color=blue>{Mathf.RoundToInt(npc.Stats.Sleep)}</color>\n" +
                           $"Recreation: <color=yellow>{Mathf.RoundToInt(npc.Stats.Recreation)}</color>\n" +
                           $"Activity: \n" +
                           $"<color=#00FFFF>{(currentActivity != null ? currentActivity.ActivityName : "Idle")}</color>\n";

        //create a progress bar for the current activity
        if (currentActivity != null)
        {
            var progress = 1 - (currentActivity.RemainingDuration / currentActivity.TotalDuration);
            const int progressBarLength = 10;
            var filledLength = Mathf.RoundToInt(progress * progressBarLength);
            var progressBar = new string('█', filledLength) + new string('░', progressBarLength - filledLength);
            textMeshPro.text += $"Progress: <color=orange>{progressBar}</color>\n";
        }
        else
        {
            textMeshPro.text += "Progress: <color=orange>Idle</color>\n";
        }
    }
}