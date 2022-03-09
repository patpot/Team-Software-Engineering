using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthenManaSynthesiser : BasicMachine
{
    void Awake()
    {
        Item earthMana = ItemManager.GetItem("Earth Mana");
        Outputs.Add(earthMana, 1f);

        TimeToProduce = 5f;
    }
}
