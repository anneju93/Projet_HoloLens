using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Mini
{
    public int AC, MaxHP, HP, AtkMod;
    public string Name, Desc;

    public Monster(int baseAC, int hpMax, int spd, int attMod, string creatureName, string description)
    {
        Name = creatureName;
        Desc = description;
        AC = baseAC;
        MaxHP = hpMax;
        HP = MaxHP;
        Speed = spd;
        MoveLeft = Speed;
        AtkMod = attMod;
    }
}
