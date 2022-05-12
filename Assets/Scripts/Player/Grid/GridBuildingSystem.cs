using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CodeMonkey.Utils;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem Instance { get; private set; }

    public List<Button> BuildingButtons;
    private List<GameObject> _buildingButtonConfirms = new List<GameObject>();

    public MachinesInInventory MachinesInv;
    public BuildingGhost BuildingGhost;

    [SerializeField] private List<PlacedObjectTypeSO> _placedObjectTypeSOList;
    [SerializeField] private List<float> _placedObjectsOffsets;
    [SerializeField] private Dictionary<string, float> _objectsToOffsets;
    private PlacedObjectTypeSO _placedObjectTypeSO;

    private GridSystem<GridObject> _grid;
    private PlacedObjectTypeSO.Dir _dir = PlacedObjectTypeSO.Dir.Down;

    private void Awake()
    {
        Instance = this;
        _objectsToOffsets = new Dictionary<string, float>();
        for (int i = 0; i < _placedObjectTypeSOList.Count; i++)
            _objectsToOffsets[_placedObjectTypeSOList[i].name] = _placedObjectsOffsets[i];

        int gridWidth = 17;
        int gridHeight = 25;
        float cellSize = 5f;
        _grid = new GridSystem<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(-40, 0, -70), (GridSystem<GridObject> g, int x, int z) => new GridObject(g, x, z));

        for (int i = 0; i < BuildingButtons.Count; i++)
        {
            // Assign our on click event to select this machine
            int index = i;
            BuildingButtons[i].onClick.AddListener(delegate { _selectMachine(index); });

            // Store reference to our confirm object, then disable it by default
            GameObject buildingButtonConfirm = BuildingButtons[i].transform.parent.Find("Confirm")?.gameObject;
            if (buildingButtonConfirm == null) continue;

            _buildingButtonConfirms.Add(buildingButtonConfirm);
            buildingButtonConfirm.SetActive(false);
        }
    }

    public class GridObject
    {
        private GridSystem<GridObject> _grid;
        private int _x;
        private int _z;
        private PlacedObject _placedObject;

        public GridObject(GridSystem<GridObject> grid, int x, int z)
        {
            this._grid = grid;
            this._x = x;
            this._z = z;
            _placedObject = null;
        }

        public void SetPlacedObject(PlacedObject placedObject)
        {
            this._placedObject = placedObject;
            _grid.TriggerGridObjectChanged(_x, _z);
        }

        public PlacedObject GetPlacedObject()
            => _placedObject;

        public void ClearPlacedObject()
        {
            _placedObject = null;
            _grid.TriggerGridObjectChanged(_x, _z);
        }

        public bool CanBuild()
            => _placedObject == null;

        public override string ToString()
        {
            return _x + ", " + _z + "\n" + _placedObject;
        }
    }

    private void Update()
    {
        // Place an object
        if (Input.GetMouseButtonDown(0) && _placedObjectTypeSO != null && !EventSystem.current.IsPointerOverGameObject())
        {
            // Get our mouse position in terms of grid position
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            if (mousePosition == Vector3.zero) return; // If the cursor isn't in a valid position, don't place the object

            _grid.GetXZ(mousePosition, out int x, out int z);

            Vector2Int placedObjectOrigin = new Vector2Int(x, z);
            placedObjectOrigin = _grid.ValidateGridPosition(placedObjectOrigin);
            Vector2Int gridPosition = _placedObjectTypeSO.GetGridPosition(placedObjectOrigin, _dir);

            // Get the grid object at the coordinates we want to build
            GridObject gridObj = _grid.GetGridObject(gridPosition.x, gridPosition.y);
            // We can build if the grid object allows it and our collision check allows it
            bool canBuild = gridObj.CanBuild() && BuildingCollisionCheck.CanBuild;

            if (canBuild)
            {
                // Convert grid position into world position
                Vector2Int rotationOffset = _placedObjectTypeSO.GetRotationOffset(_dir);
                Vector3 placedObjectWorldPosition = _grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * _grid.GetCellSize();
                
                // Create our placed object in game
                PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, placedObjectOrigin, _dir, _placedObjectTypeSO);
                _grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                placedObject.transform.position = placedObject.transform.position + new Vector3(0f, _objectsToOffsets[_placedObjectTypeSO.name]);

                // Remove this machine from our inventory and update our toolbar UI
                PlayerInventory.Instance.TryRemoveFromInventory(_placedObjectTypeSO.name, 1f);
                if (PlayerInventory.Instance.ContainsItems(_placedObjectTypeSO.name, 1f).Item2 == 1f) // If we don't have any more of this machine deselect it
                    DeselectObjectType();
                MachinesInv.UpdateUI();
            }
            else
            {
                UtilsClass.CreateWorldTextPopup("Cannot build here!", mousePosition);
            }
        }

        // Delete a placed object
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Get the object at our mouse position
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            GridObject gridObject = _grid.GetGridObject(mousePosition);
            if (gridObject  != null)
            {
                PlacedObject placedObject = gridObject.GetPlacedObject();
                if (placedObject != null)
                {
                    // Try add this machine back to our inventory
                    PlayerInventory.Instance.TryDepositItem(placedObject.Name, 1f);
                    MachinesInv.UpdateUI();

                    // Destroy the object and clear it from our grid
                    placedObject.DestroySelf();
                    gridObject.ClearPlacedObject();
                }
            }

        }

        // Rotate the machine
        if (Input.GetKeyDown(KeyCode.R))
            _dir = PlacedObjectTypeSO.GetNextDir(_dir);
    }

    private void _selectMachine(int buttonIndex)
    {
        // Disable all confirm buttons
        foreach (var btn in _buildingButtonConfirms)
            btn.SetActive(false);

        bool isClearButton = buttonIndex == BuildingButtons.Count - 1;
        if (!isClearButton)
        {
            // If we're not clicking the clear button enable the confirm object and set this as our object to place
            _buildingButtonConfirms[buttonIndex].SetActive(true);
            _placedObjectTypeSO = _placedObjectTypeSOList[buttonIndex];
        }

        RefreshSelectedObjectType();
    }

    public void DeselectObjectType()
    {
        _placedObjectTypeSO = null;
        RefreshSelectedObjectType();
    }

    public void RefreshSelectedObjectType()
        => BuildingGhost.RefreshVisual();

    public Vector3 GetMouseWorldSnappedPosition()
    {
        Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
        _grid.GetXZ(mousePosition, out int x, out int z);

        if (_placedObjectTypeSO != null)
        {
            Vector2Int rotationOffset = _placedObjectTypeSO.GetRotationOffset(_dir);
            Vector3 placedObjectWorldPosition = _grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * _grid.GetCellSize();
            return placedObjectWorldPosition;
        }
        else
        {
            return mousePosition;
        }
    }

    public Quaternion GetPlacedObjectRotation()
        => _placedObjectTypeSO == null ? Quaternion.identity : Quaternion.Euler(0, _placedObjectTypeSO.GetRotationAngle(_dir), 0);

    public PlacedObjectTypeSO GetPlacedObjectTypeSO()
        => _placedObjectTypeSO;
}
