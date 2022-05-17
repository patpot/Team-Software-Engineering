using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public class ItemStack : MonoBehaviour
    {
        public ItemData ItemData;
        public string Name => ItemData.name;
        public float ItemCount;
        public bool Collectible = true;
    }
}
