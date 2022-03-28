using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthenDiffuser : BasicMachine
{
    void Awake()
    {
        MachineName = "Earthen Diffuser";
        ItemData log = ItemManager.GetItemData("Wood Log");
        ItemData earthMana = ItemManager.GetItemData("Earth Mana");

        Inputs.Add(log, 1f);

        Outputs.Add(earthMana, 1f);

        TimeToProduce = 1f;
    }
}