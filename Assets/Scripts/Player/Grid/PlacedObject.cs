using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSO.Dir dir, PlacedObjectTypeSO placedObjectTypeSO)
    {
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.prefab, worldPosition, Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(dir),0));

        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();

        placedObject._placedObjectTypeSO = placedObjectTypeSO;
        placedObject._origin = origin;
        placedObject._dir = dir;

        return placedObject;
    }
    public string Name
        => _placedObjectTypeSO.name;
    private PlacedObjectTypeSO _placedObjectTypeSO;
    private Vector2Int _origin;
    private PlacedObjectTypeSO.Dir _dir;

    public Vector2Int GetGridPosition()
    {
        return _placedObjectTypeSO.GetGridPosition(_origin, _dir);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
