using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthenReplicator : BasicMachine
{
    void Awake()
    {
        MachineName = "Earthen Replicator";
        ItemData log = ItemManager.GetItemData("Wood Log");
        ItemData earthCrystal = ItemManager.GetItemData("Earth Crystal");

        Inputs.Add(earthCrystal, 1f);

        Outputs.Add(log, 1f);

        TimeToProduce = 1f;
    }
}
