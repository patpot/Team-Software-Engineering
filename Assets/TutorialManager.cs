using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static DialogueManager;

public class TutorialManager : MonoBehaviour
{
    public CraftingManager CraftingManager;
    public DialogueManager DialogueManager;
    public Dictionary<string, List<string>> TutorialDialogue = new Dictionary<string, List<string>>()
    {
        { "SpellbookOpen", new List<string>() {
            "Now that you've got the Spellbook open you can perform all sorts of magic, for now you'll need to release some natural Aura out of your surroundings!",
            "The best source of Aura on this island are those small trees, head over to one of them now and use your Spellbook by pressing \"Mouse1\" while aiming at one and it'll transmute into raw resources.",
        }},
        { "WoodLogAcquired", new List<string>() {
            "Cool right? Now that you've safely transmuted a tree into its raw resources you can use your spellbook to convert it into some Earth Mana.",
            "The Wood Log you just acquired has automatically been placed in your inventory. To open your inventory press \"E\".",
        }},
        { "InventoryUsage", new List<string>() {
            "Great! This is where you'll find all your items that you've acquired, you'll be able to make some external inventories to store items in soon enough, but for now you'll need to use your inventory to do some things.",
            "To move items around your inventory first select the slot by pressing \"Mouse1\", now that you are dragging the item around you can either hover over another inventory slot (in your own or any external inventory) and press \"Mouse1\" again to move the entire stack, \"Mouse2\" to move one item from the stack, or click out of the inventory and onto the floor to place the object in world. An object placed in-world can be picked up again by pressing \"Mouse1\" on it with the Spellbook out, unless it has a special interaction.",
            "Wood Logs are useful for making machines, however for now we need to convert them into Earth Mana. To do this drag that Wood Log out of your inventory and onto the ground, get out your Spellbook, aim at the Log and press \"Mouse1\".",
        }},
        { "ManaAcquired", new List<string>() {
            "Good work! You're well on your way to creating your first machine. Using the power of your spellbook you're able to combine these resources that you're collecting. To open up the crafting interface press the \"C\" key. You can view your own inventory in the crafting menu, so make sure to close any inventories you have open to be able to open it.",
        }},
        { "CraftingUIOpen", new List<string>() {
            "Here's the interface in which you'll be making all your machines and components. Looking at the recipes you might notice you don't recognise some of the resources, don't worry, you'll soon enough be making machines to produce those.",
            "Your first machine to craft should be the Crusher, this machine will take Earth Mana and Wood Logs and turn them into branches, which help you create some more advanced machines. Firstly you'll need to craft an Earthen Amplification Crystal with some Earth Mana, then combine it with a few Wood Logs to channel the Aura into this machine. You can do this manually for now, and later on automate the process. Report back once you've made the Crusher.",
        }},
        { "CrusherMade", new List<string>() {
            "Excellent work recruit, your basic training is almost done! In order to place this machine properly you can open your third eye and enter what we call \"building mode\". This will give you a top down view of the island. Press \"B\" to enter building mode.",
        }},
        { "BuildingMode", new List<string>() {
            "Now that you're in building mode you're able to place down your crusher. To place a machine select it from the toolbar then press \"Mouse1\", to navigate use the \"WASD\" keys, use \"MouseWheel\" to zoom in and out, if you need to rotate the machine you can hit the \"R\" key and if you place it in the wrong place you can press \"Mouse2\" to pick it back up. Select the Crusher from your toolbar and place it anywhere valid (not inside a tree) the preview will show exactly how it will place.",
        }},
        { "CrusherPlaced", new List<string>() {
            "Great! Let's go check up on that machine back in your normal view. Press \"B\" once again to return to your normal view. To access a machine's interface press \"Mouse1\" on it WITHOUT the Spellbook equipped.",
        }},
        { "MachineUI", new List<string>() {
            "This is how you'll manually access machines. Drag and drop items into the input slot, after however long it says it'll take your output item will be ready! Magic right?",
            "Use this Crusher to create some branches and get started on making some more machines! You'll need a Crystalliser next in order to unlock everything at your disposal. To close this UI press \"E\"",
        }},
        { "CrystalliserMade", new List<string>() {
            "Now that we're getting more into more machines it's time to learn how to automate them. Take out your Spellbook by pressing \"1\"and press \"Mouse1\" while looking at a machine (you'll also need to be pretty close), this will start the output registering process, in order to finish this process you'll need to press \"Mouse1\" again on a target inventory, such as a chest or in a more useful case another machine. For now you can craft a basic Wooden Chest if you want to try this!",
            "That concludes basic work training, its up to you to set up your machines and get to work, you've got complete freedom over how you proceed, but be advised Wood Logs come in limited capacity, running out of those might render this island permanently uninhabitable! Now to complete this assignment and guarantee the Aura of this island remains stable I'm going to need you to produce 100 Earth Crystals and deposit them into the inactive portal to the right side of the island, with this portal stabilized we'll be able to move you onto your next assignment.",
        }},
    };
    public Dictionary<string, string> TutorialTasks = new Dictionary<string, string>()
    {
        { "FirstTask", "Open your Spellbook with \"1\""},
        { "SpellbookOpen", "Click on a small tree to gather a Wood Log"},
        { "WoodLogAcquired", "Press \"E\" to open your Inventory"},
        { "InventoryUsage", "Drag a Wood Log out of your Inventory and with the Spellbook press \"Mouse1\" while looking at it"},
        { "ManaAcquired", "Press \"C\" to open the Crafting UI"},
        { "CraftingUIOpen", "Craft a Crusher"},
        { "CrusherMade", "Press \"B\" to enter Building Mode"},
        { "BuildingMode", "Place a Crusher in Building Mode"},
        { "CrusherPlaced", "Open a Machine's UI by pressing \"Mouse1\" on it without your Spellbook equipped"},
        { "MachineUI", "Craft a Crystalliser"},
        { "CrystalliserMade", "Fill the Portal Chest on the right of the island with 100 Earth Crystals"},
    };

    public TextMeshProUGUI Task;

    private float _crusherCount;
    private TutorialState _currentState;

    private void Start()
    {
        Task.text = TutorialTasks["FirstTask"];
    }

    public void Update()
    {
        switch (_currentState)
        {
            case TutorialState.Spellbook:
                if (SpellbookToggle.SpellbookActive)
                {
                    foreach (var dialogue in TutorialDialogue["SpellbookOpen"])
                        DialogueManager.AddDialogue(dialogue);
                    Task.text = TutorialTasks["SpellbookOpen"];

                    DialogueManager.ShowDialogue();
                    _currentState = TutorialState.OpenInventory;
                };
                break;
            case TutorialState.OpenInventory:
                if (PlayerInventory.Instance.BoolContainsItems("Wood Log", 1f))
                {
                    foreach (var dialogue in TutorialDialogue["WoodLogAcquired"])
                        DialogueManager.AddDialogue(dialogue);
                    Task.text = TutorialTasks["WoodLogAcquired"];

                    DialogueManager.ShowDialogue();
                    _currentState = TutorialState.InventoryUsage;
                }
                break;
            case TutorialState.InventoryUsage:
                if (UIManager.Instance.InventoryUI.activeSelf)
                {
                    foreach (var dialogue in TutorialDialogue["InventoryUsage"])
                        DialogueManager.AddDialogue(dialogue);
                    Task.text = TutorialTasks["InventoryUsage"];

                    DialogueManager.ShowDialogue();
                    _currentState = TutorialState.ManaAcquired;
                }
                break;
            case TutorialState.ManaAcquired:
                if (PlayerInventory.Instance.BoolContainsItems("Earth Mana", 1f))
                {
                    foreach (var dialogue in TutorialDialogue["ManaAcquired"])
                        DialogueManager.AddDialogue(dialogue);
                    Task.text = TutorialTasks["ManaAcquired"];

                    DialogueManager.ShowDialogue();
                    _currentState = TutorialState.CraftingMenu;
                }
                break;
            case TutorialState.CraftingMenu:
                if (CraftingManager.Active)
                {
                    foreach (var dialogue in TutorialDialogue["CraftingUIOpen"])
                        DialogueManager.AddDialogue(dialogue);
                    Task.text = TutorialTasks["CraftingUIOpen"];

                    DialogueManager.ShowDialogue();
                    _currentState = TutorialState.CrusherMade;
                }
                break;
            case TutorialState.CrusherMade:
                if (PlayerInventory.Instance.BoolContainsItems("Crusher", 1f))
                {
                    foreach (var dialogue in TutorialDialogue["CrusherMade"])
                        DialogueManager.AddDialogue(dialogue);
                    Task.text = TutorialTasks["CrusherMade"];

                    DialogueManager.ShowDialogue();
                    _currentState = TutorialState.BuildingMode;
                }
                break;
            case TutorialState.BuildingMode:
                if (CameraSwitcher.BuildMode)
                {
                    foreach (var dialogue in TutorialDialogue["BuildingMode"])
                        DialogueManager.AddDialogue(dialogue);
                    Task.text = TutorialTasks["BuildingMode"];

                    DialogueManager.ShowDialogue(400);
                    // We can't really check if a player doesn't have any crushers to check if they placed one, as they technically could've crafted multiple
                    // so we cache how many they had when this started, and then check if they have less than that in the next case
                    _crusherCount = PlayerInventory.Instance.GetItemCount("Crusher");
                    _currentState = TutorialState.CrusherPlaced;
                }
                break;
            case TutorialState.CrusherPlaced:
                if (PlayerInventory.Instance.GetItemCount("Crusher") < _crusherCount)
                {
                    foreach (var dialogue in TutorialDialogue["CrusherPlaced"])
                        DialogueManager.AddDialogue(dialogue);
                    Task.text = TutorialTasks["CrusherPlaced"];

                    DialogueManager.ShowDialogue(400);
                    _currentState = TutorialState.MachineUI;
                }
                break;
            case TutorialState.MachineUI:
                if (UIManager.MachineUIActive)
                {
                    foreach (var dialogue in TutorialDialogue["MachineUI"])
                        DialogueManager.AddDialogue(dialogue);
                    Task.text = TutorialTasks["MachineUI"];

                    DialogueManager.ShowDialogue(400);
                    _currentState = TutorialState.CrystalliserMade;
                }
                break;
            case TutorialState.CrystalliserMade:
                if (PlayerInventory.Instance.BoolContainsItems("Crystalliser", 1f))
                {
                    foreach (var dialogue in TutorialDialogue["CrystalliserMade"])
                        DialogueManager.AddDialogue(dialogue);
                    Task.text = TutorialTasks["CrystalliserMade"];

                    DialogueManager.ShowDialogue(400);
                    _currentState = TutorialState.None;
                }
                break;
        }
    }

    private enum TutorialState 
    { 
        Spellbook,
        OpenInventory,
        InventoryUsage,
        ManaAcquired,
        CraftingMenu,
        CrusherMade,
        BuildingMode,
        CrusherPlaced,
        MachineUI,
        CrystalliserMade,
        None
    }
    
}
