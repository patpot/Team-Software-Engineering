using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MagicFade : MonoBehaviour
{
    public Material GlowingFade;
    public string ItemName;
    private float _fadeAmount = 1f;

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
            PlayerInventory.Instance.TryDepositItem(ItemManager.GetItemData(ItemName), 1f);
            Destroy(gameObject);
        }
    }
}
