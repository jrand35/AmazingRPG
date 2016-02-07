using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Action : MonoBehaviour {
    protected BattleBehavior BattleBehavior { get; set; }
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public int RequiredSP { get; protected set; }
    public float Power { get; protected set; }
    public ActionTarget ActionTarget { get; protected set; }
    public virtual bool CanRun
    {
        get
        {
            return BattleBehavior.Stats.CurrentSP >= RequiredSP;
        }
    }
    public abstract IEnumerator Run(Battler user, Battler target, IList<Battler> allCharacters, IList<Battler> allEnemies, BattleController battlecontroller);
    //Implement a BattleBehavior.UseSP function
    public void UseSP(Battler user)
    {
        user.BattleBehavior.Stats.CurrentSP -= RequiredSP;
    }
}
