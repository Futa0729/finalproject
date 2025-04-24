using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battler.Character.Upgrades
{
    public enum ItemType
    {
        Potion,
        Material,
        Craftable
    }

    public class Item
    {
        public string ItemName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ItemPower { get; set; }
        public ItemType Type { get; set; } = ItemType.Material;
        public int Quantity { get; set; } = 1;
        public bool Stackable { get; set; } = true; // true if player can have multiple of this item
        public int Weight { get; set; } = 1;
        public bool IsCursed { get; set; } = false;
        public string CurseEffectDescription { get; set; } = "";


        public Item() 
        { 
        }

        public Item(string name, string description)
        {
            ItemName = name;
            Description = description;
        }


    }
}
