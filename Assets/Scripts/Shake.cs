using System.Collections;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public bool Start = false;
    public AnimationCurve Curve;
    public float Duration = 1f;

    void Update()
    {
        if (Start)
        {
            Start = false;
            StartCoroutine(Shaking());
        }
    }

    IEnumerator Shaking()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < Duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = Curve.Evaluate(elapsedTime / Duration);
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPosition;
    }
}