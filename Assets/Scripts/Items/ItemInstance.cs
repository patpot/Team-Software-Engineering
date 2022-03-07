using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class ItemInstance : MonoBehaviour
    {
        // allows for the scriptableObject item to be set as the item of choice for any gameObject
        public Item AssignedItem;
        // Start is called before the first frame update
        void Start()
        {
            // resets the amount of items the player has
            AssignedItem.ItemCount = 0;
        }

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // when the player collects an item, add one to the count
                AssignedItem.ItemCount += 1;
            }
        }
    }
}
