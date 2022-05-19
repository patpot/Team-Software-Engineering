using Assets.Scripts;
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
    public GameObject MainMenuUI;
    public GameObject PauseUI;
    public Sprite BlankInventorySprite = null;
    public static int ActiveUICount = 1;
    public static bool MachineUIActive = false; // Used for the tutorial
    public Spellbook Spellbook;

    void Awake()
    {
        Instance = this;

        // Use some fancy reflection magic along with Unity's asset database to pre-load all our items into one class without hard coding anything
        Object[] prefabs = Resources.LoadAll("Prefabs/UI", typeof(GameObject));
        foreach (var prefab in prefabs)
            _prefabs.Add(prefab.name, prefab as GameObject);
    }
    public static void LockCamera()
    {
        Instance.FPSController.CanLook = false;
        Instance.FPSController.CanMove = false;
    }
    public static void UnlockCamera()
    {
        Instance.FPSController.CanLook = true;
        Instance.FPSController.CanMove = true;
    }

    public static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public static void LockCursor()
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

    public void Update()
    {
        print(ActiveUICount);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUI.SetActive(!MainMenuUI.activeSelf && !PauseUI.activeSelf); // Flip the visibility of the pause menu, if the main menu is open do nothing
            if (PauseUI.activeSelf)
                ActiveUICount++;
            else
                ActiveUICount--;
            UpdateCameraAndCursor();
        }
    }

    public static void UpdateCameraAndCursor()
    {   
        if (ActiveUICount > 0)
        {
            LockCamera();
            UnlockCursor();
        }
        else if (!CameraSwitcher.BuildMode)
        {
            UnlockCamera();
            LockCursor();
        }
        else if (CameraSwitcher.BuildMode)
        {
            //UnlockCamera();
            UnlockCursor();
        }
    }
}