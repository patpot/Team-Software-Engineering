using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMachine : MonoBehaviour
{
    // TEMP: test struct until Cameron has items/resources working
    public struct Resource
    {
        public string ResourceName;

        public Resource (string name)
        {
            ResourceName = name;
        }
    }
    // Our required inputs and outputs for this machine to function. If no inputs are required the machine will constantly be producing outputs
    public Dictionary<Resource, float> Inputs = new Dictionary<Resource, float> ();
    public Dictionary<Resource, float> Outputs = new Dictionary<Resource, float> ();

    [System.NonSerialized] // We set TimeToProduce in script ONLY
    public float TimeToProduce;
    private float _productionTimer;
    private bool _isProducing;

    // Our current amount of resources that the player has input
    private Dictionary<Resource, float> _currentInputs = new Dictionary<Resource, float>();

    // This NEEDS to be kept at Start as all derived classes use Awake to assign their inputs and we need to execute those first
    private void Start()
    {
        // Initialise the _currentInputs dictionary to have all the required inputs resources, but no values
        foreach (var requiredResource in Inputs)
            _currentInputs.Add(requiredResource.Key, 0f);
    }

    public void InputResource(Resource resource, float quantity)
    {
        // We assume this function is only called with valid values, if an error is thrown here something was setup wrong outside of this function.
        _currentInputs[resource] += quantity;
    }

    public bool CheckForProduction()
    {
        bool canProduce = true;
        // The condition we need to meet here is that ALL inputs have the current quantity available
        foreach (var requiredInput in Inputs)
            if (_currentInputs[requiredInput.Key] < requiredInput.Value)
                canProduce = false;

        // If we are going to start production then take away the player's resources.
        if (canProduce)
            foreach (var requiredInput in Inputs)
                _currentInputs[requiredInput.Key] -= requiredInput.Value;
        return canProduce;
    }

    protected virtual void Update()
    {
        if (_isProducing)
        {
            // Increment our timer by elapsed time and check if we should have produced a resource.
            _productionTimer += Time.deltaTime;
            if (_productionTimer > TimeToProduce)
            {
                // It's been enough time to produce a resource, actually produce it
                foreach (var resource in Outputs)
                {
                    // TEMP: Will be replaced by some sort of centralized resourcemanager when resources are finished
                    Debug.Log($"Player would've produced {resource.Value} {resource.Key.ResourceName}");
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
