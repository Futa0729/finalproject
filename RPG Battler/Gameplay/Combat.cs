using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RPG_Battler.Character;
using RPG_Battler.Character.Upgrades;
using RPG_Battler.Gameplay.Rules;
using static RPG_Battler.Character.Upgrades.Spell;

namespace RPG_Battler.Gameplay
{
    public static class Combat
    {
        public static async Task StartBattleAsync(Hero hero, Monster monster, CombatEnvironment env)
        {
            var rules = RuleCatalog.GetRules(env, hero.CombatClass);
            rules.Add(new EnvironmentMonsterRule(env.Weather, monster.Habitat));
            foreach (var rule in rules)
            {
                rule.Apply(hero, monster);
                Console.WriteLine($"Rule applied: {rule.Description}");
            }

            Console.WriteLine($"\n⚔️ {hero.Name} (HP {hero.TotalHealth}, Power {hero.TotalPower}) vs {monster.Name} (HP {monster.TotalHealth}, Power {monster.TotalPower})");

            while (hero.TotalHealth > 0 && monster.TotalHealth > 0)
            {
                Console.WriteLine("\nChoose your action:");
                Console.WriteLine("1. Attack");
                Console.WriteLine("2. Cast Single Spell");
                Console.WriteLine("3. Cast Combo Spell");
                Console.WriteLine("4. Use Item");
                Console.Write("> ");
                string input = Console.ReadLine() ?? "1";

                if (input == "1")
                {
                    monster.TotalHealth -= hero.TotalPower;
                    Console.WriteLine($"{hero.Name} attacks for {hero.TotalPower} ➜ {monster.TotalHealth} HP left");
                }
                else if (input == "2" && hero.Spells.Count > 0)
                {
                    hero.SpellCastCount++;
                    if (hero.SpellCastCount == 5)
                        Console.WriteLine("🏆 Quest Complete: Cast 5 spells!");

                    Console.WriteLine("Choose a spell to cast:");
                    for (int i = 0; i < hero.Spells.Count; i++)
                        Console.WriteLine($"[{i}] {hero.Spells[i].SpellName}");

                    if (int.TryParse(Console.ReadLine(), out int spellIndex) &&
                        spellIndex >= 0 && spellIndex < hero.Spells.Count)
                    {
                        var spell = hero.Spells[spellIndex];
                        if (hero.Mana >= spell.ManaCost)
                        {
                            Console.WriteLine($"{hero.Name} casts {spell.SpellName.ToUpper()}!");
                            int dmg = spell.CalculateSpellDamage(hero.TotalPower);
                            spell.CastSpell(hero);
                            monster.TotalHealth -= dmg;
                            hero.Mana -= spell.ManaCost;
                            Console.WriteLine($"{hero.Name} deals {dmg} ➜ {monster.TotalHealth} HP left");
                        }
                        else
                        {
                            Console.WriteLine("❌ Not enough mana!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("❌ Invalid index.");
                    }
                }
                else if (input == "3" && hero.Spells.Count >= 2)
                {
                    Console.WriteLine("Enter two spells to combine (e.g., 0 1):");
                    for (int i = 0; i < hero.Spells.Count; i++)
                        Console.WriteLine($"[{i}] {hero.Spells[i].SpellName}");

                    string line = Console.ReadLine();
                    if (line != null)
                    {
                        var parts = line.Split(' ');
                        if (parts.Length == 2 &&
                            int.TryParse(parts[0], out int idx1) &&
                            int.TryParse(parts[1], out int idx2))
                        {
                            try
                            {
                                var combo = Spell.Combine(hero.Spells[idx1], hero.Spells[idx2]);
                                if (hero.Mana >= combo.ManaCost)
                                {
                                    int dmg = combo.CalculateSpellDamage(hero.TotalPower);
                                    combo.CastSpell(hero);
                                    monster.TotalHealth -= dmg;
                                    hero.Mana -= combo.ManaCost;
                                    Console.WriteLine($"{hero.Name} unleashes {combo.SpellName} for {dmg} ➜ {monster.TotalHealth} HP left");
                                }
                                else
                                {
                                    Console.WriteLine("❌ Not enough mana for combo.");
                                }
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
                    }
                }
                else if (input == "4" && hero.Items.Count > 0)
                {
                    Console.WriteLine("Select an item:");
                    for (int i = 0; i < hero.Items.Count; i++)
                        Console.WriteLine($"[{i}] {hero.Items[i].ItemName} x{hero.Items[i].Quantity}");

                    if (int.TryParse(Console.ReadLine(), out int itemIdx) &&
                        itemIdx >= 0 && itemIdx < hero.Items.Count)
                    {
                        var item = hero.Items[itemIdx];

                        if (item.ItemName == "Potion")
                        {
                            hero.TotalHealth += 30;
                            Console.WriteLine($"🧪 +30 HP ➜ {hero.TotalHealth}");
                        }
                        if (item.IsCursed)
                        {
                            hero.TotalHealth -= 10;
                            Console.WriteLine($"💀 Curse! {item.CurseEffectDescription}");
                        }

                        item.Quantity--;
                        if (item.Quantity <= 0) hero.Items.RemoveAt(itemIdx);
                    }
                    else
                    {
                        Console.WriteLine("❌ Invalid selection.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid action.");
                }

                if (monster.TotalHealth <= 0) break;

                // Monster counter-attack
                hero.TotalHealth -= monster.TotalPower;
                Console.WriteLine($"{monster.Name} hits for {monster.TotalPower} ➜ {hero.TotalHealth} HP left");

                await Task.Delay(300);
            }

            bool heroWon = hero.TotalHealth > 0;
            Console.WriteLine(heroWon ? $"\n🎉 {hero.Name} wins!" : $"\n💀 {monster.Name} wins!");

            if (heroWon)
            {
                var loot = await LootBox.OpenAsync(hero);
                if (loot != null)
                    Console.WriteLine($"🎁 {hero.Name} found: {loot.Name} ({loot.Rarity})");
            }
        }
    }
}
