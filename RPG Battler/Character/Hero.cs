using RPG_Battler.Character.Upgrades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battler.Character
{
    public class Hero : Creations
    {
        public int Health { get; set; }
        public int Power { get; set; }
        public int Luck { get; set; }
        public int Mana { get; set; }
        public int ExperienceRemaining { get; set; }
        public CombatClass CombatClass { get; set; }
        public List<Item>      Items      { get; set; } = new();
        public List<Skill>     Skills     { get; set; } = new();
        public List<Spell>     Spells     { get; set; } = new();
        public List<Equipment> Equipment  { get; set; } = new();
        public DateTime LastLevelUpDate { get; set; } = DateTime.Now;

        public int SpellCastCount { get; set; } = 0;
        public int MonstersDefeated { get; set; } = 0;


        public Hero()
        {       
        }

        public void LevelUp()
        {
            Level++;

            switch (CombatClass)
            {
                case CombatClass.Warrior:
                    TotalHealth += 20;
                    TotalPower  += 5;
                    break;

                case CombatClass.Wizard:
                    TotalPower += 8;
                    Mana       += 10;
                    break;

                case CombatClass.Rogue:
                    TotalLuck += 7;
                    TotalPower += 4;
                    break;

                default:
                    TotalHealth += 10;
                    TotalPower  += 3;
                    break;
            }

            
            CalculateTotals();

            Console.WriteLine($"{Name} leveled up to {Level}!");


        } 

        public void CheckTimeBasedLevelUp() // check if they can level up after 1 day
        {
            TimeSpan timePassed = DateTime.Now - LastLevelUpDate;
            if (timePassed.TotalDays >= 1) 
            {
                LevelUp();
                LastLevelUpDate = DateTime.Now;
                Console.WriteLine($"⏳ {Name} time-based leveled up!");
            }
            else
            {
                Console.WriteLine($"🕒 {Name} is not ready to level up yet (last: {LastLevelUpDate})");
            }
        }       

        public void ApplyEquipmentWear()
        {
            foreach (var equipment in Equipment)
            {
                if (equipment.CurrentDurability > 0)
                {
                    equipment.CurrentDurability--;
                    Console.WriteLine($"🛠️ {equipment.EquipmentName} durability: {equipment.CurrentDurability}/{equipment.MaxDurability}");
                }
                else if (equipment.CurrentDurability == 0)
                {
                    Console.WriteLine($"💥 {equipment.EquipmentName} has broken!");
                }
            }
        }

        public void CalculateTotals()
        {
            TotalPower = Power;
            TotalHealth = Health;
            TotalLuck = Luck;
            Mana = Mana;

            foreach (var equipment in Equipment)
            {
                if (equipment.CurrentDurability > 0)
                {
                    switch (equipment.StatBoostType)
                    {
                        case StatBoostType.Power:
                            TotalPower += equipment.BoostValue;
                            break;
                        case StatBoostType.Health:
                            TotalHealth += equipment.BoostValue;
                            break;
                        case StatBoostType.Luck:
                            TotalLuck += equipment.BoostValue;
                            break;
                    }
                }
                else
                {
                    Console.WriteLine($"😢 {equipment.EquipmentName} is already broken.");
                }
            }
        }
        public int InventoryWeightLimit { get; set; } = 20;

        public bool AddItem(Item newItem)
        {
            int currentWeight = Items.Sum(i => i.Weight * i.Quantity); // sum of all current item's weights

            if (currentWeight + (newItem.Weight * newItem.Quantity) > InventoryWeightLimit)
            {
                Console.WriteLine($"❌ Inventory is too heavy to add {newItem.ItemName}.");
                return false;
            }

            if (newItem.Stackable)
            {
                var existing = Items.FirstOrDefault(i => i.ItemName == newItem.ItemName); //looking for existing item
                if (existing != null)
                {
                    existing.Quantity += newItem.Quantity;
                    return true;
                }
            }

            Items.Add(newItem);
            return true;
        }
        public void Craft(string itemName)
        {
            //recipe dictionary
            var recipes = new Dictionary<string, List<string>>
            {
                { "Lucky Sword", new List<string> { "Stick", "Stick" } },
                { "Weekly Shonen Jump", new List<string> { "gum-gum fruit", "Death Note" } },
                { "Toriyama Akira", new List<string> { "Kamehameha", "Totem of Undying" } },
                { "Potion", new List<string> { "Herb", "Water" } }
            };

            if (!recipes.ContainsKey(itemName))
            {
                Console.WriteLine($"❌ Unknown recipe: {itemName}");
                return;
            }


            var needed = recipes[itemName]; // get the required items for the recipe
            var allHave = needed.All(req => Items.Any(i => i.ItemName == req && i.Quantity >= 1)); // check if all required items are in inventory

            if (!allHave)
            {
                Console.WriteLine($"❌ Missing ingredients to craft {itemName}");
                return;
            }

            // remove the required items from inventory
            foreach (var req in needed)
            {
                var item = Items.First(i => i.ItemName == req);
                item.Quantity--;
                if (item.Quantity <= 0) Items.Remove(item);
            }

            // add the crafted item to inventory
            switch (itemName)
            {
                case "Lucky Sword":
                    Equipment.Add(new Equipment("Lucky Sword", EquipmentSlot.RightArm, StatBoostType.Power, 10, 5));
                    break;
                case "Weekly Shonen Jump":
                    Equipment.Add(new Equipment("Weekly Shonen Jump", EquipmentSlot.RightArm, StatBoostType.Mana, 8, 4));
                    break;
                case "Toriyama Akira":
                    Equipment.Add(new Equipment("Toriyama Akira", EquipmentSlot.LeftArm, StatBoostType.Health, 12, 6));
                    break;
                case "Potion":
                    Items.Add(new Item("Potion", "Heals 30 HP") { Quantity = 1 });
                    break;
            }

            Console.WriteLine($"🛠️ You crafted: {itemName}!");
            
            switch (itemName)
            {
                case "Lucky Sword":
                    Console.WriteLine("✨ The Lucky Sword glows, and feel luckyyyyyyyyyyy!");
                    break;
                case "Weekly Shonen Jump":
                    Console.WriteLine("📚 Only 290 yen every Monday!.");
                    break;
                case "Toriyama Akira":
                    Console.WriteLine("🤬 Are you talking about Kririn?!!!!!");
                    break;
                case "Potion":
                    Console.WriteLine("💧 You crafted a Potion! It can heal you.");
                    break;
            }

            CalculateTotals();
        }


        
    }
}
