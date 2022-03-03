using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Equipement;

[System.Serializable]

public class PlayerCharacter : Mini
{
    public int Lvl, HitDiceLeft;
    public int[] AbilityScores;
    public string Name, Race;
    public List<string> Proficiencies;

    [System.NonSerialized]
    public int ProfMod, AttacksInTurn, HitDiceSize;
    [System.NonSerialized]
    public int[] AbilityMods;
    [System.NonSerialized]
    public string[] AbilityNames = new string[] { "STR", "DEX", "CON", "INT", "WIS", "CHA" };

    public Weapon eqWeapon;
    public Armor eqArmor;
    public Shield eqShield;

    protected override void Start()
    {
        base.Start();

        AbilityScores = GetRacialBoost(Race, AbilityScores);
        AbilityMods = GetModifiers(AbilityScores);
        ProfMod = GetProficiencyMod(Lvl);

        MovesLeft = (int)(Speed / 5);

        //BaseAC is 10 + DEX modifier
        BaseAC = 10 + AbilityMods[1];
        AC = BaseAC;

        //The maximum number of hit dice is determined by the lvl
        HitDiceLeft = Lvl;

        //These values are determined by the class, these are just default values
        HitDiceSize = 8;
        MaxHP = GenerateMaxHP(Lvl, HitDiceSize);
        HP = MaxHP;
        AttacksInTurn = 1;

        //Equipping at Start() doesn't let the proficiencies list from Fighter load properly, making the player non-proficient with everything
    }

    protected int GenerateMaxHP(int lvl, int hitDiceSize)
    {
        int hpRoll = 0;

        //To generate Max Hp, we roll a number of hit dice equal to lvl - 1 and add the result to the maximum of 1 hit die and the player's CON modifier * the lvl
        //ex. for a lvl 5 fighter (d10 hit dice): 4d10 + 10 + CON 
        for (int x = 0; x < lvl - 1; x++)
        {
            hpRoll += RollDice(hitDiceSize);
        }

        //Minimum of 1 HP so a player with low CON can't lose HP when leveling up
        int conBonus = AbilityMods[2];
        if(conBonus < 1)
        {
            conBonus = 1;
        }

        return HitDiceSize + hpRoll + (lvl * conBonus);
    }

    private int[] GetModifiers(int[] scores)
    {
        int[] mods = new int[6];

        for (int x = 0; x <= 5; x++)
        {
            mods[x] = (int)Mathf.Floor((scores[x] - 10) / 2f);
        }
        return mods;
    }

    private int GetProficiencyMod(int lvl)
    {
        int mod = (int)(Mathf.Ceil(lvl / 4f) + 1);
        return mod;
    }

    private int SavingThrow(int ability)
    {
        int result = RollDice(20) + AbilityMods[ability];

        if (Proficiencies.Contains(AbilityNames[ability] + " Saving Throws"))
        {
            result += ProfMod;
        }

        return result;
    }

    public override void Attack(Mini target)
    {
        //Make player roll d20
        int rollToHit = RollDice(20);
        int rollToDamage, atkDmg;


        if (rollToHit != 1)
        {
            if (rollToHit < RollToCrit)
            {
                //Normal attack
                //AtkMod is set when equipping a weapon, based on the player's proficiency with the weapon and their STR Mod
                if (rollToHit + AtkMod >= target.AC)
                {
                    Debug.Log("Hit!");
                    //Damage is your weapon's damage die (ex. d8) + your STR modifier
                    //Make player roll weapon damage dice
                    rollToDamage = RollDice(eqWeapon.damageDice);

                    atkDmg = rollToDamage + AbilityMods[0] + eqWeapon.magicLvl;
                    
                    //You cannot deal negative damage to an enemy, so we check the damage
                    target.Damage(atkDmg < 0 ? 0 : atkDmg, eqWeapon.damageType);
                }
                else
                {
                    Debug.Log("Miss!");
                }
            }
            else
            {
                Debug.Log("Crit!");
                //Crit, Auto-Hit so AC doesn't matter, double the dice (or double dice result) but not modifiers
                //Make player roll weapon damage dice
                rollToDamage = RollDice(eqWeapon.damageDice);

                atkDmg = rollToDamage * 2 + AbilityMods[0];
                target.Damage(atkDmg < 0 ? 0 : atkDmg, eqWeapon.damageType);
            }
        }
        else
        {
            Debug.Log("Oh no! Crit fail");
            //Crit fail, Auto-Miss
        }
    }

    internal override void Die()
    {
        //Game Over
        gridScript.RemoveMiniFromCell(gameObject);
        Destroy(gameObject);
    }

#region Equip/Unequip
    public void EquipArmor(Armor newArmor)
    {
        if (eqArmor != null)
        {
            UnequipArmor();
        }
        eqArmor = newArmor;
        AC = newArmor.newAC;

        if (!newArmor.proficient)
        {
            //A character that wears armor he is not proficient with has his speed reduced and cannot cast spells
            Speed -= 10;
        }
    }

    private void UnequipArmor()
    {
        if (!eqArmor.proficient)
        {
            //Removes the penalty from wearing armor you don't have profciency in
            Speed += 10;
        }

        AC = BaseAC;
        eqArmor = null;

        if (eqShield != null)
        {
            AC += eqShield.acBoost;
        }

    }

    public void EquipShield(Shield newShield)
    {
        if (eqShield != null)
        {
            UnequipShield();
        }
        eqShield = newShield;
        AC += newShield.acBoost;
    }

    private void UnequipShield()
    {
        if (eqShield != null)
        {
            AC -= eqShield.acBoost;
            eqShield = null;
        }
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        if (eqWeapon != null)
        {
            UnequipWeapon();
        }
        eqWeapon = newWeapon;
        
        if(newWeapon.proficient){
            AtkMod = ProfMod + AbilityMods[0] + newWeapon.magicLvl;
        }
        else AtkMod = AbilityMods[0] + newWeapon.magicLvl;
    }

    private void UnequipWeapon()
    {
        if (eqWeapon != null)
        {
            //You can still do damage without a weapon, so we "equip" fists
            eqWeapon = new Weapon(this);
        }

        //Fists are always proficient and never have a magic level
        AtkMod = ProfMod + AbilityMods[0];
    }
    #endregion
    
    private int[] GetRacialBoost(string race, int[] baseScores)
    {
        int[] boost;
        switch (race)
        {
            case "Dwarf (Hill)":
                Speed = 25;
                boost = new int[] { 0, 0, 2, 0, 1, 0 };
                break;
            case "Dwarf (Mountain)":
                Speed = 25;
                boost = new int[] { 2, 0, 2, 0, 0, 0 };
                break;
            case "Dragonborn":
                boost = new int[] { 2, 0, 0, 0, 0, 1 };
                break;
            case "Elf (Drow)":
                boost = new int[] { 0, 2, 0, 0, 0, 1 };
                break;
            case "Elf (High)":
                boost = new int[] { 0, 2, 0, 1, 0, 0 };
                break;
            case "Elf (Wood)":
                boost = new int[] { 0, 2, 0, 0, 1, 0 };
                break;
            case "Gnome (Forest)":
                Speed = 25;
                boost = new int[] { 0, 1, 0, 2, 0, 0 };
                break;
            case "Gnome (Rock)":
                Speed = 25;
                boost = new int[] { 0, 0, 1, 2, 0, 0 };
                break;
            case "Halfling (Lightfoot)":
                Speed = 25;
                boost = new int[] { 0, 2, 0, 0, 0, 1 };
                break;
            case "Halfling (Stout)":
                Speed = 25;
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
                break;
        }

        for (int x = 0; x <= 5; x++)
        {
            boost[x] += baseScores[x];
        }

        return boost;
    }
}