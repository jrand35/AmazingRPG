using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character2Behavior : BattleBehavior
{
    public Character2Behavior(Battler parent)
    {
        Battler = parent;
        Name = "Steve";
        Stats = new Stats
        {
            MaxHP = 400,
            MaxSP = 400,
            CurrentHP = 200,
            CurrentSP = 400,
            Attack = 400,
            Defense = 400,
            SpAttack = 400,
            SpDefense = 400,
            Speed = 600,
            Luck = 20
        };
        SpecialAbilities = new List<Action>();
        SpecialAbilities.Add(new Restore(this));
    }

    public override IEnumerator StandardAttack(Battler user, Battler target)
    {
        yield return new WaitForSeconds(1f);
        int baseDamage = (user.BattleBehavior.Stats.Attack * 4) - (target.BattleBehavior.Stats.Defense * 2);
        baseDamage = new System.Random().Next((int)(baseDamage * 0.9), (int)(baseDamage * 1.1));
        target.BattleBehavior.TakeDamage(user, baseDamage);
    }

    class Restore : Action
    {
        public Restore(BattleBehavior parent)
        {
            BattleBehavior = parent;
            Name = "Restore";
            RequiredSP = 5;
            Power = 0;
            ActionTarget = ActionTarget.PartyMember;
        }

        public override IEnumerator Run(Battler user, Battler target)
        {
            yield return new WaitForSeconds(1f);
            target.BattleBehavior.Stats.CurrentHP += 20;
            Debug.Log(user.BattleBehavior.Name + " restored 20 HP to " + target.BattleBehavior.Name);
        }
    }
}
