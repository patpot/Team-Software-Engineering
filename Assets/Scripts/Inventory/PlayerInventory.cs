using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerInventory : Inventory
{
    private void Awake()
    {
        InventoryName = "Player";
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            ToggleInventory();
    }
}