using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HyperRatBehavior : BattleBehavior
{
    private Animator anim;
    public HyperRatBehavior(Battler parent)
    {
        Battler = parent;
        anim = parent.gameObject.GetComponent<Animator>();
    }

    public override void Initialize()
    {
        base.Initialize();
        Name = "Hyper Rat";
        Stats = new Stats
        {
            MaxHP = 900,
            MaxSP = 500,
            CurrentHP = 900,
            CurrentSP = 500,
            Attack = 23,
            Defense = 20,
            SpAttack = 30,
            SpDefense = 25,
            Speed = 32,
            Luck = 22
        };
        SpecialAbilities = new List<Action>();
        SpecialAbilities.Add(new ToxicBite(this));
    }

    public override Battler ChooseTarget(IList<Battler> characters)
    {
        //Do not attack dead characters
        IList<Battler> liveCharacters = characters.Where(b => b.BattleBehavior.Status.StatusEffect != StatusEffect.Defeated).ToList();
        int index = Random.Range(0, liveCharacters.Count);
        return liveCharacters[index];
    }

    public override IEnumerator StandardAttack(Battler user, Battler target)
    {
        anim.SetInteger("State", 1);
        Vector3 startPos = user.gameObject.transform.position;
        yield return Move.MoveInFrontOfBattler(user, target, startPos, new Vector3(2f, 0f, 0f), 30);
        anim.SetInteger("State", 2);
        int baseDamage = (user.BattleBehavior.Stats.Attack * 6) - (target.BattleBehavior.Stats.Defense * 3);
        if (baseDamage > 0)
            baseDamage = new System.Random().Next((int)(baseDamage * 0.9), (int)(baseDamage * 1.1));
        else
            baseDamage = 0;
        yield return new WaitForSeconds(0.5f);
        target.BattleBehavior.TakeDamage(user, baseDamage);
        yield return new WaitForSeconds(0.3f);
        anim.SetInteger("State", 1);
        yield return Move.MoveBackFromBattler(user, target, startPos, new Vector3(2f, 0f, 0f), 30);
        anim.SetInteger("State", 0);
    }

    class ToxicBite : Action
    {
        public ToxicBite(BattleBehavior parent)
        {
            BattleBehavior = parent;
            Name = "Toxic Bite";
            Description = "";
            RequiredSP = 10;
            Power = 1.3f;
            ActionTarget = ActionTarget.LivePartyMember;
        }

        public override IEnumerator Run(Battler user, Battler target, BattleController bc)
        {
            SpecialEffectsManager.SpecialName(Name);
            Animator anim = user.gameObject.GetComponent<Animator>();
            anim.SetInteger("State", 1);
            Vector3 startPos = user.gameObject.transform.position;
            yield return Move.MoveInFrontOfBattler(user, target, startPos, new Vector3(2f, 0f, 0f), 30);
            anim.SetInteger("State", 2);
            int baseDamage = (int)(user.BattleBehavior.Stats.SpAttack * Power * 6) - (target.BattleBehavior.Stats.SpDefense * 3);
            if (baseDamage > 0)
                baseDamage = new System.Random().Next((int)(baseDamage * 0.9), (int)(baseDamage * 1.1));
            else
                baseDamage = 0;
            yield return new WaitForSeconds(0.5f);
            target.BattleBehavior.TakeDamage(user, baseDamage);
            yield return new WaitForSeconds(0.3f);
            anim.SetInteger("State", 1);
            yield return Move.MoveBackFromBattler(user, target, startPos, new Vector3(2f, 0f, 0f), 30);
            anim.SetInteger("State", 0);
        }
    }
}
