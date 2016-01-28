using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleBehavior : MonoBehaviour {
    public event CharacterHPHandler HPChanged;
    public string Name { get; protected set; }
    public Stats Stats { get; set; }
    public IList<Action> SpecialAbilities { get; protected set; }
    protected Battler Battler { get; set; }
	// Use this for initialization
	void Start () {
	    
	}

    public virtual IEnumerator StandardAttack(Battler user, Battler target)
    {
        yield return 0;
    }

    public virtual void TakeDamage(Battler user, int baseDamage)
    {
        //Debug.Log("Took damage" + baseDamage.ToString());
        Stats.CurrentHP -= baseDamage;
        if (Stats.CurrentHP < 0)
        {
            Stats.CurrentHP = 0;
        }
        if (HPChanged != null)
        {
            HPChanged(Stats.CurrentHP, Stats.MaxHP);
        }
    }

    public virtual void RestoreHP(Battler user, int amount)
    {
        //Debug.Log("Restored HP");
        Stats.CurrentHP += amount;
        if (Stats.CurrentHP > Stats.MaxHP)
        {
            Stats.CurrentHP = Stats.MaxHP;
        }
        if (HPChanged != null)
        {
            HPChanged(Stats.CurrentHP, Stats.MaxHP);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
