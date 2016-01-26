using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleBehavior : MonoBehaviour {
    public string Name { get; protected set; }
    public Stats Stats { get; protected set; }
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
        Stats.CurrentHP -= baseDamage;
    }

    public virtual void RestoreHP(Battler user, int amount)
    {

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
