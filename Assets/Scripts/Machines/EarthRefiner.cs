using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthRefiner : BasicMachine
{
    private Resource _testResource;
    private void Awake()
    {
        TimeToProduce = 2f;

        _testResource = new Resource("Earth Mana");
        Inputs[_testResource] = 5f; // Require 5 mana to produce

        Outputs[new Resource("Earth Crystal")] = 1f; // Produce 1 Earth Crystal
    }

    protected override void Update()
    {
        // TEMP: Allow us to test input purely functionally
        if (Input.GetKey(KeyCode.P))
            InputResource(_testResource, 1f);

        base.Update();
    }
}
