using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBehavior : BattleBehavior
{
    public EnemyBehavior(Battler parent)
    {
        Battler = parent;
        Name = "Enemy";
        Stats = new Stats
        {
            MaxHP = 400,
            MaxSP = 400,
            CurrentHP = 400,
            CurrentSP = 400,
            Attack = 20,
            Defense = 20,
            SpAttack = 400,
            SpDefense = 400,
            Speed = 400,
            Luck = 10
        };
        SpecialAbilities = new List<Action>();
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
