using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Equipement;

[System.Serializable]

class Fighter : PlayerCharacter
{
    //[System.NonSerialized]
    public string SubClass, FightingStyle;

    protected override void Start()
    {
        base.Start();

        //The subclass and fighting style are hard-coded for now since they're the simplest options
        SubClass = "Champion";
        FightingStyle = "Defense";

        //A fighter uses a d10 to as a Hit dice
        HitDiceSize = 10;
        MaxHP = GenerateMaxHP(Lvl, HitDiceSize);

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

        ClassFeatures();
        SubclassFeatures(SubClass);
        FightingStyleFeatures(FightingStyle);

        EquipArmor(new Armor(this));
        EquipShield(new Shield(this));
        EquipWeapon(new Weapon(this));
    }

    private void ClassFeatures()
    {
        if (Lvl >= 5)
        {
            AttacksInTurn = 2;
        }
    }

    private void SubclassFeatures(string subclass)
    {
        //Starting equipement for a fighter, boosted to +1 to make it easier for the player. These are usually player choice, but hard-coded for simplicity
        if (eqArmor == null && eqShield == null && eqWeapon == null)
        {
            EquipArmor(new Armor("Chain Mail +1", "Chain Mail", 1, this));
            EquipShield(new Shield("Shield +1", "Shields", 1, this));
            EquipWeapon(new Weapon("Longsword +1", "Longswords", 1, this));
        }


        //Subclass features are only activated after lvl 3
        if (Lvl >= 3)
        {
            switch (subclass)
            {
                case "Champion":
                    //The Champion Subclass allows the player to Crit on a 19 or above instead of just a 20 (Making a crit a 10% chance)
                    RollToCrit = 19;
                    break;
            }
        }
    }

    private void FightingStyleFeatures(string style)
    {
        switch (style)
        {
            case "Archery":
                //+2 to hit with ranged weapons
                break;
            case "Defense":
                BaseAC += 1;
                break;
            case "Duelist":
                //When wielding only 1 one-handed melee weapon, +2 damage with that weapon
                break;
            case "Great Weapon Fighting":
                //When 2-handing a weapon and rolling a 1 or 2 on a damage die, you can re-roll those dice
                break;
            case "Protection":
                //When an ally within 5ft. is attacked, you can use your reaction to give the attacker disadvantage
                break;
            case "Two-Weapon Fighting":
                //When wielding 2 weapons, you can attack with the second weapon as a bonus action
                break;
        }
    }
}