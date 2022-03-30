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
    private GameObject _previewUi;

    public List<Sprite> InputIcons;
    public List<Sprite> OutputIcons;

    public float TimeToProduce;
    public string MachineName;

    public void SetValues(string machineName, float timeToProduce, List<Sprite> inputIcons, List<Sprite> outputIcons)
    {
        InputIcons = inputIcons;
        OutputIcons = outputIcons;
        TimeToProduce = timeToProduce;
        MachineName = machineName;

        if (this.GetComponent<MeshCollider>() == null)
            gameObject.AddComponent<MeshCollider>();
    }

    public void OnMouseDown()
    {
        if (SpellbookToggle.SpellbookActive) return;
        if (UIManager.UIActive) return;
        UIManager.UIActive = true;
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
            if (i < InputIcons.Count)
            {
                inputIcon.SetActive(true);
                Image icon = inputIcon.GetComponentsInChildren<Image>()[1];
                icon.sprite = InputIcons[i];
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
            if (i < OutputIcons.Count)
            {
                outputIcon.SetActive(true);
                Image icon = outputIcon.GetComponentsInChildren<Image>()[1];
                icon.sprite = OutputIcons[i];
            }
            else // If not, don't render it
                outputIcon.SetActive(false);

        }

        // Set position to be the mouse position and tween up a bit
        _previewUi.transform.position = Input.mousePosition;
        _previewUi.transform.DOMoveY(Input.mousePosition.y + 40f, 0.4f);

        // Create a tween fading in all the images
        Utils.FadeInUI(_previewUi);
    }

    public void OnMouseExit()
    {
        if (_previewUi != null)
            UIManager.UIActive = false;
        else return; // UI Manager wasn't rendering our UI, move on
        // Make sure we dispose of all of our tweens
        foreach (var image in _previewUi.GetComponentsInChildren<Image>())
            image.DOKill();
        _previewUi.transform.DOKill();
        // Then destroy the object
        Destroy(_previewUi);
    }
}
