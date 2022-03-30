using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class BasicMachine : MonoBehaviour
{
    [NonSerialized] public string MachineName; // Used for UI
    [NonSerialized] public float TimeToProduce; // We set TimeToProduce in script ONLY

    // Our required inputs and outputs for this machine to function. If no inputs are required the machine will constantly be producing outputs
    public Dictionary<ItemData, float> Inputs = new Dictionary<ItemData, float> ();
    public Dictionary<ItemData, float> Outputs = new Dictionary<ItemData, float> ();

    private float _productionTimer;
    private bool _isProducing;

    public Inventory InputInventory;
    public HashSet<Inventory> OutputInventories = new HashSet<Inventory>();

    // This NEEDS to be kept at Start as all derived classes use Awake to assign their inputs and we need to execute those first
    private void Start()
    {
        // Instead of making in-world input chests we store them in the machine, we create an inventory, lock all the slots to an item type and allow a max of 20 to be stored at a time
        InputInventory = gameObject.AddComponent<Inventory>();
        InputInventory.SlotCount = Inputs.Count;
        InputInventory.SlotSize = 20;
        InputInventory.LockSlots();
        foreach (var item in Inputs)
            InputInventory.TryDepositItem(item.Key, 0f);

        List<Sprite> inputIcons = new List<Sprite>();
        foreach (var input in Inputs)
            inputIcons.Add(input.Key.Icon);
        List<Sprite> outputIcons = new List<Sprite>();
        foreach (var output in Outputs)
            outputIcons.Add(output.Key.Icon);

        foreach (var mesh in GetComponentsInChildren<MeshRenderer>())
            mesh.gameObject.AddComponent<MachinePreviewUI>().SetValues(MachineName, TimeToProduce, inputIcons, outputIcons);
    }

    //public void InputItem(ItemData item, float quantity)
    //{
    //    // We assume this function is only called with valid values, if an error is thrown here something was setup wrong outside of this function.
    //    _currentInputs[item] += quantity;
    //    foreach (var inventory in InputInventories)
    //    {
    //        inventory.TryDepositItem(item, quantity);
    //    }
    //}

    public bool CheckForProduction()
    {
        // Check if our internal input inventory contains our required inputs
        (Dictionary<InventorySlotData, float> slotsToChange, int leftoverQuantity) matchingInvData = InputInventory.ContainsItems(Inputs);
        if (matchingInvData.leftoverQuantity == 0) // We can change all these slots with no leftovers, so let's do that
            foreach (var slot in matchingInvData.slotsToChange)
                slot.Key.ItemCount -= slot.Value;

        return matchingInvData.leftoverQuantity == 0;
    }

    protected virtual void Update()
    {
        if (_isProducing)
        {
            // Increment our timer by elapsed time and check if we should have produced an item.
            _productionTimer += Time.deltaTime;
            if (_productionTimer > TimeToProduce)
            {
                // It's been enough time to produce an item, actually produce it and try deposit it to a target inventory
                foreach (var item in Outputs)
                {
                    ItemData itemData = item.Key;
                    float itemCount = item.Value;
                    foreach (var inventory in OutputInventories)
                    {
                        itemCount = inventory.TryDepositItem(itemData, itemCount);
                        if (itemCount == 0f)
                            break;
                    }
                }
                // Reset our values
                _productionTimer = 0f;
                _isProducing = false;
            }
        }
        else
        {
            _isProducing = CheckForProduction();
        }
    }
}
