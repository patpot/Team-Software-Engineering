using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseMachine : MonoBehaviour
{
    void Start()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit);
        if (hit.transform == null)
            return;

        float yHeight = transform.position.y - hit.transform.position.y; // GroundPosition - mid point of object gets us half height, all objects pivot on 0,0,0 so we add this on
        transform.position = hit.point + new Vector3(0f, yHeight);
    }
}
