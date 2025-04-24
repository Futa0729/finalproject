using RPG_Battler.Character;

[Habitat("Rainy")]  
public class Goblin : Monster
{
    public Goblin()
        : base("Goblin", level: 1, health: 80, power: 10, habitat: HabitatType.Forest)
    {
    }
}
