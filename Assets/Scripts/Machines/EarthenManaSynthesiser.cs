using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthenManaSynthesiser : BasicMachine
{
    void Awake()
    {
        ItemData earthMana = ItemManager.GetItemData("Earth Mana");
        Outputs.Add(earthMana, 1f);

        TimeToProduce = 1f;
    }
}
