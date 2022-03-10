using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot
{
    public ItemData ItemData;
    private float _itemCount;
    public float ItemCount
    {
        get { return _itemCount; }
        set
        {
            _itemCount = value;
            UpdateSlotUI();
        }
    }
    public GameObject InventorySlotObject;

    public void UpdateSlotUI()
    {
        if (InventorySlotObject == null) return; // UI isn't being rendered, get out before we cause errors

        Sprite icon = UIManager.Instance.BlankInventorySprite;
        if (ItemData != null)
            icon = ItemData.Icon;
        string itemCount = "";
        if (ItemCount > 0)
            itemCount = ItemCount.ToString();

        InventorySlotObject.GetComponentsInChildren<Image>()[1].sprite = icon; // [0] is background, [1] is icon
        InventorySlotObject.GetComponentInChildren<TextMeshProUGUI>().text = itemCount;
    }
}