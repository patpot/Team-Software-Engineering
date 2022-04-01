using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastUpwards : MonoBehaviour
{
    [SerializeField] private LayerMask layers;

    public bool SomethingInWay()
    {
        return Physics.CheckBox(transform.position, new Vector3(5f, 500f, 5f), Quaternion.identity, layers);
    }

    private void Update()
    {
        if (SomethingInWay())
        {
            Debug.Log("Something in way");
        }
        if (!SomethingInWay())
        {
            Debug.Log("Nothing in way");
        }
    }
}
