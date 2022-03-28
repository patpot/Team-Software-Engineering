using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthenCrusher : BasicMachine
{
    void Awake()
    {
        MachineName = "Earthen Crusher";
        ItemData earthMana = ItemManager.GetItemData("Earth Mana");
        ItemData woodLog = ItemManager.GetItemData("Wood Log");
        ItemData branch = ItemManager.GetItemData("Branch");

        Inputs.Add(earthMana, 1f);
        Inputs.Add(woodLog, 1f);

        Outputs.Add(branch, 1f);

        TimeToProduce = 1f;
    }
}
