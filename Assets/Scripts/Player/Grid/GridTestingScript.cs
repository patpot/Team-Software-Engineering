using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridTestingScript : MonoBehaviour
{
    private GridSystem<bool> _grid;

    private void Start()
    {
        //_grid = new GridSystem<bool>(4, 2, 10f, new Vector3 (0,0));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //_grid.SetValue(UtilsClass.GetMouseWorldPosition(), true);
        }
        if(Input.GetMouseButtonDown(1))
        {
            //Debug.Log(_grid.GetValue(UtilsClass.GetMouseWorldPosition()));
        }
    }
}
