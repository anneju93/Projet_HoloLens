using UnityEngine;
using System.Collections;

[System.Serializable]

class Fighter : Joueur
{
    string SubClass, FightingStyle

    public Fighter(int lvl, int[] baseScores, string name, string race, ArrayList<string> startingInventory, string subClass, string fightingStyle)
    {
        Parent(lvl, baseScores, name, race, startingInventory)
        SubClass = subClass;
        FightingStyle = fightingStyle;

        //A fighter uses a d10 to as a Hit dice
        HitDiceSize = 10;
        Parent.GenerateMaxHP(lvl, HitDiceSize);

        Proficiencies.Add("All Armors");
        Proficiencies.Add("Shields");
        Proficiencies.Add("Simple Weapons");
        Proficiencies.Add("Martial Weapons");
        Proficiencies.Add("STR Saving Throws");
        Proficiencies.Add("CON Saving Throws");
        //These skills are normally selected by the player from choices depending on the character's Class and Background, but are hard-coded for simplicity
        Proficiencies.Add("Athletics");
        Proficiencies.Add("Acrobatics");
        Proficiencies.Add("Intimidation");
    }
}