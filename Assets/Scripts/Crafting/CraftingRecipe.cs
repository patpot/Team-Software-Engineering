using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    // adds a new option to the asset menu, for quick creation of new items
    [CreateAssetMenu(fileName = "New Recipe", menuName = "Recipe")]
    public class CraftingRecipe : ScriptableObject
    {
        public List<ItemData> Inputs;
        public ItemData Output;
        public float OutputQuantity = 1f;
    }
}