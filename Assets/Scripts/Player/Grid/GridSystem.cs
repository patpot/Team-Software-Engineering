using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridSystem<TGridObject>
{
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int X;
        public int Z;
    }

    private int _width;
    private int _height;
    private float _cellSize;
    private Vector3 _originPosition;
    private TGridObject[,] _gridArray;

    public GridSystem(int width, int height, float cellSize, Vector3 originPosition, Func<GridSystem<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this._width = width;
        this._height = height;
        this._cellSize = cellSize;
        this._originPosition = originPosition;

        _gridArray = new TGridObject[width, height];

        for (int x = 0; x < _gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < _gridArray.GetLength(1); z++)
            {
                _gridArray[x, z] = createGridObject(this, x, z);
            }
        }

        bool showDebug = false;
        if (showDebug)
        {
            TextMesh[,] debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int z = 0; z < _gridArray.GetLength(1); z++)
                {
                    debugTextArray[x, z] = UtilsClass.CreateWorldText(_gridArray[x, z]?.ToString(), null, GetWorldPosition(x, z) + new Vector3(cellSize, 0, cellSize) * .5f, 15, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => {
                debugTextArray[eventArgs.X, eventArgs.Z].text = _gridArray[eventArgs.X, eventArgs.Z]?.ToString();
            };
        }
    }

    public int GetWidth()
        => _width;

    public int GetHeight()
        => _height;

    public float GetCellSize()
        => _cellSize;

    public Vector3 GetWorldPosition(int x, int z)
        => new Vector3(x, 0, z) * _cellSize + _originPosition;

    public void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt((worldPosition - _originPosition).x / _cellSize);
        z = Mathf.FloorToInt((worldPosition - _originPosition).z / _cellSize);
    }

    public void SetGridObject(int x, int z, TGridObject value)
    {
        if (x >= 0 && z >= 0 && x < _width && z < _height)
        {
            _gridArray[x, z] = value;
            TriggerGridObjectChanged(x, z);
        }
    }

    public void TriggerGridObjectChanged(int x, int z)
        => OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { X = x, Z = z });

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        GetXZ(worldPosition, out int x, out int z);
        SetGridObject(x, z, value);
    }

    public TGridObject GetGridObject(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < _width && z < _height)
            return _gridArray[x, z];
        else
            return default(TGridObject);
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        return GetGridObject(x, z);
    }

    public Vector2Int ValidateGridPosition(Vector2Int gridPosition)
    {
        return new Vector2Int(
            Mathf.Clamp(gridPosition.x, 0, _width - 1),
            Mathf.Clamp(gridPosition.y, 0, _height - 1)
        );
    }
}
