using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battler.Character
{
    public enum HabitatType
    {
        Forest,
        Desert,
        Ice,
        Cave,
        Undead
    }
    
    public class Monster : Creations
    {
        public HabitatType Habitat { get; set; }
        public Monster() { }

        public Monster(string name, int level, int health, int power, HabitatType habitat)
        {
            Name = name;
            Level = level;
            TotalHealth = health;
            TotalPower = power;
            Habitat = habitat;
        }
    }
    
}
