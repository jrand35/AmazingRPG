using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBehavior : BattleBehavior
{
    public EnemyBehavior(Battler parent)
    {
        Battler = parent;
    }

    public override void Initialize()
    {
        Name = "Block";
        Stats = new Stats
        {
            MaxHP = 300,
            MaxSP = 300,
            CurrentHP = 300,
            CurrentSP = 300,
            Attack = 300,
            Defense = 300,
            SpAttack = 300,
            SpDefense = 300,
            Speed = 300,
            Luck = 20
        };
        SpecialAbilities = new List<Action>();
        //SpecialAbilities.Add(new Restore(this));
    }

    public override IEnumerator StandardAttack(Battler user, Battler target)
    {
        yield return new WaitForSeconds(1f);
        int baseDamage = (user.BattleBehavior.Stats.Attack * 4) - (target.BattleBehavior.Stats.Defense * 2);
        if (baseDamage > 0)
            baseDamage = new System.Random().Next((int)(baseDamage * 0.9), (int)(baseDamage * 1.1));
        else
            baseDamage = 0;
        target.BattleBehavior.TakeDamage(user, baseDamage);
    }
}
