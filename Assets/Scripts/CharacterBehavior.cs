using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterBehavior : BattleBehavior
{
    public CharacterBehavior(Battler parent)
    {
        Battler = parent;
        Name = "Character";
        Stats = new Stats
        {
            MaxHP = 500,
            MaxSP = 500,
            CurrentHP = 500,
            CurrentSP = 500,
            Attack = 500,
            Defense = 500,
            SpAttack = 500,
            SpDefense = 500,
            Speed = 500,
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
