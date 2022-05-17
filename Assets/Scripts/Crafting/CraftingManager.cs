using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Assets.Scripts
{
    public class CraftingManager : MonoBehaviour
    {
        public static CraftingManager Instance;
        public static bool Active => Instance.CraftingUI.activeSelf;
        public GameObject CraftingUI;
        public Transform RecipeHolder;
        public GameObject SideMenu;
        public Transform SideMenuInputHolder;
        public GameObject CraftingOutput;
        public Button CraftButton;

        public GameObjectPool InputItemPool;
        private static Dictionary<string, CraftingRecipe> _recipes = new Dictionary<string, CraftingRecipe>();

        // XD
        private Dictionary<string, string> _flavourTexts = new Dictionary<string, string>()
        {
            { "Crusher", "Converts 1 Earth Mana and 1 Wood Log into 1 Branch over 1 second." },
            { "Crystalliser", "Converts 1 Earth Mana into 1 Earth Crystal over 1 second." },
            { "Diffuser", "Converts 0.5 Wood Logs into 1 Earth Mana over 1 second." },
            { "Earthen Amplification Crystal", "A powerful crystal capable of channeling aura into a machine." },
            { "Earthen Mana Synthesiser", "Produces 1 Earth Mana every 10 seconds." },
            { "Replicator", "Converts 1 Earth Crystal into 1 Wood Log over 1 second." },
            { "Wooden Chest", "Contains 20 slots that each store 10 items." },
        };

        private void Awake()
        {
            Instance = this;
        }
        void Start()
        {
            // Load in all our items from our Resources/Crafting folder
            Object[] recipes = Resources.LoadAll("Crafting", typeof(CraftingRecipe));
            foreach (var recipe in recipes)
                _recipes.Add(recipe.name, recipe as CraftingRecipe);

            // Now we've loaded them all, draw our UI!
            // Get our prefab so we can start filling it in
            GameObject recipePrefab = UIManager.GetPrefab("RecipePreview");
            foreach (var recipe in _recipes)
            {
                // Create our holder and properly parent it
                GameObject recipeHolder = Instantiate(recipePrefab);
                recipeHolder.transform.SetParent(RecipeHolder, false);

                // Set our icon
                Image iconImg = recipeHolder.GetComponentsInChildren<Image>()[1]; // 0 is background, 1 is icon
                iconImg.sprite = recipe.Value.Output.Icon;

                // Assign our event for when we get clicked on
                Button btn = recipeHolder.GetComponentInChildren<Button>();
                btn.onClick.AddListener(() => ShowRecipe(recipe.Key));
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.C) && !CameraSwitcher.BuildMode)
                ToggleCraftingMenu();
        }

        public void ToggleCraftingMenu()
        {
            if (!CraftingUI.activeSelf && UIManager.UIActive || UIManager.Instance.InventoryUI.activeSelf) return; // Don't draw any new UI if we have UI active            
            CraftingUI.SetActive(!CraftingUI.activeSelf);

            // If the main crafting UI was deactivated, deactivate our side menu and fake player inventory as well
            if (!CraftingUI.activeSelf)
            {
                SideMenu.SetActive(false);
                UIManager.Instance.FakePlayerInventory.SetActive(false);
                UIManager.UnlockCamera();
                UIManager.LockCursor();
            }

            UIManager.UIActive = CraftingUI.activeSelf; // If the UI was activated tell our UI manager we can't render other UI, if deactivated vice versa

            if (CraftingUI.activeSelf)
            {
                Utils.FadeInUI(CraftingUI);
                UIManager.LockCamera();
                UIManager.UnlockCursor();

                // Draw the player's fake inventory
                PlayerInventory.Instance.OpenFakeInventory();
            }
        }

        public void ShowRecipe(string recipeName)
        {
            SideMenu.SetActive(true);
            // Iterate backwards through our children and send them back to the pool, if we did it forward we'd be changing indexes mid iteration so we flip it
            for (int i = SideMenuInputHolder.childCount - 1; i >= 0; i--)
                InputItemPool.ReturnToPool(SideMenuInputHolder.GetChild(i).gameObject);

            // Find our recipe and split it into a dictionary of item data mapped to quantity
            CraftingRecipe recipe = _recipes[recipeName];
            Dictionary<ItemData, float> inputsByItem = RecipesToQuantities(recipe);

            // Display the name of our recipe output
            SideMenu.GetComponentInChildren<TextMeshProUGUI>().text = recipeName;

            // Put together our UI elements
            foreach (var input in inputsByItem)
            {
                GameObject inputItem = InputItemPool.GetObjectFromPool();
                inputItem.SetActive(true);
                inputItem.transform.SetParent(SideMenuInputHolder);
                // Get our UI elements
                Image background = inputItem.GetComponentsInChildren<Image>()[0]; // 0 is background, 1 is icon
                Image icon = inputItem.GetComponentsInChildren<Image>()[1]; // 0 is background, 1 is icon
                TextMeshProUGUI quantity = inputItem.GetComponentInChildren<TextMeshProUGUI>();

                // Check if the player has this quantity of items, if so set the icon to green, if not set it to red
                if (PlayerInventory.Instance.BoolContainsItems(input.Key.Name, input.Value))
                    background.color = Color.green;
                else
                    background.color = Color.red;

                // Assign them values
                icon.sprite = input.Key.Icon;
                quantity.text = input.Value.ToString();
            }

            // Next, draw our output
            Image outputIcon = CraftingOutput.GetComponentsInChildren<Image>()[1]; // 0 is background, 1 is icon
            TextMeshProUGUI outputQuantity = CraftingOutput.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI flavourText = CraftingOutput.GetComponentsInChildren<TextMeshProUGUI>()[1];

            // Assign them values
            outputIcon.sprite = recipe.Output.Icon;
            outputQuantity.text = recipe.OutputQuantity.ToString();
            flavourText.text = _flavourTexts[recipeName];

            // Assign our crafting button the correct recipe
            CraftButton.onClick.RemoveAllListeners();
            CraftButton.onClick.AddListener(() => AttemptCraft(recipeName));
        }

        public static Dictionary<ItemData, float> RecipesToQuantities(string recipeName)
            => RecipesToQuantities(_recipes[recipeName]);
        public static Dictionary<ItemData, float> RecipesToQuantities(CraftingRecipe recipe)
        {
            // Unity doesn't have a good way to serialize dictionaries in editor, and as all recipes are set in editor we have to convert them to a more usable format.
            // When creating a recipe you just create a list of objects, so if you want 4 crystals you will put crystal in the list 4 times, that's not ideal for operations
            // So we have this helper function to move recipes into a more usable format.

            Dictionary<ItemData, float> inputsByItem = new Dictionary<ItemData, float>();
            foreach (var input in recipe.Inputs)
            {
                if (inputsByItem.ContainsKey(input))
                    inputsByItem[input]++;
                else
                    inputsByItem.Add(input, 1);
            }

            return inputsByItem;
        }

        public void AttemptCraft(string recipeName)
        {
            CraftingErrorType errorType = PlayerInventory.Instance.AttemptCraft(_recipes[recipeName]);
            if (errorType == CraftingErrorType.NoInputs)
            {
                Debug.Log("Not enough inputs to craft this!");
            }
            else if (errorType == CraftingErrorType.NoSpareSlots)
            {
                Debug.Log("No inventory slots to put the output!");
            }
            else
            {
                CraftingRecipe recipe = _recipes[recipeName];
                Dictionary<ItemData, float> requiredInputs = CraftingManager.RecipesToQuantities(recipe);
                int i = 0;
                foreach (var reqInp in requiredInputs)
                {
                    Image inputBackground = SideMenuInputHolder.GetChild(i).GetComponentsInChildren<Image>()[0]; // 0 is background, 1 is icon
                    if (PlayerInventory.Instance.BoolContainsItems(reqInp.Key.Name, reqInp.Value))
                        inputBackground.color = Color.green;
                    else
                        inputBackground.color = Color.red;
                    i++;
                }
            }
        }

        public enum CraftingErrorType
        {
            NoInputs,
            NoSpareSlots,
            NoError
        }
    }
}