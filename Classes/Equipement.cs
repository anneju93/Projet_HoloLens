using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipement
{
    public string name, type;
    public int magicLvl;
    public bool proficient;
    PlayerCharacter player;

    public Equipement(PlayerCharacter usedBy)
    {
        name = "";
        type = "";
        magicLvl = 0;
        proficient = false;
        player = usedBy;
    }
    public Equipement(string eqName, string eqType, int eqMagicLvl, PlayerCharacter usedBy)
    {
        name = eqName;
        type = eqType;
        magicLvl = eqMagicLvl;
        proficient = false;
        player = usedBy;
    }

    //##################################################

    public class Armor : Equipement
    {
        public string armorClass;
        public int newAC;

        public Armor(PlayerCharacter usedBy) : base(usedBy)
        {
            name = "Chain Mail";
            type = "Chain Mail";
            armorClass = GetArmorClass(type);
            magicLvl = 0;
            newAC = GetNewAC(type);
            proficient = isPlayerProficient();
        }

        public Armor(string armorName, string armorType, int mLvl, PlayerCharacter usedBy) : base(armorName, armorType, mLvl, usedBy)
        {
            armorClass = GetArmorClass(type);
            newAC = GetNewAC(type);
            proficient = isPlayerProficient();
        }

        private bool isPlayerProficient()
        {
            //For armors, proficiency is based on armor class (Light, Medium or Heavy) instead of individual types
            if (player.Proficiencies.Contains(armorClass) || player.Proficiencies.Contains("All Armors"))
            {
                return true;
            }
            else return false;
        }

        private string GetArmorClass(string aType)
        {
            switch (aType)
            {
                case "Padded":
                case "Leather":
                case "Studded Leather":
                    return "Light Armors";
                case "Hide":
                case "Chain Shirt":
                case "Scale Mail":
                case "Breastplate":
                case "Half Plate":
                    return "Medium Armors";
                case "Ring Mail":
                case "Chain Mail":
                case "Splint":
                case "Plate":
                    return "Heavy Armors";
                default:
                    return "Impovised Armors";
            }
        }

        private int GetNewAC(string aType)
        {
            int dexMod = player.AbilityMods[1];
            switch (aType)
            {
                //Light armors don't have a maximum bonus from your DEX modifier, which makes them ideal for high-DEX characters, such as rogues and rangers
                case "Padded":
                case "Leather":
                    return 11 + dexMod;
                case "Studded Leather":
                    return 12 + dexMod;

                //Medium armors have a maximum DEX modifier of +2, so even if your character has +4 DEX, your bonus to AC is still +2
                case "Hide":
                    if (dexMod <= 1)
                    {
                        return 12 + dexMod;
                    }
                    else return 12 + 2;
                case "Chain Shirt":
                    if (dexMod <= 1)
                    {
                        return 13 + dexMod;
                    }
                    else return 13 + 2;
                case "Scale Mail":
                case "Breastplate":
                    if (dexMod <= 1)
                    {
                        return 14 + dexMod;
                    }
                    else return 14 + 2;
                case "Half Plate":
                    if (dexMod <= 1)
                    {
                        return 15 + dexMod;
                    }
                    else return 15 + 2;

                //Heavy armors don't get a bonus from your DEX modifier, which is useful if you have a negative modifier
                case "Ring Mail":
                    return 14;
                case "Chain Mail":
                    return 16;
                case "Splint":
                    return 17;
                case "Plate":
                    return 18;
                default:
                    return player.BaseAC;
            }
        }
    }

    public class Shield : Equipement
    {
        public int acBoost;

        public Shield(PlayerCharacter usedBy) : base(usedBy)
        {
            name = "Shield";
            type = "Shields";
            magicLvl = 0;
            acBoost = GetACBoost();
            proficient = isPlayerProficient();
        }

        public Shield(string sName, string sType, int mLvl, PlayerCharacter usedBy) : base(sName, sType, mLvl, usedBy)
        {
            acBoost = GetACBoost();
            proficient = isPlayerProficient();
        }

        public int GetACBoost()
        {
            return 2 + magicLvl;
        }

        private bool isPlayerProficient()
        {
            //Shield proficiencies are not taken into account since it only affects spellcasting, which isn't included in this project
            if (player.Proficiencies.Contains("Shields"))
            {
                return true;
            }
            else return false;
        }
    }

    public class Weapon : Equipement
    {
        public string weaponClass, damageType;
        public int damageDice;

        //By default, most melee weapons use a character's STR modifier for attack and damage rolls
        //Some types of weapons with the "Finesse" attribute can choose to use DEX instead, but for simplicity we won't include it and will use STR for everything

        public Weapon(PlayerCharacter usedBy) : base(usedBy)
        {
            name = "Unarmed Strike";
            type = "Fists";
            weaponClass = GetWeaponClass(type);
            damageType = GetDamageType(type);
            magicLvl = 0;
            proficient = true;
            damageDice = GetDamageDie(type);
            player = usedBy;
        }

        public Weapon(string wName, string wType, int mLvl, PlayerCharacter usedBy) : base(wName, wType, mLvl, usedBy)
        {
            weaponClass = GetWeaponClass(wType);
            proficient = IsPlayerProficient();
            damageDice = GetDamageDie(type);
        }

        private bool IsPlayerProficient()
        {
            //For weapons, proficiency is either with individual weapon types or a whole class (Simple or Martial weapons)
            if (player.Proficiencies.Contains(type) || player.Proficiencies.Contains(weaponClass))
            {
                return true;
            }
            else return false;
        }

        private string GetDamageType(string wType)
        {
            switch (wType)
            {
                case "Fists":
                case "Clubs":
                case "Greatclubs":
                case "Light Hammers":
                case "Maces":
                case "Quarterstaffs":
                case "Flails":
                case "Mauls":
                case "Warhammers":
                    return "Bludgeoning";
                case "Daggers":
                case "Javelins":
                case "Spears":
                case "Lances":
                case "Morningstars":
                case "Pikes":
                case "Rapiers":
                case "Shortswords":
                case "Tridents":
                case "Warpicks":
                    return "Piercing";
                case "Handaxes":
                case "Sickles":
                case "Battleaxes":
                case "Glaives":
                case "Greataxes":
                case "Greatswords":
                case "Halberds":
                case "Longswords":
                case "Scimitars":
                case "Whips":
                    return "Piercing";
                default: return "Generic";
            }
        }

        private string GetWeaponClass(string wType)
        {
            switch (wType)
            {
                case "Clubs":
                case "Daggers":
                case "Greatclubs":
                case "Handaxes":
                case "Javelins":
                case "Light Hammers":
                case "Maces":
                case "Quarterstaffs":
                case "Sickles":
                case "Spears":
                    return "Simple Weapons";
                case "Battleaxes":
                case "Flails":
                case "Glaives":
                case "Greataxes":
                case "Greatswords":
                case "Halberds":
                case "Lances":
                case "Longswords":
                case "Mauls":
                case "Morningstars":
                case "Pikes":
                case "Rapiers":
                case "Scimitars":
                case "Shortswords":
                case "Tridents":
                case "Warpicks":
                case "Warhammers":
                case "Whips":
                    return "Simple Weapons";
                case "Fists":
                    return "Unarmed";
                default:
                    //Non-conventional weapons, such as a table leg ripped of during a bar fight, are considered improvised weapons
                    return "Improvised Weapons";
            }
        }

        private int GetDamageDie(string wType)
        {
            switch (wType)
            {
                case "Fists":
                    //An unarmed strike deals 1 + STR Mod, so you don't get a dice
                    return 1;
                case "Clubs":
                case "Daggers":
                case "Light Hammers":
                case "Sickles":
                case "Whips":
                    //These weapons deal 1d4 damage
                    return 4;
                case "Handaxes":
                case "Javelins":
                case "Maces":
                case "Quarterstaffs":
                case "Spears":
                case "Scimitars":
                case "Shortswords":
                case "Tridents":
                    //These weapons deal 1d6 damage
                    return 6;
                case "Greatclubs":
                case "Battleaxes":
                case "Flails":
                case "Longswords":
                case "Morningstars":
                case "Rapiers":
                case "Warpicks":
                case "Warhammers":
                    //These weapons deal 1d8 damage
                    return 8;
                case "Glaives":
                case "Halberds":
                case "Pikes":
                    //These weapons deal 1d10 damage
                    return 10;
                case "Greataxes":
                case "Greatswords":
                case "Lances":
                case "Mauls":
                    //These weapons deal 1d12 damage
                    return 12;
                default:
                    return 4;
            }
        }
    }
}
