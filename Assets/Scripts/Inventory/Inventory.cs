using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Linq;

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

    public void ForceLoadSlots()
    {
        // TODO: Load inventory from some kind of save system
        for (int i = 0; i < SlotCount; i++)
            InventorySlotData.Add(new InventorySlotData());
    }

    /// <summary>
    /// Function to deposit an item into an inventory, returns amount of leftover items
    /// </summary>
    /// <param name="item"> Data of the item to deposit</param>
    /// <param name="itemCount"> Amount of item being deposited</param>
    public float TryDepositItem(ItemData item, float itemCount, bool allowZeroValue = false)
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
                    return 0f;
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
                return 0f;
            }
            else
            {
                // Not enough space to take the deposited amount, add on what we can and iterate
                slot.ItemCount = SlotSize;
                itemCount -= remainingSpace;
            }
        }

        // We've gone through all the matching items and couldn't get rid of our deposited amount, check for empty spaces?
        if (itemCount > 0f || (allowZeroValue && itemCount == 0))
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

                        return 0f;
                    }
                }
            }
        }
        return itemCount;
    }

    public void TryRemoveFromInventory(string itemName, float itemCount)
        => TryRemoveFromInventory(ItemManager.GetItemData(itemName), itemCount);
    public void TryRemoveFromInventory(ItemData item, float itemCount)
    {
        (Dictionary<InventorySlotData, float> slotsToChange, int leftoverQuantity) matchingInvData = ContainsItems(item, itemCount);

        bool hasInputs = matchingInvData.leftoverQuantity == 0;
        if (hasInputs)
        {
            // We definitely have the inputs, now make sure we have enough inventory space
            bool spareSlot = false;
            foreach (var slot in InventorySlotData)
            {
                if (slot.ItemCount == 0)
                {
                    spareSlot = true;
                    break;
                }
            }

            if (!spareSlot)
            {
                // We didn't have a spare slot initially, quickly simulate all of the slots that are going to change and check if any of them will be empty
                foreach (var slot in matchingInvData.slotsToChange)
                {
                    if (slot.Key.ItemCount - slot.Value <= 0)
                    {
                        spareSlot = true;
                        break;
                    }
                }
            }

            // Unfortunately we don't have enough inventory space, tell the player this and cancel
            if (!spareSlot)
                return;

            // The craft succeeded! We now need to go through all the marked slots and subtract the amount we said we needed to in order to reach this condition.
            foreach (var slot in matchingInvData.slotsToChange)
                slot.Key.ItemCount -= slot.Value;
        }
        else
        {
            return;
        }
    }
    public void SwapInventorySlotData(InventorySlotData data1, InventorySlotData data2)
    {
        // Swap position in list of two slot datas
        Inventory inv1 = data1.InventorySlot.GetInventory();
        Inventory inv2 = data2.InventorySlot.GetInventory();
        int data1Index = inv1.InventorySlotData.IndexOf(data1);
        int data2Index = inv2.InventorySlotData.IndexOf(data2);
        inv1.InventorySlotData[data1Index] = data2;
        inv2.InventorySlotData[data2Index] = data1;
    }

    // Returns a dictionary mapping slots to how much needs to be removed from them and and int equal to how much leftover quantity there will be
    public (Dictionary<InventorySlotData, float>, int) ContainsItems(ItemData data, float itemCount)
        => ContainsItems(new Dictionary<ItemData, float> { { data, itemCount } });
    public (Dictionary<InventorySlotData, float>, int) ContainsItems(Dictionary<ItemData, float> requiredInputs)
    {
        List<ItemData> inputsLeft = requiredInputs.Keys.ToList();
        Dictionary<InventorySlotData, float> slotsToChange = new Dictionary<InventorySlotData, float>();

        // Loop through every inventory slot and check if we have the matching items
        foreach (var input in requiredInputs)
        {
            ItemData itemData = input.Key;
            float requiredQuantity = input.Value;
            foreach (var slot in InventorySlotData)
            {
                // The item in this slot matches the required one, check quantities
                if (slot.ItemData == itemData)
                {
                    // If there's enough items to satisfy the requirement straight up, just add this slot
                    if (slot.ItemCount >= requiredQuantity)
                    {
                        slotsToChange.Add(slot, requiredQuantity);
                        inputsLeft.Remove(itemData);
                        // We're done without needing to fully iterate, break out
                        if (inputsLeft.Count == 0)
                            break;
                    }
                    else
                    {
                        // Wasn't enough to complete the recipe, deduct whatever we took out and move on
                        slotsToChange.Add(slot, slot.ItemCount);
                        requiredQuantity -= slot.ItemCount;
                    }
                }
            }
            // We're done without needing to fully iterate, break out
            if (inputsLeft.Count == 0)
                break;
        }

        return (slotsToChange, inputsLeft.Count);
    }

    public void LockSlots()
        => InventorySlotData.ForEach(slot => slot.Locked = true);

    public void ToggleInventory()
    {
        if (SpellbookToggle.SpellbookActive) return;
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
        UIManager.Instance.LockCamera();
        UIManager.Instance.UnlockCursor();
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

            // Assign all our object values
            slotObj.transform.SetParent(gl.transform, false);
            slotObj.SetActive(true);

            // Store our slot data and update our UI
            invSlot.SetInventory(this);
            invSlot.SetSlotData(invSlotData);
            invSlot.UpdateSlotUI();
        }

        // Create a tween fading in all the images
        Utils.FadeInUI(inventory);

        if (this != PlayerInventory.Instance)
        {
            // Adjust the player tab if we arent opening the player's inventory
            PlayerInventory.Instance.OpenFakeInventory();
        }
    }

    private void _closeInventory()
    {
        UIManager.Instance.FakePlayerInventory.SetActive(false);
        UIManager.UIActive = false;
        UIManager.Instance.UnlockCamera();
        UIManager.Instance.LockCursor();

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