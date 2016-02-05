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
    }

    public override void ChooseTarget(IList<Battler> characters)
    {
        //Do not attack dead characters
        IList<Battler> liveCharacters = characters.Where(b => b.BattleBehavior.Status.StatusEffect != StatusEffect.Defeated).ToList();
        int index = Random.Range(0, liveCharacters.Count);
        Target = liveCharacters[index];
    }

    public override IEnumerator StandardAttack(Battler user, Battler target)
    {
        anim.SetInteger("State", 1);
        Vector3 startPos = user.gameObject.transform.position;
        yield return Move.MoveInFrontOfBattler(user, Target, startPos, new Vector3(2f, 0f, 0f), 30);
        anim.SetInteger("State", 2);
        int baseDamage = (user.BattleBehavior.Stats.Attack * 6) - (Target.BattleBehavior.Stats.Defense * 3);
        if (baseDamage > 0)
            baseDamage = new System.Random().Next((int)(baseDamage * 0.9), (int)(baseDamage * 1.1));
        else
            baseDamage = 0;
        yield return new WaitForSeconds(0.5f);
        Target.BattleBehavior.TakeDamage(user, baseDamage);
        yield return new WaitForSeconds(0.3f);
        anim.SetInteger("State", 1);
        yield return Move.MoveBackFromBattler(user, Target, startPos, new Vector3(2f, 0f, 0f), 30);
        anim.SetInteger("State", 0);
    }
}
