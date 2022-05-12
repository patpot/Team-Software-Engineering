using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour
{
    private Transform _visual;
    private const int GHOST_LAYER = 11;

    private void Start()
    {
        RefreshVisual();
    }
    
    private void LateUpdate()
    {
        // Get our mouse position and move the ghost to that position
        Vector3 targetPosition = GridBuildingSystem.Instance.GetMouseWorldSnappedPosition();
        if (targetPosition == Vector3.zero) return; // If the cursor wasn't in a valid position, don't move the ghost

        targetPosition.y = 1f;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);
        transform.rotation = Quaternion.Lerp(transform.rotation, GridBuildingSystem.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f);
    }

    public void RefreshVisual()
    {
        // If we had a preview visual before, destroy it
        if (_visual)
            Destroy(_visual.gameObject);

        // FInd out which object we're going to be placing
        PlacedObjectTypeSO placedObjectTypeSO = GridBuildingSystem.Instance.GetPlacedObjectTypeSO();
        if (placedObjectTypeSO != null)
        {
            // Create a new preview version of this object
            _visual = Instantiate(placedObjectTypeSO.visual, transform, false);
            // Assign this object and all its children to the ghost layer
            _setLayerRecursive(_visual.gameObject, GHOST_LAYER);
        }
    }

    private void _setLayerRecursive(GameObject targetGameObject, int layer)
    {
        // Recursively iterate through our children and set their layers
        targetGameObject.layer = layer;
        foreach (Transform child in targetGameObject.transform)
            _setLayerRecursive(child.gameObject, layer);
    }
}

