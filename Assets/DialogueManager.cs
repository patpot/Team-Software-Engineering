using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using DG.Tweening;
using Assets.Scripts;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    public GameObject DialogueObj;
    private CanvasGroup _dialogueCG;
    public TextMeshProUGUI TextArea;
    public Button NextButton;

    private bool _instantLoadDialogueNextTick;
    private bool _skipDialogue;
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
        _skipDialogue = false;
        NextButton.onClick.AddListener(() => {
            DisplayText(0);
            EventSystem.current.SetSelectedGameObject(null);
        });

        AddDialogue("Hello! Are you the new recruit? We have lots to discuss about your first assignment here.");
        AddDialogue("I'll be going through the basics with you to catch you up to speed, if you ever forget something I've said you can press \"T\" to open this dialogue box back up and press \"Q\" to go back through old dialogue, if you find I'm speaking too slowly you can press \"Mouse1\" anywhere on the screen to instantly show my dialogue. You can also view your current task in the top left corner.");
        AddDialogue("As you know the Aura Restoration Council are in charge of the maintenance and restoration of areas with particularly low magical aura. Restoring these areas up to their full potential is a vital part of the council's work, and if areas of low magical aura were to exist for too long things would become.. problematic.");
        AddDialogue("As it's just you we're going to need to build some additional infrastructure to support the restoration of this zone. We've got a set of machines that'll help you out, but you'll need to gather some resources to build them.");
        AddDialogue("Now, as part of the standard issue kit you have a Spellbook to help you channel your own aura, pressing \"1\" will equip it. Give it a go! You'll be using it a lot.");
        _dialogueCG = DialogueObj.GetComponent<CanvasGroup>();
    }
    private Tween _fadeTween;
    public void FadeInDialogue(int startOffset)
    {
        if (_skipDialogue) return;
        
        UIManager.ActiveUICount++;
        UIManager.UpdateCameraAndCursor();

        _fadeTween.Kill();
        _fadeTween = _dialogueCG.DOFade(1f, startOffset / 1000f); // seconds to ms
    }

    public void AddDialogue(string dialogue)
    {
        _dialogue.Add(dialogue);
    }

    public void ShowDialogue(int startOffset = 800, DialogueTransition transitionType = DialogueTransition.Forward)
    {
        if (UIManager.Instance.MainMenuUI.activeSelf || _skipDialogue) return;

        if (_dialogueCG.alpha < 1f)
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
            if (transitionType == DialogueTransition.Forward && _dialogueCG.alpha == 1f)
            {
                UIManager.ActiveUICount--;
                UIManager.UpdateCameraAndCursor();
                _fadeTween = _dialogueCG.DOFade(0f, 0.8f);
                return;
            }
            else
                return;
        }
        
        // Clear the text field before we do any amount of waiting in case some text is leftover
        TextArea.text = "";
        _currentDialogue = dialogue;
        await Task.Delay(startOffset);

        // Slowly fill up the text field
        _instantLoadDialogueNextTick = false;
        char[] characters = dialogue.ToCharArray();
        foreach (var captionChar in characters)
        {
            if (dialogue != _currentDialogue) return;
            TextArea.text += captionChar;
            await Task.Delay(_punctuationDelay.ContainsKey(captionChar) ? _punctuationDelay[captionChar] : 10);
            if (_instantLoadDialogueNextTick)
            {
                TextArea.text = dialogue;
                break;
            }
        }
    }

    public void Update()
    {
        // Show the last piece of dialogue
        if (Input.GetKeyDown(KeyCode.Q) && _dialogueCG.alpha >= 0f)
            ShowDialogue(0, DialogueTransition.Backward);

        // Opens the box back up
        if (Input.GetKeyDown(KeyCode.T))
            ShowDialogue(0, DialogueTransition.OpenDialogue);

        if (Input.GetMouseButtonDown(0))
            _instantLoadDialogueNextTick = true;
    }

    public enum DialogueTransition
    {
        Forward,
        Backward,
        OpenDialogue,
    }
}
