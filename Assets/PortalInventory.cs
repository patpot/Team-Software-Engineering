using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalInventory : Inventory
{
    // Start is called before the first frame update
    void Awake()
    {
        InventoryName = "Portal";

        foreach (var invSlot in UIManager.Instance.FakePlayerInventory.GetComponentsInChildren<InventorySlot>())
            invSlot.SetInventory(this);
    }

    private void Start()
    {
        for (int i = 0; i < SlotCount; i++)
            InventorySlotData.Add(new InventorySlotData());
    }

    private void Update()
    {
        if (GetItemCount("Earth Crystal") >= 100)
            print("you win lol");
    }
}
