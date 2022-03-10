using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    // Keep this private to tell other classes to use our publicly exposed helper functions
    private static Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();

    // Inventory Fields
    public GameObjectPool InventorySlotPool;
    public GameObject InventoryUI;
    public Sprite BlankInventorySprite;
    public static bool CanRenderUI = true;

    void Awake()
    {
        Instance = this;

        // Use some fancy reflection magic along with Unity's asset database to pre-load all our items into one class without hard coding anything
        Object[] prefabs = Resources.LoadAll("Prefabs/UI", typeof(GameObject));
        foreach (var prefab in prefabs)
            _prefabs.Add(prefab.name, prefab as GameObject);
    }

    public static GameObject GetPrefab(string itemName)
    {
        if (_prefabs.ContainsKey(itemName))
            return _prefabs[itemName];
        else
        {
            Debug.Log("Error! Tried to get prefab that doesn't exist! Check for typos in your script as this is a developer error!");
            throw new KeyNotFoundException();
        }
    }
}