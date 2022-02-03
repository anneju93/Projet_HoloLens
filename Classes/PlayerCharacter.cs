using UnityEngine;
using System.Collections;

[System.Serializable]

public class PlayerCharacter
{
    int Lvl, BaseAC, AC, MaxHP, HP, Speed, MoveLeft, ProfMod, HitDiceSize, HitDiceLeft;
    int[] AbilityScores, AbilityMods;
    string[] AbilityNames {"STR", "DEX", "CON", "INT", "WIS", "CHA"};
    string Name, Race;
    ArrayList<string> Proficiencies, Inventory, Equipement, Resistances, Feats;

    public PlayerCharacter(int lvl, int[] baseScores, string name, string race, ArrayList<string> startingInventory)
    {
        Lvl = 1;
        AbilityScores = GetRacialBoost(race, baseScores);
        AbilityMods = GetModifiers(AbilityScores);
        BaseAC = 10 + AbilityMods[1];
        AC = BaseAC;
        HitDiceLeft = lvl;
        Inventory = startingInventory;
        Proficiencies = new();
        Equipement = new();
        Resistances = new();
        Feats = new();

        //These values are determined by the class, these are just default values
        HitDiceSize = 8;
        MaxHP = 25;
        HP = MaxHP;
    }

    private void GenerateMaxHP(int lvl, int HitDiceSize)
    {
        int hpRoll = 0;

        //To generate Max Hp, we "roll" hit dice equal to lvl - 1 and add the result to the maximum of 1 hit die and the CON modifier X the lvl
        for (int x = 0; x < lvl - 1; x++)
        {
            hpRoll += Math.Floor(Random.Next(1, HitDiceSize + 1));
        }

        this.MaxHP = HitDiceSize + hpRoll + (lvl * AbilityMods[2]);
        HP = this.MaxHP;
    }

    private int[] GetModifiers(int[] scores)
    {
        int[] mods = new int[6];

        for (int x = 0; x <= 5; x++)
        {
            mods[x] = Math.Floor((scores[x] - 10) / 2);
        }
        return mods;
    }

    private int GetProficiencyMod(int lvl)
    {
        int mod = Math.Ceiling(lvl / 4) + 1;
        return mod;
    }

    private int[] GetRacialBoost(string race, int[] baseScores)
    {
        int[] boost;
        switch (race)
        {
            case "Dwarf (Hill)":
                boost = new int[] { 0, 0, 2, 0, 1, 0 };
                break;
            case "Dwarf (Mountain)":
                boost = new int[] { 2, 0, 2, 0, 0, 0 };
                break;
            case "Dragonborn":
                boost = new int[] { 2, 0, 0, 0, 0, 1 };
                break;
            case "Elf (Drow)":
            case "Halfling (Lightfoot)":
                boost = new int[] { 0, 2, 0, 0, 0, 1 };
                break;
            case "Elf (High)":
                boost = new int[] { 0, 2, 0, 1, 0, 0 };
                break;
            case "Elf (Wood)":
                boost = new int[] { 0, 2, 0, 0, 1, 0 };
                break;
            case "Gnome (Forest)":
                boost = new int[] { 0, 1, 0, 2, 0, 0 };
                break;
            case "Gnome (Rock)":
                boost = new int[] { 0, 0, 1, 2, 0, 0 };
                break;
            case "Halfling (Lightfoot)":
                boost = new int[] { 0, 2, 1, 0, 0, 0 };
                break;
            case "Half-Elf":
                //Player picks 2 attributes to add 1, hard-coded for now.
                boost = new int[] { 0, 1, 0, 1, 0, 2 };
                break;
            case "Half-Orc":
                boost = new int[] { 2, 0, 1, 0, 0, 0 };
                break;
            case "Human":
                boost = new int[] { 1, 1, 1, 1, 1, 1 };
                break;
            case "Tiefling":
                boost = new int[] { 0, 0, 0, 1, 0, 2 };
                break;
            default:
                boost = new int[] { 0, 0, 0, 0, 0, 0 };
        }

        for(int x = 0, x >= 5, x++)
        {
            boost[x] = baseScores[x] + boost[x];
        }
        return boost;
    }

    private int SavingThrow(int ability)
    {
        int result = Math.Floor(Random.Next(1, 21)) + AbilityMods[ability];

        if (Proficiencies.Contains(AbilityNames[ability] + " Saving Throws"))
        {
            result += ProfMod;
        }

        return result;
    }

}