using Assets.Scripts;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.CraftingManager;

public class PlayerInventory : Inventory
{
    public static PlayerInventory Instance;
    private void Awake()
    {
        Instance = this;
        InventoryName = "Player";

        foreach (var invSlot in UIManager.Instance.FakePlayerInventory.GetComponentsInChildren<InventorySlot>())
            invSlot.SetInventory(this);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            ToggleInventory();
    }

    public CraftingErrorType AttemptCraft(CraftingRecipe recipe)
    {
        // Get our inputs sorted by quantity and create a place to temporarily store which slots we're going to be changing
        Dictionary<ItemData, float> requiredInputs = CraftingManager.RecipesToQuantities(recipe);
        //List<ItemData> inputsLeft = requiredInputs.Keys.ToList();
        //Dictionary<InventorySlotData, float> slotsToChange = new Dictionary<InventorySlotData, float>();

        (Dictionary<InventorySlotData, float> slotsToChange, int leftoverQuantity) matchingInvData = ContainsItems(requiredInputs);
        // Loop through every inventory slot and check if we have the matching items

        //foreach (var input in requiredInputs)
        //{
        //    ItemData itemData = input.Key;
        //    float requiredQuantity = input.Value;
        //    foreach (var slot in InventorySlotData)
        //    {
        //        // The item in this slot matches the required one, check quantities
        //        if (slot.ItemData == itemData)
        //        {
        //            // If there's enough items to satisfy the requirement straight up, just add this slot
        //            if (slot.ItemCount >= requiredQuantity)
        //            {
        //                slotsToChange.Add(slot, requiredQuantity);
        //                inputsLeft.Remove(itemData);
        //                // We're done without needing to fully iterate, break out
        //                if (inputsLeft.Count == 0)
        //                    break;
        //            }
        //            else
        //            {
        //                // Wasn't enough to complete the recipe, deduct whatever we took out and move on
        //                slotsToChange.Add(slot, slot.ItemCount);
        //                requiredQuantity -= slot.ItemCount;
        //            }
        //        }
        //    }
        //    // We're done without needing to fully iterate, break out
        //    if (inputsLeft.Count == 0)
        //        break;
        //}


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
                return CraftingErrorType.NoSpareSlots;

            // The craft succeeded! We now need to go through all the marked slots and subtract the amount we said we needed to in order to reach this condition.
            foreach (var slot in matchingInvData.slotsToChange)
                slot.Key.ItemCount -= slot.Value;
            // Now that's done, give the player their well earned output
            TryDepositItem(recipe.Output, recipe.OutputQuantity);
            return CraftingErrorType.NoError;
        }
        else
        {
            return CraftingErrorType.NoInputs;
        }
    }

    public void OpenFakeInventory()
    {
        GameObject fakeInventory = UIManager.Instance.FakePlayerInventory;
        fakeInventory.SetActive(true);

        InventorySlot[] invSlots = fakeInventory.GetComponentsInChildren<InventorySlot>();
        for (int i = 0; i < invSlots.Length; i++)
        {
            InventorySlot invSlot = invSlots[i];
            invSlot.SetSlotData(InventorySlotData[i]);
            invSlot.UpdateSlotUI();
        }

        // Create a tween fading in all the images
        Utils.FadeInUI(fakeInventory);
    }
}