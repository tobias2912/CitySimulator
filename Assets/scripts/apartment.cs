using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class apartment : poi
{
    //list of npcs in the apartment
    public List<Npc> Npcs = new List<Npc>();

    public apartment()
    {
        hideNPC = true;
    }
}
