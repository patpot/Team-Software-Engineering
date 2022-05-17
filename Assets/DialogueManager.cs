using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using DG.Tweening;
using Assets.Scripts;

public class DialogueManager : MonoBehaviour
{
    public GameObject DialogueObj;
    public TextMeshProUGUI TextArea;
    public Button NextButton;

    private bool _skipDialogue;
    private bool _fadingIn;
    //private bool _displayingText;
    private string _currentDialogue;
    private int _dialogueIndex = -1;
    private List<string> _dialogue = new List<string>();
    private Dictionary<char, int> _punctuationDelay = new Dictionary<char, int>()
    {
        {'.', 350 },
        {'?', 450 },
        {'!', 300 },
        {'\"', 200 },
        {',', 250 },
    };

    public void Start()
    {
        // Dialogue skip override for testing
        _skipDialogue = true;
        if (_skipDialogue)
        {
            UIManager.UnlockCamera();
            UIManager.LockCursor();
        }
        NextButton.onClick.AddListener(() => DisplayText(0));

        AddDialogue("Hello! Are you the new recruit? We have lots to discuss about your first assignment here.");
        AddDialogue("I'll be going through the basics with you to catch you up to speed, if you ever forget something I've said you can press \"T\" to open this dialogue box back up and press \"Q\" to go back through old dialogue.");
        AddDialogue("As you know the Aura Restoration Council are in charge of the maintenance and restoration of areas with particularly low magical aura. Restoring these areas up to their full potential is a vital part of the council's work, and if areas of low magical aura were to exist for too long things would become.. problematic.");
        AddDialogue("As its just you we're going to need to build some additional infrastructure to support the restoration of this zone. We've got a set of machines that'll help you out, but you'll need to gather some resources to build them.");
        AddDialogue("Now, as part of the standard issue kit you have a Spellbook to help you channel your own aura, pressing \"1\" will equip it. Give it a go! You'll be using it a lot.");
        ShowDialogue(800);
    }
    public void FadeInDialogue(int startOffset)
    {
        if (_skipDialogue) return;
        
        UIManager.UIActive = true;
        UIManager.LockCamera();
        UIManager.UnlockCursor();

        DialogueObj.SetActive(true);
        DialogueObj.GetComponent<CanvasGroup>().alpha = 0f;
        DialogueObj.GetComponent<CanvasGroup>().DOFade(1f, startOffset / 1000f); // seconds to ms
        _fadingIn = true;
    }

    public void AddDialogue(string dialogue)
    {
        _dialogue.Add(dialogue);
    }

    public void ShowDialogue(int startOffset = 800, DialogueTransition transitionType = DialogueTransition.Forward)
    {
        if (_skipDialogue) return;
        
        FadeInDialogue(startOffset);
        DisplayText(startOffset, transitionType);
    }
    
    public async Task DisplayText(int startOffset = 800, DialogueTransition transitionType = DialogueTransition.Forward)
    {
        string dialogue;
        // Check what we want to be displaying
        if (transitionType == DialogueTransition.Forward) _dialogueIndex = Math.Min(_dialogue.Count - 1, ++_dialogueIndex);
        else if (transitionType == DialogueTransition.Backward) _dialogueIndex = Math.Max(0, --_dialogueIndex);
        else if (transitionType == DialogueTransition.OpenDialogue) return;

        dialogue = _dialogue[_dialogueIndex];
        // Decide what to do if this dialogue is the same as the last one shown
        if (dialogue == _currentDialogue)
        {
            if (transitionType == DialogueTransition.Forward)
            {
                UIManager.UIActive = false;
                // If we were in first person go back to that
                if (!(UIManager.Instance.InventoryUI.activeSelf || CraftingManager.Active) && !CameraSwitcher.BuildMode)
                {
                    UIManager.UnlockCamera();
                    UIManager.LockCursor();
                } 
                
                DialogueObj.GetComponent<CanvasGroup>().DOFade(0f, 0.8f);
                await Task.Delay(800);
                DialogueObj.SetActive(_fadingIn);
                return;
            }
            else
                return;
        }
        
        // Clear the text field before we do any amount of waiting in case some text is leftover
        TextArea.text = "";
        _currentDialogue = dialogue;
        await Task.Delay(startOffset);
        _fadingIn = false;

        // Slowly fill up the text field
        char[] characters = dialogue.ToCharArray();
        foreach (var captionChar in characters)
        {
            if (dialogue != _currentDialogue) return;
            TextArea.text += captionChar;
            await Task.Delay(_punctuationDelay.ContainsKey(captionChar) ? _punctuationDelay[captionChar] : 20);
        }
    }

    public void Update()
    {
        // Show the last piece of dialogue
        if (Input.GetKeyDown(KeyCode.Q) && DialogueObj.activeSelf)
            ShowDialogue(0, DialogueTransition.Backward);

        // Opens the box back up
        if (Input.GetKeyDown(KeyCode.T))
            ShowDialogue(0, DialogueTransition.OpenDialogue);
    }

    public enum DialogueTransition
    {
        Forward,
        Backward,
        OpenDialogue,
    }
}
