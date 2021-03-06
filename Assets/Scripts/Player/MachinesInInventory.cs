using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachinesInInventory : MonoBehaviour
{
    [SerializeField] private GameObject _crusherUI;
    [SerializeField] private GameObject _crystalliserUI;
    [SerializeField] private GameObject _synthesiserUI;
    [SerializeField] private GameObject _replicatorUI;
    [SerializeField] private GameObject _diffuserUI;
    [SerializeField] private GameObject _chestUI;
    
    public void UpdateUI()
    {
        _crusherUI.SetActive(false);
        _crystalliserUI.SetActive(false);
        _synthesiserUI.SetActive(false);
        _replicatorUI.SetActive(false);
        _diffuserUI.SetActive(false);
        _chestUI.SetActive(false);

        for (int i = 0; i < PlayerInventory.Instance.InventorySlotData.Count; i++)
        {
            if (PlayerInventory.Instance.InventorySlotData[i].ItemData == null) continue;
            string itemName = PlayerInventory.Instance.InventorySlotData[i].ItemData.Name;

            if (itemName == "Crusher") _crusherUI.SetActive(true);
            else if (itemName == "Crystalliser") _crystalliserUI.SetActive(true);
            else if (itemName == "Earthen Mana Synthesiser") _synthesiserUI.SetActive(true);
            else if (itemName == "Replicator") _replicatorUI.SetActive(true);
            else if (itemName == "Diffuser") _diffuserUI.SetActive(true);
            else if (itemName == "Wooden Chest") _chestUI.SetActive(true);
        }
    }
}
