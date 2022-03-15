using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// InventorySlotData is a class left intentionally separate from Unity's gameobject system as it is simply a data store.
/// It is unattached from an object in order to reduce overhead from inventories and not requiring blank objects to store data.
/// If you are looking for the MonoBehaviour that references this data it is InventorySlot.cs
/// </summary>
public class InventorySlotData
{
    public ItemData ItemData;
    public bool ContainsItem => ItemData != null;
    private float _itemCount;
    public float ItemCount
    {
        get { return _itemCount; }
        set
        {
            _itemCount = value;
            if (InventorySlot != null)
                InventorySlot.UpdateSlotUI();
        }
    }
    public InventorySlot InventorySlot;
}