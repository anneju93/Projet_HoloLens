using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Mini
{
    public string Name, Desc;
    public int DamageDice;
    public string DamageType;

    protected override void Start()
    {
        base.Start();

        //As DM, you can roll for monster HP (HP for a skeleton is 2d8+4), but it is simpler to (and in my game I also do this) just use the average
        HP = MaxHP;
        AC = BaseAC;

        MovesLeft = (int)(Speed / 5);
    }

    public override void Attack(Mini target)
    {
        //Hardcoded for testing, replace later
        int rollToHit = RollDice(20);
        int rollToDamage = RollDice(DamageDice);

        Debug.Log("Swing! Total to hit = " + (rollToHit + AtkMod));

        if (rollToHit != 1)
        {
            if (rollToHit < RollToCrit)
            {
                //Normal attack
                if (rollToHit + AtkMod >= target.AC)
                {
                    Debug.Log("Hit!");
                    target.Damage(rollToDamage + DmgMod, DamageType);
                }
                else
                {
                    //Miss
                    Debug.Log("Miss!");
                }
            }
            else
            {
                Debug.Log("Crit!");
                //Critical hit! Auto-Hit so AC doesn't matter, double the dice (or double dice result) but not modifiers
                target.Damage(rollToDamage * 2 + DmgMod, DamageType);
            }
        }
        else
        {
            Debug.Log("Oh no! Crit fail");
            //Crit fail, Auto-Miss
        }
    }
}
