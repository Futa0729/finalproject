using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battler.Character.Upgrades
{
    public class Equipment
    {
        public string EquipmentName { get; set; } = string.Empty;
        public EquipmentSlot Slot { get; set; }   
        public StatBoostType StatBoostType { get; set; } 
        public int BoostValue { get; set; }
        public int MaxDurability { get; set; } = 10; // how many times the item can be used
        public int CurrentDurability { get; set; } = 10; // how many times the item has been used

        public Equipment() 
        { 
        }

        public Equipment(string name, EquipmentSlot slot, StatBoostType statBoostType, int boostValue, int durability)
        {
            EquipmentName = name;
            Slot = slot;
            StatBoostType = statBoostType;
            BoostValue = boostValue;
            MaxDurability = durability;
            CurrentDurability = durability;
        }
    }
    public enum EquipmentSlot
    {
        Head,
        Chest,
        LeftArm,
        RightArm,
        Legs,
        Boots,
        Cape
    }

    public enum StatBoostType
    {
        Health,
        Power,
        Luck,
        Mana
    }
}
