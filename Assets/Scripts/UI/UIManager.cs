using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    // Keep this private to tell other classes to use our publicly exposed helper functions
    private static Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();
    public Transform MainCanvas;
    public FirstPersonController FPSController;

    // Inventory Fields
    public GameObjectPool InventorySlotPool;
    public GameObject InventoryUI;
    public GameObject FakePlayerInventory;
    public Sprite BlankInventorySprite = null;
    public static bool UIActive = false;

    void Awake()
    {
        Instance = this;

        // Use some fancy reflection magic along with Unity's asset database to pre-load all our items into one class without hard coding anything
        Object[] prefabs = Resources.LoadAll("Prefabs/UI", typeof(GameObject));
        foreach (var prefab in prefabs)
            _prefabs.Add(prefab.name, prefab as GameObject);
    }
    public void LockCamera()
    {
         FPSController.CanLook = false;
         FPSController.CanMove = false;
    }
    public void UnlockCamera()
    { 
        FPSController.CanLook = true;
        FPSController.CanMove = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static GameObject CreatePrefab(string prefabName)
        => Instantiate(GetPrefab(prefabName));
    public static GameObject GetPrefab(string prefabName)
    {
        if (_prefabs.ContainsKey(prefabName))
            return _prefabs[prefabName];
        else
        {
            Debug.Log("Error! Tried to get prefab that doesn't exist! Check for typos in your script as this is a developer error!");
            throw new KeyNotFoundException();
        }
    }
}