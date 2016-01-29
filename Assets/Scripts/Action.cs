﻿using UnityEngine;
using System.Collections;

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
    public abstract IEnumerator Run(Battler user, Battler target);
}
