using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using RPG_Battler.Character;
using RPG_Battler.Character.Upgrades;
using RPG_Battler.Gameplay;
using RPG_Battler.Gameplay.Rules;
using static RPG_Battler.Character.Upgrades.Spell;

namespace RPG_Battler.Gameplay
{
    public static class MainProgram
    {
        public static async Task Main()
        {
            string savePath = "save_hero.json";

            Hero hero = LoadHeroFromFile(savePath) ?? new Hero
            {
                Name = "Harry Potter",
                CombatClass = CombatClass.Wizard,
                Level = 1,
                TotalHealth = 100,
                Health = 100,
                TotalPower = 25,
                Power = 25,
                TotalLuck = 30,
                Luck = 30,
                Mana = 50,
                SpellCastCount = 0,
                MonstersDefeated = 0,
                Items = new List<Item>
                {
                    new Item("Herb", "A healing herb") { Quantity = 2, Weight = 1 },
                    new Item("Water", "A bottle of water") { Quantity = 1, Weight = 1 }
                },
                Skills = new List<Skill>(),
                Spells = new List<Spell>
                {
                    new Spell("Fireball", 10, 30),
                    new Spell("Wind Gust", 8, 20)
                },
                Equipment = new List<Equipment>
                {
                    new Equipment("Iron Sword", EquipmentSlot.RightArm, StatBoostType.Power, 5, 3)
                }
            };

            var monsters = new List<Monster>
            {
                new Monster("Goblin", 1, 50, 10, HabitatType.Forest),
                new Monster("Sand Worm", 2, 70, 15, HabitatType.Desert),
                new Monster("雪女", 3, 100, 20, HabitatType.Ice),
                new Monster("Skeleton", 2, 60, 12, HabitatType.Undead),
                new Monster("両面宿儺", 5, 10000, 500, HabitatType.Cave),
            };

            var arena = new CombatEnvironment("Rainy", "Night");

            bool running = true;
            while (running)
            {
                Console.WriteLine("\n🏰 Welcome to RPG Battler!");
                Console.WriteLine("1. Enter Battle");
                Console.WriteLine("2. Craft Item");
                Console.WriteLine("3. View Inventory");
                Console.WriteLine("4. Combine Spells");
                Console.WriteLine("5. Exit Game");
                Console.WriteLine("6. Save Game");
                Console.WriteLine("7. Load Game");
                Console.Write("Select an option: ");

                string input = Console.ReadLine() ?? "1";

                switch (input)
                {
                    case "1":
                        var random = new Random();
                        Monster template = monsters[random.Next(monsters.Count)];
                        var selectedMonster = new Monster(template.Name, template.Level, template.TotalHealth, template.TotalPower, template.Habitat);
                        hero.TotalHealth = hero.Health;   
                        hero.Mana        = 50; 
                        await Combat.StartBattleAsync(hero, selectedMonster, arena);
                        break;

                    case "2":
                        Console.Write("Enter item name to craft: ");
                        string craftTarget = Console.ReadLine() ?? "";
                        hero.Craft(craftTarget);
                        break;

                    case "3":
                        Console.WriteLine("\n📦 Inventory:");
                        foreach (var item in hero.Items)
                            Console.WriteLine($"- {item.ItemName} x{item.Quantity}");
                        break;

                    case "4":
                        if (hero.Spells.Count < 2)
                        {
                            Console.WriteLine("❌ Not enough spells to combine.");
                            break;
                        }

                        Console.WriteLine("Enter indexes of two spells to combine (e.g., 0 1):");
                        for (int i = 0; i < hero.Spells.Count; i++)
                            Console.WriteLine($"[{i}] {hero.Spells[i].SpellName}");

                        var inputLine = Console.ReadLine();
                        if (inputLine != null)
                        {
                            var parts = inputLine.Split(' ');
                            if (parts.Length == 2 &&
                                int.TryParse(parts[0], out int index1) &&
                                int.TryParse(parts[1], out int index2))
                            try
                            {
                                var newSpell = Spell.Combine(hero.Spells[index1], hero.Spells[index2]);
                                hero.Spells.Add(newSpell);
                                Console.WriteLine($"✨ {newSpell.SpellName} was created!");
                            }
                            catch (InvalidComboException ex)
                            {
                                Console.WriteLine($"❌ {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("❌ Invalid input.");
                        }
                        break;

                    case "5":
                        Console.WriteLine("👋 Goodbye!");
                        running = false;
                        break;

                    case "6":
                        SaveHeroToFile(hero, savePath);
                        break;

                    case "7":
                        var loaded = LoadHeroFromFile(savePath);
                        if (loaded != null)
                        {
                            hero = loaded;
                            Console.WriteLine("✅ Hero loaded successfully.");
                        }
                        break;

                    default:
                        Console.WriteLine("❌ Invalid choice. Try again.");
                        break;
                }
            }
        }

        static void SaveHeroToFile(Hero hero, string path)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true, // makes it easy to read
                IncludeFields = true // include fields
            };
            string json = JsonSerializer.Serialize(hero, options);
            File.WriteAllText(path, json);
            Console.WriteLine("💾 Game saved!");
        }

        static Hero? LoadHeroFromFile(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("⚠️ No save file found.");
                return null;
            }

            string json = File.ReadAllText(path);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true
            };
            var hero = JsonSerializer.Deserialize<Hero>(json, options);
            Console.WriteLine("📂 Game loaded!");
            return hero;
        }
    }
}