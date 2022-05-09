using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MachineConnectionFader : MonoBehaviour
{
    public Material ConnectorMaterial;
    private float _alphaCountdown;
    private const float FADE_TIME = 3f;

    public void ResetAlpha()
    {
        _alphaCountdown = float.NegativeInfinity;
        ConnectorMaterial.SetFloat("_Alpha", 1f);
    }
    public void StartFade()
        => _alphaCountdown = FADE_TIME;

    public void Update()
    {
        if (_alphaCountdown >= 0f)
        {
            _alphaCountdown -= Time.deltaTime;
            ConnectorMaterial.SetFloat("_Alpha", _alphaCountdown);
        }
    }
}