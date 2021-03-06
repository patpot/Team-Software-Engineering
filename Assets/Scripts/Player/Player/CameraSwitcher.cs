using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    public static bool BuildMode;
    public MachinesInInventory MachinesInv;

    [SerializeField] private GameObject _fpsCam;
    [SerializeField] private GameObject _topDownCam;
    [SerializeField] private GameObject _gridBuildingSystem;
    [SerializeField] private GameObject _buildingGhost;
    [SerializeField] private GameObject _topDownCanvas;
    [SerializeField] private GameObject _playerCanvas;
    [SerializeField] private FirstPersonController _fpsController;
    
    void Start()
    {
        // Set our default values for the scene
        //_topDownCam.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.B))
        {
            if (UIManager.ActiveUICount > 0) return;
            
            if (BuildMode)
            {
                // Switch to first person mode
                // Re-enable our FPS camera
                _fpsCam.SetActive(true);
                _fpsController.enabled = true;
                _playerCanvas.SetActive(true);

                // Disable the grid building system
                _gridBuildingSystem.SetActive(false);
                _buildingGhost.SetActive(false);

                // Disable our top down camera and UI
                _topDownCam.SetActive(false);
                _topDownCanvas.SetActive(false);

                //// Lock the cursor again
                //UIManager.UnlockCamera();
                //UIManager.LockCursor();
            }
            else
            {
                // Switch to build mode
                // Disable our FPS camera
                _fpsCam.SetActive(false);
                _fpsController.enabled = false;
                _playerCanvas.SetActive(false);

                // Enable our grid building system
                _gridBuildingSystem.SetActive(true);
                _buildingGhost.SetActive(true);

                // Switch to our top town camera
                _topDownCam.SetActive(true);
                _topDownCanvas.SetActive(true);

                //// Unlock the cursor as we are in build mode
                //UIManager.UnlockCursor();

                // Refresh our UI for the "building inventory" which displays which machines are available to build
                MachinesInv.UpdateUI();
            }
            BuildMode = !BuildMode;
            UIManager.UpdateCameraAndCursor();
        }
    }
}
