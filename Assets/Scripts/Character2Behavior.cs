using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character2Behavior : BattleBehavior
{
    public Animator anim;
    public Character2Behavior(Battler parent)
    {
        Battler = parent;
    }

    public override void Initialize()
    {
        anim = Battler.GetComponent<Animator>();
        Name = "Steve";
        Stats = new Stats
        {
            MaxHP = 400,
            MaxSP = 400,
            CurrentHP = 400,
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
        Animator anim = user.GetComponent<Animator>();
        Vector3 startPos = user.gameObject.transform.position;
        anim.SetInteger("State", 1);
        yield return Move.MoveInFrontOfBattler(user, target, startPos, new Vector3(-2f, 0f, 0f));
        anim.SetInteger("State", 2);
        int baseDamage = (user.BattleBehavior.Stats.Attack * 4) - (target.BattleBehavior.Stats.Defense * 2);
        baseDamage = new System.Random().Next((int)(baseDamage * 0.9), (int)(baseDamage * 1.1));
        yield return new WaitForSeconds(0.5f);
        target.BattleBehavior.TakeDamage(user, baseDamage);
        yield return new WaitForSeconds(0.5f);
        anim.SetInteger("State", 1);
        yield return Move.MoveBackFromBattler(user, target, startPos, new Vector3(-2f, 0f, 0f));
        anim.SetInteger("State", 0);
    }

    class Restore : Action
    {
        public Restore(BattleBehavior parent)
        {
            BattleBehavior = parent;
            Name = "Restore";
            Description = "Restores 100 HP to a party member.";
            RequiredSP = 5;
            Power = 0;
            ActionTarget = ActionTarget.PartyMember;
        }

        public override IEnumerator Run(Battler user, Battler target)
        {
            yield return new WaitForSeconds(1f);
            target.BattleBehavior.RestoreHP(user, 100);
            Debug.Log(user.BattleBehavior.Name + " restored 100 HP to " + target.BattleBehavior.Name);
        }
    }
}
