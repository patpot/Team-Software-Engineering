using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableInventorySlot : MonoBehaviour, IPointerDownHandler
{
    public InventorySlotData SlotData;
    public void Update()
    {
        transform.position = Input.mousePosition;
        if (SlotData.ItemCount == 0)
            Destroy(gameObject);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        SlotData.InventorySlot.TryDropItem(eventData);
    }
}