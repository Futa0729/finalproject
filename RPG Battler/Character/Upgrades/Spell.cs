using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPG_Battler.Character;
using RPG_Battler.Character.Upgrades;
using RPG_Battler.Gameplay;
using RPG_Battler.Gameplay.Rules;


namespace RPG_Battler.Character.Upgrades
{
    public class Spell
    {
        public string SpellName { get; set; }
        public int ManaCost { get; set; }
        public int BaseDamage { get; set; }
        public int CastCount { get; private set; } = 0; // how many times the spell has been cast


        public Spell(string name, int manaCost, int baseDamage)
        {
            SpellName = name;
            ManaCost = manaCost;
            BaseDamage = baseDamage;
        }

        public int CalculateSpellDamage(int heroPower)
        {
            int bonus = CastCount >= 5 ? 10 : 0; 
            return BaseDamage + (heroPower / 2) + bonus;
        }

        public void CastSpell(Hero hero)
        { 
            CastCount++;
        }

        public class FirestormSpell : Spell
        {
            public FirestormSpell() : base("Firestorm", manaCost: 20, baseDamage: 60) { }
        }

        public class BlizzardSpell : Spell
        {
            public BlizzardSpell() : base("Blizzard", manaCost: 18, baseDamage: 55) { }
        }
        public static Spell Combine(Spell a, Spell b)
        {
            if ((a.SpellName == "Fireball" && b.SpellName == "Wind Gust") ||
                (a.SpellName == "Wind Gust" && b.SpellName == "Fireball"))
                return new FirestormSpell();

            if ((a.SpellName == "Ice Shard" && b.SpellName == "Wind Gust") ||
                (a.SpellName == "Wind Gust" && b.SpellName == "Ice Shard"))
                return new BlizzardSpell();

            throw new InvalidComboException($"Cannot combine {a.SpellName} and {b.SpellName}");
        }
        public class InvalidComboException : Exception
        {
            public InvalidComboException(string message) : base(message) { }
        }

    }
}
