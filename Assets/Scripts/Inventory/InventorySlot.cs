using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.Items;
using Unity.Mathematics;

public class InventorySlot : MonoBehaviour, IPointerDownHandler
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

    public GameObject DraggableObj;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left &&
            eventData.button != PointerEventData.InputButton.Right) return;

        if (_slotData.ItemCount == 0) return;

        // First click creates the draggable object
        if (DraggableObj == null)
        {
            // Start dragging, highlight this object
            this.GetComponentInChildren<Image>().color = Color.yellow;

            // And spawn in our draggable preview
            DraggableObj = UIManager.CreatePrefab("DraggableInventorySlot");
            DraggableObj.GetComponent<DraggableInventorySlot>().SlotData = _slotData;
            DraggableObj.transform.SetParent(transform.parent.parent.parent); // Parent to the canvas
            DraggableObj.GetComponentsInChildren<Image>()[1].sprite = _icon.sprite;
        }
        // Second click is handled on the draggable object, and calls TryDropITEM
    }

    public void TryDropItem(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left &&
            eventData.button != PointerEventData.InputButton.Right) return;

        // Create fake pointerdata with our draggable object's end position as the position
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = DraggableObj.transform.position;

        // Raycast on the end position of our drag and iterate through the results to find another inventory slot
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        bool dropObject = true;
        foreach (var result in results)
        {
            var targetInvSlot = result.gameObject.GetComponent<InventorySlot>();
            if (targetInvSlot != null && targetInvSlot != this)
            {
                // We ended our drag over another inventory slot, figure out what to do next
                // Assign local variables to ensure all calls are O(1) and have logical names
                dropObject = false;
                InventorySlotData targetInvSlotData = targetInvSlot.GetSlotData();
                Inventory targetInventory = targetInvSlot.GetInventory();
                float itemCountTotal;
                float amtToDeposit;
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    amtToDeposit = _slotData.ItemCount;
                    itemCountTotal = _slotData.ItemCount + targetInvSlotData.ItemCount;
                }
                else
                {
                    amtToDeposit = 1f;
                    itemCountTotal = 1 + targetInvSlotData.ItemCount;
                }
                float targetInventorySlotSize = targetInventory.SlotSize;

                if (_slotData.ItemData == targetInvSlotData.ItemData)
                {
                    // Our items match, let's try and add them together!
                    if (itemCountTotal <= targetInventorySlotSize)
                    {
                        // We can take all the items, so take them all and nullify the slot we just took from
                        targetInvSlotData.ItemCount += amtToDeposit;
                        _slotData.ItemCount -= amtToDeposit;
                    }
                    else if (itemCountTotal > targetInventorySlotSize && targetInvSlotData.ItemCount != targetInventorySlotSize)
                    {
                        // Too many items, fill in however many we can and then update both slots
                        float diff = targetInventorySlotSize - targetInvSlotData.ItemCount;
                        targetInvSlotData.ItemCount += diff;
                        _slotData.ItemCount -= diff;
                    }
                    else if (targetInvSlotData.ItemCount == targetInventorySlotSize && _slotData.ItemCount <= targetInventorySlotSize
                        && eventData.button == PointerEventData.InputButton.Left)
                    {
                        // One of the slots is full, but not both. Just swap their data if we're left clicking.

                        // Before we override any values make sure we change our positions in our respective inventories first
                        _inventory.SwapInventorySlotData(_slotData, targetInvSlotData);
                        // Swap our actually slot data
                        InventorySlotData tempData = this._slotData;
                        this.SetSlotData(targetInvSlotData);
                        targetInvSlot.SetSlotData(tempData);

                        this.GetComponentInChildren<Image>().color = Color.white;
                        Destroy(DraggableObj);
                    }
                }
                else
                {
                    // ItemData didn't match, try swap the items
                    if (_slotData.ItemCount >= targetInventorySlotSize && targetInvSlot._slotData.ItemData == null && eventData.button == PointerEventData.InputButton.Left)
                    {
                        // Our stack is larger than this slot can handle, if it was empty we can override its data

                        targetInvSlotData.ItemData = _slotData.ItemData;
                        targetInvSlotData.ItemCount = targetInventorySlotSize;
                        _slotData.ItemCount -= targetInventorySlotSize;
                    }
                    else if (_slotData.ItemCount <= targetInventorySlotSize && targetInvSlotData.ItemCount <= _inventory.SlotSize)
                    {
                        // This slot contains items and we can validly put all of our items into it IF we swap, ensure we are left clicking then swap
                        if (eventData.button == PointerEventData.InputButton.Left)
                        {
                            // Before we override any values make sure we change our positions in our respective inventories first
                            _inventory.SwapInventorySlotData(_slotData, targetInvSlotData);
                            // Swap our actually slot data
                            InventorySlotData tempData = this._slotData;
                            this.SetSlotData(targetInvSlotData);
                            targetInvSlot.SetSlotData(tempData);

                            // If the new slot is now locked (we switched states entirely) we need to update its itemdata to be the locked item
                            if (targetInvSlotData.Locked)
                                targetInvSlotData.ItemData = tempData.ItemData;

                            // As there was a full switch we will destroy our draggable object
                            this.GetComponentInChildren<Image>().color = Color.white;
                            Destroy(DraggableObj);
                        }
                        else if (targetInvSlot._slotData.ItemData == null && eventData.button == PointerEventData.InputButton.Right)
                        {
                            // We can't swap, but we can still put our items into the target slot
                            targetInvSlotData.ItemData = _slotData.ItemData;
                            targetInvSlotData.ItemCount += amtToDeposit;
                            _slotData.ItemCount-= amtToDeposit;
                        }
                    }
                }
                // Finally force a UI refresh
                this.UpdateSlotUI();
                targetInvSlot.UpdateSlotUI();
                break;
            }
            else if (targetInvSlot != null)
            {
                // We hit our own inventory slot, stop the drag
                this.GetComponentInChildren<Image>().color = Color.white;
                Destroy(DraggableObj);
                dropObject = false; 
            }
        }
        // Destroy our temporary draggable preview if we put down all our items
        if (_slotData.ItemCount == 0)
        {
            this.GetComponentInChildren<Image>().color = Color.white;
            Destroy(DraggableObj);
        }

        if (dropObject)
        {
            if (_slotData.ItemData?.Model == null) return; // Item doesn't have a model so we can't place it in world

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 6))
            {
                if (hit.point.y > 1) return; // Don't want anything being put too high up

                GameObject droppedObject = Instantiate(_slotData.ItemData.Model);
                droppedObject.name = _slotData.ItemData.Name;
                float yHeight = hit.point.y - hit.transform.position.y; // GroundPosition - mid point of object gets us half height, all objects pivot on 0,0,0 so we add this on
                droppedObject.transform.position = hit.point + new Vector3(0f, yHeight);

                // Drop the item in the world
                var itemStack = droppedObject.AddComponent<ItemStack>();
                itemStack.ItemData = _slotData.ItemData;
                itemStack.ItemCount = 1;

                _slotData.ItemCount--;
                this.UpdateSlotUI();
            }
        }
    }
    public void SetInventory(Inventory inventory)
        => _inventory = inventory;
    public Inventory GetInventory()
        => _inventory;
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
