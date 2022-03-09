using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    // Keep this private to tell other classes to use our publicly exposed helper functions
    private static Dictionary<string, Item> _items = new Dictionary<string, Item>();

    void Awake()
    {
        // Use some fancy reflection magic along with Unity's asset database to pre-load all our items into one class without hard coding anything
        // It's also worth nothing that since this needs to be ran before any other script we give it a custom execution order of -1 in Edit->Project Settings->Script Execution Order

        // https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html. t:(type name) will return the guids of all assets of a certain type
        string[] itemGuids = AssetDatabase.FindAssets("t:" + typeof(Item).Name);
        Item[] items = new Item[itemGuids.Length];
        // Iterate through all our located GUIDs, turn them into paths and then load the assets at those paths into our dictionary
        for (int i = 0; i < items.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(itemGuids[i]);
            var item = items[i] = AssetDatabase.LoadAssetAtPath<Item>(path);

            _items[item.name] = item;
        }
    }
    
    public static Item GetItem(string itemName)
    {
        if (_items.ContainsKey(itemName))
            return _items[itemName];
        else
        {
            Debug.Log("Error! Tried to get an item that doesn't exist! Check for typos in your script as this is a developer error!");
            throw new KeyNotFoundException();
        }
    }
}
