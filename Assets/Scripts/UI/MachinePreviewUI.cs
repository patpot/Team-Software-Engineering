using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts;

public class MachinePreviewUI : MonoBehaviour
{
    // Our required inputs and outputs for this machine to function. If no inputs are required the machine will constantly be producing outputs
    public List<(ItemData, float)> Inputs = new List<(ItemData, float)>();
    public List<(ItemData, float)> Outputs = new List<(ItemData, float)>();

    public float TimeToProduce;
    public string MachineName;

    public Inventory InputInventory;
    public Inventory OutputInventory;

    private GameObject _previewUi;

    public void SetValues(string machineName, float timeToProduce, Dictionary<ItemData, float> inputs, Dictionary<ItemData, float> outputs, Inventory inputInventory, Inventory outputInventory)
    {
        foreach (var input in inputs)
            Inputs.Add((input.Key, input.Value));

        foreach (var output in outputs)
            Outputs.Add((output.Key, output.Value));
        
        TimeToProduce = timeToProduce;
        MachineName = machineName;
        InputInventory = inputInventory;
        OutputInventory = outputInventory;

        if (this.GetComponent<MeshCollider>() == null)
            gameObject.AddComponent<MeshCollider>();
    }

    public void OnMouseDown()
    {
        if (SpellbookToggle.SpellbookActive) return;
        if (UIManager.ActiveUICount > 0) return;
        if (Vector3.Distance(transform.position, Camera.main.transform.position) > 5f) return;

        if (_previewUi == null && !CameraSwitcher.BuildMode)
        {
            UIManager.ActiveUICount++;
            UIManager.MachineUIActive = true;
            UIManager.UpdateCameraAndCursor();
            
            // We construct our UI when we hover over the object and slightly fade it in for a bit more juice
            _previewUi = UIManager.CreatePrefab("MachinePreviewUI");
            // Assign this object to the canvas
            _previewUi.transform.SetParent(UIManager.Instance.MainCanvas, false);

            // Set the machine name text
            _previewUi.GetComponentInChildren<TextMeshProUGUI>().text = MachineName;
            // Set the time to produce text
            _previewUi.transform.Find("Clock").GetComponentInChildren<TextMeshProUGUI>().text = TimeToProduce.ToString() + "s";
            // Set input icons
            var inputIconParent = _previewUi.GetComponentInChildren<GridLayoutGroup>().transform;
            for (int i = 0; i < inputIconParent.childCount; i++)
            {
                GameObject inputIcon = inputIconParent.transform.GetChild(i).gameObject;
                // If we have an icon, set the icon to match and render it
                if (i < Inputs.Count)
                {
                    inputIcon.SetActive(true);
                    Image icon = inputIcon.GetComponentsInChildren<Image>()[1];
                    icon.sprite = Inputs[i].Item1.Icon;
                    TextMeshProUGUI quantity = inputIcon.GetComponentInChildren<TextMeshProUGUI>();
                    quantity.text = Inputs[i].Item2.ToString();
                }
                else // If not, don't render it
                    inputIcon.SetActive(false);
            }

            // Set output icons
            var outputIconParent = _previewUi.GetComponentsInChildren<GridLayoutGroup>()[1].transform;
            for (int i = 0; i < outputIconParent.childCount; i++)
            {
                GameObject outputIcon = outputIconParent.transform.GetChild(i).gameObject;
                // If we have an icon, set the icon to match and render it
                if (i < Outputs.Count)
                {
                    outputIcon.SetActive(true);
                    Image icon = outputIcon.GetComponentsInChildren<Image>()[1];
                    icon.sprite = Outputs[i].Item1.Icon;
                    TextMeshProUGUI quantity = outputIcon.GetComponentInChildren<TextMeshProUGUI>();
                    quantity.text = Outputs[i].Item2.ToString();
                }
                else // If not, don't render it
                    outputIcon.SetActive(false);
            }

            // Set input inventory icons
            var inputInventoryParent = _previewUi.GetComponentsInChildren<GridLayoutGroup>()[2].transform;
            for (int i = 0; i < inputInventoryParent.childCount; i++)
            {
                GameObject inputSlot = inputInventoryParent.transform.GetChild(i).gameObject;
                if (i > InputInventory.SlotCount - 1)
                {
                    inputSlot.SetActive(false);
                    continue;
                }
                inputSlot.SetActive(true);
                var invSlot = inputSlot.GetComponent<InventorySlot>();
                // Store our slot data and update our UI
                invSlot.SetInventory(InputInventory);
                invSlot.SetSlotData(InputInventory.InventorySlotData[i]);
                invSlot.UpdateSlotUI();
            }

            // Set output inventory icons
            var outputInventoryParent = _previewUi.GetComponentsInChildren<GridLayoutGroup>()[3].transform;
            for (int i = 0; i < outputInventoryParent.childCount; i++)
            {
                GameObject outputSlot = outputInventoryParent.transform.GetChild(i).gameObject;
                if (i > OutputInventory.SlotCount - 1)
                {
                    outputSlot.SetActive(false);
                    continue;
                }
                outputSlot.SetActive(true);
                var invSlot = outputSlot.GetComponent<InventorySlot>();
                // Store our slot data and update our UI
                invSlot.SetInventory(OutputInventory);
                invSlot.SetSlotData(OutputInventory.InventorySlotData[i]);
                invSlot.UpdateSlotUI();
            }

            // Set position to be the mouse position and tween up a bit
            _previewUi.transform.position = Input.mousePosition;
            _previewUi.transform.DOMoveY(Input.mousePosition.y + 40f, 0.4f);

            // Create a tween fading in all the images
            Utils.FadeInUI(_previewUi);

            // Open player's fake inventory
            PlayerInventory.Instance.OpenFakeInventory();
        }
    }

    public void LateUpdate()
    {
        // TEMP but closing UI is hard.
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_previewUi != null)
            {
                UIManager.ActiveUICount--;
                UIManager.MachineUIActive = false;
                UIManager.UpdateCameraAndCursor();

                // Make sure we dispose of all of our tweens
                foreach (var image in _previewUi.GetComponentsInChildren<Image>())
                    image.DOKill();
                _previewUi.transform.DOKill();
                // Then destroy the object
                Destroy(_previewUi);
                // Close player's fake inventory
                UIManager.Instance.FakePlayerInventory.SetActive(false);
            }
        }
    }
}
