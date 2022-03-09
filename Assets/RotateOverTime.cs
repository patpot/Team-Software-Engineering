using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
    [SerializeField]
    public Vector3 RotateSpeed = Vector3.zero;

    public void Update()
    {
        transform.Rotate(RotateSpeed.x * Time.deltaTime, RotateSpeed.y * Time.deltaTime, RotateSpeed.z * Time.deltaTime);
    }
}
