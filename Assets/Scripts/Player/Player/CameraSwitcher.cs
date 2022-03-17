using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject _fpsCam;
    [SerializeField] private GameObject _topDownCam;
    [SerializeField] private GameObject _gridBuildingSystem;

    [SerializeField] private Canvas _fpsCanvas;
    [SerializeField] private Canvas _topDownCanvas;
    

    private FirstPersonController _fpsController;

    // Start is called before the first frame update
    void Start()
    {
        _fpsController = GetComponent<FirstPersonController>();

        _fpsCam.SetActive(true);
        _fpsCanvas.enabled = true;
        _fpsController.enabled = true;

        _gridBuildingSystem.SetActive(false);

        _topDownCam.SetActive(false);
        _topDownCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.C) && _fpsCam.activeInHierarchy)
        {
            _fpsCam.SetActive(false);
            _fpsCanvas.enabled = false;
            _fpsController.enabled = false;

            _gridBuildingSystem.SetActive(true);

            _topDownCam.SetActive(true);
            _topDownCanvas.enabled = true;

        }
        else if (Input.GetKeyUp(KeyCode.C) && _topDownCam.activeInHierarchy)
        {
            _fpsCam.SetActive(true);
            _fpsCanvas.enabled = true;
            _fpsController.enabled = true;

            _gridBuildingSystem.SetActive(false);

            _topDownCam.SetActive(false);
            _topDownCanvas.enabled = false;
        }
    }
}
