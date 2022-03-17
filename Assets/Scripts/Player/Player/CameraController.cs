using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Movement Variables")]
    [SerializeField] private float _normalSpeed;
    [SerializeField] private float _fastSpeed;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _movementTime;
    [SerializeField] private float _minPosition;
    [SerializeField] private float _maxPosition;

    [Header("Rotation Variables")]
    [SerializeField] private float _rotationAmount;

    [Header("Zoom Variables")]
    [SerializeField] private Vector3 _zoomAmount;
    [SerializeField] private Vector3 _mouseZoomAmount;
    [SerializeField] private float _minZoom;
    [SerializeField] private float _maxZoom;

    [Header("Current Positions")]
    [SerializeField] private Vector3 _newPosition;
    [SerializeField] private Quaternion _newRotation;
    [SerializeField] private Vector3 _newZoom;
    [SerializeField] private Vector3 _startPosition;

    [Header("Mouse Variables")]
    [SerializeField] private Vector3 _dragStartPosition;
    [SerializeField] private Vector3 _dragCurrentPosition;
    [SerializeField] private Vector3 _rotateStartPosition;
    [SerializeField] private Vector3 _rotateCurrentPosition;

    void Start()
    {
        _startPosition = transform.position;
        _newPosition = transform.position;
        _newRotation = transform.rotation;
        _newZoom = cameraTransform.localPosition;
    }

    void LateUpdate()
    {
        HandleMouseInput();
        HandleMovementInput();
    }

    void HandleMouseInput()
    {
        if (Input.mouseScrollDelta.y != 0)
            _newZoom += Input.mouseScrollDelta.y * _mouseZoomAmount;

        if (Input.GetMouseButtonDown(1))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if(plane.Raycast(ray, out entry))
                _dragStartPosition = ray.GetPoint(entry);
        }
        if (Input.GetMouseButton(1))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                _dragCurrentPosition = ray.GetPoint(entry);

                _newPosition = transform.position + _dragStartPosition - _dragCurrentPosition;
            }
        }

        if (Input.GetMouseButtonDown(2))
            _rotateStartPosition = Input.mousePosition;
        
        if (Input.GetMouseButton(2))
        {
            _rotateCurrentPosition = Input.mousePosition;

            Vector3 difference = _rotateStartPosition - _rotateCurrentPosition;

            _rotateStartPosition = _rotateCurrentPosition;

            _newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
        }

    }

    void HandleMovementInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            _movementSpeed = _fastSpeed;
        else
            _movementSpeed = _normalSpeed;
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            _newPosition += transform.forward * _movementSpeed;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            _newPosition += transform.forward * -_movementSpeed;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            _newPosition += transform.right * _movementSpeed;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            _newPosition += transform.right * -_movementSpeed;

        if (Input.GetKey(KeyCode.Q))
            _newRotation *= Quaternion.Euler(Vector3.up * _rotationAmount);
        if (Input.GetKey(KeyCode.E))
            _newRotation *= Quaternion.Euler(Vector3.up * -_rotationAmount);

        if (Input.GetKey(KeyCode.R))
            _newZoom += _zoomAmount;
        if (Input.GetKey(KeyCode.F))
            _newZoom -= _zoomAmount;

        _newZoom.y = Mathf.Clamp(_newZoom.y, -_minZoom, _maxZoom);
        _newZoom.z = Mathf.Clamp(_newZoom.z, -_maxZoom, _minZoom);

        _newPosition = new Vector3(Mathf.Clamp(_newPosition.x, _startPosition.x + _minPosition, _startPosition.x + _maxPosition),0.1f, Mathf.Clamp(_newPosition.z, _startPosition.z + _minPosition, _startPosition.z + _maxPosition));

        transform.position = Vector3.Lerp(transform.position, _newPosition, Time.deltaTime * _movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation, Time.deltaTime * _movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, _newZoom, Time.deltaTime * _movementTime);
    }
}
