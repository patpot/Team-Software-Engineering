using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCollisionCheck : MonoBehaviour
{
    [SerializeField] private LayerMask _layers;
    public static bool CanBuild;

    private void Awake()
    {
        CanBuild = true;
    }
    public bool SomethingInWay()
    {
        return Physics.CheckBox(transform.position, new Vector3(2.5f, 10f, 2.5f), Quaternion.identity, _layers);
    }

    private void Update()
    {
        CanBuild = !SomethingInWay();
    }
}
