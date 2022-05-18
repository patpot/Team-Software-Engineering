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
    //public void Start()/// GET RID OF
    //{
    //    // TODO: Load inventory from some kind of save system
    //    for (int i = 0; i < SlotCount; i++)
    //        InventorySlotData.Add(new InventorySlotData());
    //    TryDepositItem("Earth Crystal", 11f);/// GET RID OF
    //}

    public void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
            ToggleInventory();
    }

    public CraftingErrorType AttemptCraft(CraftingRecipe recipe)
    {
        // Get our inputs sorted by quantity and create a place to temporarily store which slots we're going to be changing
        Dictionary<ItemData, float> requiredInputs = CraftingManager.RecipesToQuantities(recipe);
        (Dictionary<InventorySlotData, float> slotsToChange, int leftoverQuantity) matchingInvData = ContainsItems(requiredInputs);

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