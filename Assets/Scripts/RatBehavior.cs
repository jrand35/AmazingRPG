using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RatBehavior : BattleBehavior
{
    private Animator anim;
    public RatBehavior(Battler parent)
    {
        Battler = parent;
        anim = parent.gameObject.GetComponent<Animator>();
    }

    public override void Initialize()
    {
        base.Initialize();
        Name = "Rat";
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
        yield return Move.MoveInFrontOfBattler(user, Target, startPos, new Vector3(2f, 0f, 0f));
        anim.SetInteger("State", 2);
        int baseDamage = (user.BattleBehavior.Stats.Attack * 4) - (Target.BattleBehavior.Stats.Defense * 2);
        if (baseDamage > 0)
            baseDamage = new System.Random().Next((int)(baseDamage * 0.9), (int)(baseDamage * 1.1));
        else
            baseDamage = 0;
        yield return new WaitForSeconds(0.5f);
        Target.BattleBehavior.TakeDamage(user, baseDamage);
        yield return new WaitForSeconds(0.3f);
        anim.SetInteger("State", 1);
        yield return Move.MoveBackFromBattler(user, Target, startPos, new Vector3(2f, 0f, 0f));
        anim.SetInteger("State", 0);
    }
}
