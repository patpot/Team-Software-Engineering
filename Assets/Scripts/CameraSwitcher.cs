using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private Camera _fpsCam;
    [SerializeField] private Camera _topDownCam;
    [SerializeField] private Canvas _fpsCanvas;
    [SerializeField] private Canvas _topDownCanvas;
    private FirstPersonController _fpsController;

    // Start is called before the first frame update
    void Start()
    {
        _fpsController = GetComponent<FirstPersonController>();

        _fpsCam.enabled = true;
        _fpsCanvas.enabled = true;
        _fpsController.enabled = true;

        _topDownCam.enabled = false;
        _topDownCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.C) && (_fpsCam.enabled == true))
        {
            _fpsCam.enabled = false;
            _fpsCanvas.enabled = false;
            _fpsController.enabled = false;

            _topDownCam.enabled = true;
            _topDownCanvas.enabled = true;

        }
        else if (Input.GetKeyUp(KeyCode.C) && (_topDownCam.enabled == true))
        {
            _fpsCam.enabled = true;
            _fpsCanvas.enabled = true;
            _fpsController.enabled = true;

            _topDownCam.enabled = false;
            _topDownCanvas.enabled = false;
        }
    }
}
