using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    // Keep this private to tell other classes to use our publicly exposed helper functions
    private static Dictionary<string, ItemData> _itemData = new Dictionary<string, ItemData>();

    void Awake()
    {
        // Load in all our items from our Resources/ItemData folder
        Object[] itemDatas = Resources.LoadAll("ItemData", typeof(ItemData));
        foreach (var itemData in itemDatas)
            _itemData.Add(itemData.name, itemData as ItemData);
    }
    
    public static ItemData GetItemData(string itemName)
    {
        if (_itemData.ContainsKey(itemName))
            return _itemData[itemName];
        else
        {
            Debug.Log($"Error! Tried to get item data that doesn't exist! {itemName}. Check for typos in your script as this is a developer error!");
            throw new KeyNotFoundException();
        }
    }
}