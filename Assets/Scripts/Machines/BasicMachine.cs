using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BasicMachine : MonoBehaviour
{
    [NonSerialized] public string MachineName; // Used for UI
    [NonSerialized] public float TimeToProduce; // We set TimeToProduce in script ONLY

    // Our required inputs and outputs for this machine to function. If no inputs are required the machine will constantly be producing outputs
    public Dictionary<ItemData, float> Inputs = new Dictionary<ItemData, float> ();
    public Dictionary<ItemData, float> Outputs = new Dictionary<ItemData, float> ();

    private float _productionTimer;
    private bool _isProducing;

    public Inventory InternalInputInventory;
    public Inventory InternalOutputInventory;

    public List<MachineConnection> MachineConnections = new List<MachineConnection>();
    private int _connectionIndex;

    // This NEEDS to be kept at Start as all derived classes use Awake to assign their inputs and we need to execute those first
    private void Start()
    {
        // Instead of making in-world input chests we store them in the machine, we create an internal inventory, lock all the slots to an item type and allow a max of 20 to be stored at a time
        InternalInputInventory = gameObject.AddComponent<Inventory>();
        InternalInputInventory.SlotCount = Inputs.Count;
        InternalInputInventory.SlotSize = 20;
        InternalInputInventory.ForceLoadSlots();
        InternalInputInventory.LockSlots();
        // Now that we've locked our slots we need to assign it item data by faking a deposit
        foreach (var item in Inputs)
            InternalInputInventory.TryDepositItem(item.Key, 0f, true);

        InternalOutputInventory = gameObject.AddComponent<Inventory>();
        InternalOutputInventory.SlotCount = Outputs.Count;
        InternalOutputInventory.SlotSize = 20;
        InternalOutputInventory.ForceLoadSlots();
        InternalOutputInventory.LockSlots();
        // Now that we've locked our slots we need to assign it item data by faking a deposit
        foreach (var item in Outputs)
            InternalOutputInventory.TryDepositItem(item.Key, 0f, true);

        List<Sprite> inputIcons = new List<Sprite>();
        foreach (var input in Inputs)
            inputIcons.Add(input.Key.Icon);
        List<Sprite> outputIcons = new List<Sprite>();
        foreach (var output in Outputs)
            outputIcons.Add(output.Key.Icon);

        foreach (var mesh in GetComponentsInChildren<MeshRenderer>())
            mesh.gameObject.AddComponent<MachinePreviewUI>().SetValues(MachineName, TimeToProduce, inputIcons, outputIcons, InternalInputInventory, InternalOutputInventory);
    }

    public bool CheckForProduction()
    {
        // Check if our internal input inventory contains our required inputs
        (Dictionary<InventorySlotData, float> slotsToChange, int leftoverQuantity) matchingInvData = InternalInputInventory.ContainsItems(Inputs);
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
                OutputItems();
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

    public void RemoveConnection(MachineConnection connection)
    {
        MachineConnections.Remove(connection);
        _connectionIndex = MachineConnections.Count == 0 ? 0 : _connectionIndex %= MachineConnections.Count;
    }

    public void OutputItems()
    {
        foreach (var item in Outputs)
        {
            ItemData itemData = item.Key;
            float itemCount = item.Value;
            // No registered connections, use our local inventory
            if (MachineConnections.Count == 0)
            {
                InternalOutputInventory.TryDepositItem(itemData, itemCount);
                continue;
            }

            MachineConnections[_connectionIndex].SendItemStack(itemData, itemCount);
        }

        // Round robin through our connections
        if (MachineConnections.Count > 0)
        {
            _connectionIndex++;
            _connectionIndex %= MachineConnections.Count;
        }
    }
}
