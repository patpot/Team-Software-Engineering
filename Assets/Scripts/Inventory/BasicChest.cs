using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicChest : Inventory
{
    private void Awake()
    {
        InventoryName = "Basic Chest";
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            if (UIManager.Instance.InventoryUI.activeSelf)
                ToggleInventory();
    }
    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (SpellbookToggle.SpellbookActive) return;
            if (UIManager.UIActive) return;
            if (Vector3.Distance(transform.position, Camera.main.transform.position) > 5f) return;
            
            ToggleInventory();
        }
    }
}
