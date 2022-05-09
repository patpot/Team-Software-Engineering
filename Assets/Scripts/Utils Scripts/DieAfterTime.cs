using UnityEngine;
public class DieAfterTime : MonoBehaviour 
{
    public float Offset;
    void Update() => Destroy(gameObject, Offset);
}