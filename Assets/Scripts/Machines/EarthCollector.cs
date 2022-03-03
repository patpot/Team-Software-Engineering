using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthCollector : BasicMachine
{
    public void Awake()
    {
        TimeToProduce = 2f;
        Outputs[new Resource("Earth Mana")] = 1f;
    }
}
