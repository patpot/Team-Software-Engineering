using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts
{
    // adds a new option to the asset menu, for quick creation of new items
    [CreateAssetMenu(fileName = "New Item", menuName = "Item")]
    public class ItemData : ScriptableObject
    {
        // all the statistics of the items
        public int Id;
        public string Name => this.name;
        public string Description;
        public Sprite Icon;
    }
}