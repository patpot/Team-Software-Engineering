using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieAfterTime : MonoBehaviour
{
    public float Offset;
    private float _timer;
    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > Offset)
            Destroy(gameObject);
    }
}
