using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthenCrystalliser : BasicMachine
{
    void Awake()
    {
        MachineName = "Earthen Crystalliser";
        ItemData earthMana = ItemManager.GetItemData("Earth Mana");
        ItemData earthCrystal = ItemManager.GetItemData("Earth Crystal");

        Inputs.Add(earthMana, 1f);

        Outputs.Add(earthCrystal, 1f);

        TimeToProduce = 1f;
    }
}