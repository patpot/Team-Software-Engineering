using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridBuildingSystem : MonoBehaviour
{
    //[SerializeField] private LayerMask otherLayers = new LayerMask();

public static GridBuildingSystem Instance { get; private set; }

    public event EventHandler OnSelectedChanged;
    //public event EventHandler OnObjectPlaced;


    [SerializeField] private List<PlacedObjectTypeSO> _placedObjectTypeSOList;
    private PlacedObjectTypeSO _placedObjectTypeSO;

    private GridSystem<GridObject> _grid;
    private PlacedObjectTypeSO.Dir _dir = PlacedObjectTypeSO.Dir.Down;

    private void Awake()
    {
        Instance = this;

        int gridWidth = 15;
        int gridHeight = 25;
        float cellSize = 5f;
        _grid = new GridSystem<GridObject>(gridWidth, gridHeight, cellSize, new Vector3(-25,0,-60), (GridSystem<GridObject> g, int x, int z) => new GridObject(g, x, z));

        _placedObjectTypeSO = _placedObjectTypeSOList[0];
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
        {
            return _placedObject;
        }

        public void ClearPlacedObject()
        {
            _placedObject = null;
            _grid.TriggerGridObjectChanged(_x, _z);
        }

        public bool CanBuild()
        {
            return _placedObject == null;
        }

        public override string ToString()
        {
            return _x + ", " + _z + "\n" + _placedObject;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _placedObjectTypeSO != null)
        {
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            _grid.GetXZ(Mouse3D.GetMouseWorldPosition(), out int x, out int z);

            Vector2Int placedObjectOrigin = new Vector2Int(x, z);
            placedObjectOrigin = _grid.ValidateGridPosition(placedObjectOrigin);

            List<Vector2Int> gridPositionList =  _placedObjectTypeSO.GetGridPositionList(placedObjectOrigin, _dir);
            
            bool canBuild = true;
            foreach(Vector2Int gridPosition in gridPositionList)
            {
                if (_grid.GetGridObject(gridPosition.x, gridPosition.y) != null)
                {
                    if (!_grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
                    {
                        canBuild = false;
                        break;
                    }
                }
                else
                {

                    canBuild = false;
                    break;
                }
            }

            if (canBuild)
            {
                Vector2Int rotationOffset = _placedObjectTypeSO.GetRotationOffset(_dir);
                Vector3 placedObjectWorldPosition = _grid.GetWorldPosition(placedObjectOrigin.x, placedObjectOrigin.y) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * _grid.GetCellSize();

                PlacedObject placedObject =  PlacedObject.Create(placedObjectWorldPosition, placedObjectOrigin, _dir, _placedObjectTypeSO);

                foreach(Vector2Int gridPosition in gridPositionList)
                {
                    _grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                }
            }
            else
            {
                UtilsClass.CreateWorldTextPopup("Cannot build here!", mousePosition);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            if (_grid.GetGridObject(mousePosition) != null)
            {
                GridObject gridObject = _grid.GetGridObject(Mouse3D.GetMouseWorldPosition());
                PlacedObject placedObject = gridObject.GetPlacedObject();
                if (placedObject != null)
                {
                    placedObject.DestroySelf();

                    List<Vector2Int> gridPositionList = placedObject.GetGridPositionList();
                    foreach (Vector2Int gridPosition in gridPositionList)
                    {
                        _grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                    }
                }
            }
            
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            _dir = PlacedObjectTypeSO.GetNextDir(_dir);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) { _placedObjectTypeSO = _placedObjectTypeSOList[0]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { _placedObjectTypeSO = _placedObjectTypeSOList[1]; RefreshSelectedObjectType(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { _placedObjectTypeSO = _placedObjectTypeSOList[2]; RefreshSelectedObjectType(); }

        if (Input.GetKeyDown(KeyCode.Alpha0)) { DeselectObjectType(); }
    }

    private void DeselectObjectType()
    {
        _placedObjectTypeSO = null; RefreshSelectedObjectType();
    }

    private void RefreshSelectedObjectType()
    {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }


    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        _grid.GetXZ(worldPosition, out int x, out int z);
        return new Vector2Int(x, z);
    }

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
    {
        if (_placedObjectTypeSO != null)
        {
            return Quaternion.Euler(0, _placedObjectTypeSO.GetRotationAngle(_dir), 0);
        }
        else
        {
            return Quaternion.identity;
        }
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO()
    {
        return _placedObjectTypeSO;
    }
}
