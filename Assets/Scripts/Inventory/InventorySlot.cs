using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private Inventory _inventory;
    private InventorySlotData _slotData;
    private Image _icon;
    private TextMeshProUGUI _itemCountDisplay;
    
    public void Awake()
    {
        _icon = GetComponentsInChildren<Image>()[1]; // [0] is background, [1] is icon
        _itemCountDisplay = GetComponentInChildren<TextMeshProUGUI>();
    }

    private GameObject _draggableObj;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        // Start dragging, highlight this object
        this.GetComponentInChildren<Image>().color = Color.yellow;

        // And spawn in our draggable preview
        _draggableObj = UIManager.CreatePrefab("DraggableInventorySlot");
        _draggableObj.transform.SetParent(transform.parent.parent);
        _draggableObj.GetComponentsInChildren<Image>()[1].sprite = _icon.sprite;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        this.GetComponentInChildren<Image>().color = Color.white;

        if (_draggableObj != null)
        {
            // Create fake pointerdata with our draggable object's end position as the position
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = _draggableObj.transform.position;

            // Raycast on the end position of our drag and iterate through the results to find another inventory slot
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            foreach (var result in results)
            {
                var invSlot = result.gameObject.GetComponent<InventorySlot>();
                if (invSlot != null && invSlot != this)
                {
                    // We ended our drag over another inventory slot, swap our data!
                    InventorySlotData tempData = this._slotData;
                    this.SetSlotData(invSlot.GetSlotData());
                    invSlot.SetSlotData(tempData);
                    // Now that we've swapped data, force a UI refresh
                    this.UpdateSlotUI();
                    invSlot.UpdateSlotUI();
                    // Finally, update the main Inventory script's list to be in the new order
                    _inventory.SwapInventorySlotData(this.GetSlotData(), invSlot.GetSlotData());
                }
            }
            // Destroy our temporary draggable preview
            Destroy(_draggableObj);
        }
    }
    public void SetInventory(Inventory inventory)
        => _inventory = inventory;
    public void SetSlotData(InventorySlotData slotData)
    {
        _slotData = slotData;
        _slotData.InventorySlot = this;
    }

    public InventorySlotData GetSlotData()
        => _slotData;

    public void UpdateSlotUI()
    {
        // Figure out what our icon and item count should display
        Sprite icon = UIManager.Instance.BlankInventorySprite;
        // If we contain an item, render the icon for that, if not use the blank default sprite
        if (_slotData.ContainsItem)
            icon = _slotData.ItemData.Icon;

        string itemCount = "";
        if (_slotData.ItemCount > 0)
            itemCount = _slotData.ItemCount.ToString();

        _icon.sprite = icon;
        _itemCountDisplay.text = itemCount;
    }
}
