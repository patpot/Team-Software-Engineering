using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Inventory : MonoBehaviour
{
    public string InventoryName;
    public float SlotSize;
    public int SlotCount;
    public List<InventorySlotData> InventorySlotData = new List<InventorySlotData>();

    public const int MAX_COLUMNS = 10;

    public void Start()
    {
        // TODO: Load inventory from some kind of save system
        for (int i = 0; i < SlotCount; i++)
            InventorySlotData.Add(new InventorySlotData());
    }

    /// <summary>
    /// Function to deposit an item into an inventory, returns true/false if successful
    /// </summary>
    /// <param name="item"> Data of the item to deposit</param>
    /// <param name="itemCount"> Amount of item being deposited</param>
    public bool TryDepositItem(ItemData item, float itemCount)
    {
        // Check first if there's any stacks we can add these items onto
        List<InventorySlotData> matchingSlotsWithSpace = new List<InventorySlotData>();
        float spareSlotCount = 0f;
        foreach (var slot in InventorySlotData)
        {
            // Matches our item and isn't full
            if (slot.ItemData == item && slot.ItemCount < SlotSize)
            {
                float remainingSpace = SlotSize - slot.ItemCount;
                if (remainingSpace >= itemCount)
                {
                    // There's enough room to just add on the deposited amount
                    slot.ItemCount += itemCount;
                    return true;
                }
                else
                {
                    // There's space, but not enough, keep this one in mind for later
                    matchingSlotsWithSpace.Add(slot);
                    spareSlotCount += remainingSpace;

                    if (spareSlotCount >= itemCount)
                    {
                        // We've found enough slots to fill our condition, break out of the loop early
                        break;
                    }
                }
            }
        }

        foreach (var slot in matchingSlotsWithSpace)
        {
            float remainingSpace = SlotSize - slot.ItemCount;
            if (remainingSpace >= itemCount)
            {
                // There's enough space in here to take the deposited amount, simply add it on and return
                slot.ItemCount += itemCount;
                return true;
            }
            else
            {
                // Not enough space to take the deposited amount, add on what we can and iterate
                slot.ItemCount = SlotSize;
                itemCount -= remainingSpace;
            }
        }

        // We've gone through all the matching items and couldn't get rid of our deposited amount, check for empty spaces?
        if (itemCount > 0)
        {
            foreach (var slot in InventorySlotData)
            {
                // No items assigned meaning it's empty
                if (slot.ItemData == null)
                {
                    // We have too many items for just one slot, fill up this slot and continue the loop
                    if (itemCount > SlotSize)
                    {
                        slot.ItemData = item;
                        slot.ItemCount = SlotSize;

                        itemCount -= SlotSize;
                    }
                    else if (itemCount <= SlotSize) // We have less items than the slot can take, set it to be this amount and we are done.
                    {
                        slot.ItemData = item;
                        slot.ItemCount = itemCount;

                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void SwapInventorySlotData(InventorySlotData data1, InventorySlotData data2)
    {
        // Swap position in list of two slot datas
        int data1Index = InventorySlotData.IndexOf(data1);
        int data2Index = InventorySlotData.IndexOf(data2);
        InventorySlotData[data1Index] = data2;
        InventorySlotData[data2Index] = data1;
    }

    public void ToggleInventory()
    {
        GameObject inventory = UIManager.Instance.InventoryUI;
        if (!inventory.activeSelf && UIManager.UIActive) return; // Don't draw any new UI if we have UI active
        inventory.SetActive(!inventory.activeSelf);
        if (inventory.activeSelf)
            _drawInventory();
        else
            _closeInventory();
    }

    private void _drawInventory()
    {
        UIManager.UIActive = true;
        GameObject inventory = UIManager.Instance.InventoryUI;
        // Change title text
        inventory.GetComponentInChildren<TextMeshProUGUI>().text = InventoryName + " Inventory";

        int slotCount = InventorySlotData.Count;
        int columnCount = 0;
        int rowCount = 0;
        // Calculate ideal size
        for (int i = MAX_COLUMNS; i > 0; i--)
        {
            if (slotCount % i == 0)
            {
                columnCount = i;
                break;
            }
        }

        if (columnCount == 0)
        {
            Debug.LogError("ERROR! Tried to make an inventory with a prime number as the slot count but we require full rows.");
            return;
        }

        rowCount = slotCount / columnCount;

        // Set our constraints
        GridLayoutGroup gl = inventory.GetComponentInChildren<GridLayoutGroup>();
        gl.constraintCount = columnCount;
        // Set correct height and width for our objects before we add any slots
        float inventoryBackgroundWidth = 100 + (columnCount * 105); // 100 is default size, we then add on 105 for each column
        float inventoryTopBarWidth = inventoryBackgroundWidth + 55 + (3f * columnCount);
        float inventoryBackgroundHeight = 180 + ((rowCount-1) * 120) - (rowCount * 2); // 180 is default, 120 per 2 but -2 each extra row

        float inventoryXAdjustment = (inventoryBackgroundWidth - 600) / 2f;
        inventory.transform.localPosition = new Vector3(-325f, 200f) - new Vector3(inventoryXAdjustment, 0);

        RectTransform background = inventory.GetComponentsInChildren<RectTransform>()[1];
        background.sizeDelta = new Vector2(inventoryBackgroundWidth, inventoryBackgroundHeight);
        RectTransform topBar = inventory.GetComponentsInChildren<RectTransform>()[2];
        topBar.sizeDelta = new Vector2(inventoryTopBarWidth, topBar.sizeDelta.y);

        RectTransform slotHolder = inventory.GetComponentsInChildren<RectTransform>()[4];
        slotHolder.anchoredPosition = new Vector3(-270 + 2.5f * columnCount, 153.5f + 0f); // -270 and 153.5 are starting values, we use them as our start point

        GameObjectPool slotPool = UIManager.Instance.InventorySlotPool;
        // Draw our slots
        foreach (var invSlotData in InventorySlotData)
        {
            GameObject slotObj = slotPool.GetObjectFromPool();

            // Get a reference to our InventorySlot monobehaviour attached to the object and store it in our data class
            var invSlot = slotObj.GetComponent<InventorySlot>();
            // Store our slot data and update our UI
            invSlot.SetInventory(this);
            invSlot.SetSlotData(invSlotData);
            invSlot.UpdateSlotUI();

            // Assign all our object values
            slotObj.GetComponent<InventorySlot>().SetSlotData(invSlotData);
            slotObj.transform.SetParent(gl.transform, false);
            slotObj.SetActive(true);
        }

        // Create a tween fading in all the images
        foreach (var image in inventory.GetComponentsInChildren<Image>())
        {
            image.color = new Color(1, 1, 1, 0);
            image.DOColor(new Color(1, 1, 1, 1), 0.4f);
        }
    }

    private void _closeInventory()
    {
        UIManager.UIActive = false;
        GameObject inventory = UIManager.Instance.InventoryUI;
        GridLayoutGroup gl = inventory.GetComponentInChildren<GridLayoutGroup>();
        GameObjectPool slotPool = UIManager.Instance.InventorySlotPool;

        // Loop through and return all slots back to their pool
        for (int i = gl.transform.childCount - 1; i >= 0; i--)
            slotPool.ReturnToPool(gl.transform.GetChild(i).gameObject);

        foreach (var slot in InventorySlotData)
            slot.InventorySlot = null;
    }
}