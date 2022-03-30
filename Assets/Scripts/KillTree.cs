using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class KillTree : MonoBehaviour
{
    public Material GlowingFade;
    private float _fadeAmount = 1f;
    // Update is called once per frame

    void Start()
    {
        _fadeAmount = 1f;
        GlowingFade = GetComponent<MeshRenderer>().material;
        GlowingFade.SetFloat("_Alpha", _fadeAmount);
    }

    void Update()
    {
        _fadeAmount -= Time.deltaTime;
        GlowingFade.SetFloat("_Alpha", _fadeAmount);

        if (_fadeAmount < 0f)
        {
            GlowingFade.SetFloat("_Alpha", 1f);
            PlayerInventory.Instance.TryDepositItem(ItemManager.GetItemData("Wood Log"), 1f);
            Destroy(gameObject);
        }
    }
}
