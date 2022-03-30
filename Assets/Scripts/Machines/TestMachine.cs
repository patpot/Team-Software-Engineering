using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Machines
{
    public class TestMachine : BasicMachine
    {
        void Awake()
        {
            MachineName = "Earthen Mana Synthesiser 2";
            ItemData earthMana = ItemManager.GetItemData("Earth Mana");
            Inputs.Add(earthMana, 1f);

            ItemData earthCrystal = ItemManager.GetItemData("Earth Crystal");
            Outputs.Add(earthCrystal, 1f);

            TimeToProduce = 1f;
        }
    }

}
