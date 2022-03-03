using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mini : MonoBehaviour
{
    public int BaseAC, MaxHP, AC, HP, Speed, MovesLeft, AtkMod, DmgMod;
    public List<string> Immunities, Resistances, Vulnerabilities;

    private GameObject grid;
    [System.NonSerialized]
    public MapGrid gridScript;
    [System.NonSerialized]
    public Vector3 initialPosition = new Vector3(1.25f, 0, 1.25f);
    [System.NonSerialized]
    public int RollToCrit;
    [System.NonSerialized]
    public List<GameObject> MeleeTargets;

    protected virtual void Start()
    {
        grid = GameObject.FindWithTag("MapGrid");
        gridScript = (MapGrid)grid.GetComponent(typeof(MapGrid));

        //A Crit is when you roll a 20 on a d20 (The main dice of normal gameplay), however, some monsters and players can crit on lower numbers as well (See Champion sub-class)
        RollToCrit = 20;
    }

    public void Grabbed()
    {
        initialPosition = this.transform.position;
        MeleeTargets = new List<GameObject>();
        gridScript.GenerateMoveMap(gameObject);
    }

    public void Dropped()
    {
        gridScript.SnapToGrid(gameObject);
        if(initialPosition != this.transform.position)
        {
            CheckForTargets();
        }

        //Pour tester, retirer plus tard
        MovesLeft = (int)(Speed / 5);
    }

    private void CheckForTargets()
    {
        List<GameObject> targets = new List<GameObject>();
        MeleeTargets = new List<GameObject>();

        Debug.Log("Searching");
        targets = gridScript.GetMeleeTargets(gameObject);
        Debug.Log(targets.Count);
        foreach (GameObject target in targets)
        {
            if (target.tag == "Monster" && gameObject.tag == "Player")
            {
                Debug.Log("Found Monster");
                MeleeTargets.Add(target);
            }
            if (target.tag == "Player" && gameObject.tag == "Monster")
            {
                Debug.Log("Found Player");
                Mini playerScript = (Mini)target.GetComponent(typeof(Mini));
                Attack(playerScript);
            }
        }
    }

    public virtual void Attack(Mini target)
    {
        //Hardcoded for testing, replace later
        int rollToHit = RollDice(20);
        int rollToDamage = RollDice(6);

        Debug.Log("Using mini attack");
        if (rollToHit != 1)
        {
            if(rollToHit < RollToCrit)
            {
                //Normal hit
                if (rollToHit + AtkMod >= target.AC)
                {
                    target.Damage(rollToDamage + DmgMod, "Generic");
                }
            }
            else
            {
                //Critical hit! Auto-Hit so AC doesn't matter, double the dice (or double dice result) but not modifiers
                target.Damage(rollToDamage * 2 + DmgMod, "Generic");
            }
        }
        else
        {
            //Crit fail, Auto-Miss
        }
    }

    public void Damage(int damage, string dmgType)
    {
        if (Immunities.Contains(dmgType))
        {
            //You are immune to the damage taken, therefore you don't take any damage
            damage = 0;
            //*Plink* sound to indicate immunity
        }

        if (Resistances.Contains(dmgType))
        {
            //You are resistant to the damage taken, halving the damage, rounded down
            damage = (int)(Mathf.Floor(damage / 2));
            //Sound to show resistance?
        }

        if (Vulnerabilities.Contains(dmgType))
        {
            //You are vulnerable to the damage taken, doubling the damage
            damage = damage * 2;
            //*Crunch* sound to indicate extra damage
        }

        Debug.Log("Ouch");
        HP -= damage;
        //Play "Hurt" Sound

        if(HP <= 0)
        {
            Die();
        }
    }

    internal virtual void Die()
    {
        gridScript.RemoveMiniFromCell(gameObject);
        Destroy(gameObject);
    }

    internal int RollDice(int diceSize)
    {
        return (int)(Mathf.Floor(Random.Range(1, diceSize + .99f)));
    }
}
