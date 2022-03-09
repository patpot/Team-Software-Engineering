using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobbing : MonoBehaviour
{
    //adjust this to change speed
    [SerializeField]
    float _speed = 2.5f;
    //adjust this to change how high it goes
    [SerializeField]
    float _height = 0.75f;

    private float _startY;
    private void Start()
    {
        _startY = transform.localPosition.y;
    }

    void Update()
    {
        //get the objects current position and put it in a variable so we can access it later with less code
        Vector3 pos = transform.localPosition;
        //calculate what the new Y position will be
        float newY = Mathf.Sin(Time.time * _speed) * _height;
        //set the object's Y to the new calculated Y
        transform.localPosition = new Vector3(pos.x, (_startY + newY), pos.z);
    }
}
