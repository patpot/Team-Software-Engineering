using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOverTime : MonoBehaviour
{
    public Vector3 Speed = Vector3.right;

    private void Update()
    {
        transform.position += Speed * Time.deltaTime;
    }
}
