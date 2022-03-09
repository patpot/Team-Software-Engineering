using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMachine : MonoBehaviour
{
    // Our required inputs and outputs for this machine to function. If no inputs are required the machine will constantly be producing outputs
    public Dictionary<Item, float> Inputs = new Dictionary<Item, float> ();
    public Dictionary<Item, float> Outputs = new Dictionary<Item, float> ();

    [System.NonSerialized] // We set TimeToProduce in script ONLY
    public float TimeToProduce;
    private float _productionTimer;
    private bool _isProducing;

    // Our current amount of items that the player has input
    private Dictionary<Item, float> _currentInputs = new Dictionary<Item, float>();

    // This NEEDS to be kept at Start as all derived classes use Awake to assign their inputs and we need to execute those first
    private void Start()
    {
        // Initialise the _currentInputs dictionary to have all the required inputs items, but no values
        foreach (var requiredItem in Inputs)
            _currentInputs.Add(requiredItem.Key, 0f);
    }

    public void InputItem(Item item, float quantity)
    {
        // We assume this function is only called with valid values, if an error is thrown here something was setup wrong outside of this function.
        _currentInputs[item] += quantity;
    }

    public bool CheckForProduction()
    {
        bool canProduce = true;
        // The condition we need to meet here is that ALL inputs have the current quantity available
        foreach (var requiredInput in Inputs)
            if (_currentInputs[requiredInput.Key] < requiredInput.Value)
                canProduce = false;

        // If we are going to start production then take away the player's input items.
        if (canProduce)
            foreach (var requiredInput in Inputs)
                _currentInputs[requiredInput.Key] -= requiredInput.Value;
        return canProduce;
    }

    protected virtual void Update()
    {
        if (_isProducing)
        {
            // Increment our timer by elapsed time and check if we should have produced an item.
            _productionTimer += Time.deltaTime;
            if (_productionTimer > TimeToProduce)
            {
                // It's been enough time to produce an item, actually produce it
                foreach (var item in Outputs)
                    item.Key.ItemCount += item.Value;

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
