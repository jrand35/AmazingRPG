﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SteveBehavior : BattleBehavior
{
    private bool defending;
    public Animator anim;
    public SteveBehavior(Battler parent)
    {
        Battler = parent;
    }

    public override bool Defending
    {
        get
        {
            return defending;
        }
        set
        {
            defending = value;
            anim.SetBool("Defending", value);
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        defending = false;
        anim = Battler.GetComponent<Animator>();
        Name = "Steve";
        Stats = new Stats
        {
            MaxHP = 400,
            MaxSP = 230,
            CurrentHP = 400,
            CurrentSP = 230,
            Attack = 23,
            Defense = 20,
            SpAttack = 25,
            SpDefense = 25,
            Speed = 30,
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
        yield return Move.MoveInFrontOfBattler(user, target, startPos, new Vector3(-2f, 0f, 0f), 50);
        anim.SetInteger("State", 2);
        int baseDamage = (user.BattleBehavior.Stats.Attack * 6) - (target.BattleBehavior.Stats.Defense * 3);
        baseDamage = new System.Random().Next((int)(baseDamage * 0.9), (int)(baseDamage * 1.1));
        yield return new WaitForSeconds(0.5f);
        target.BattleBehavior.TakeDamage(user, baseDamage);
        yield return new WaitForSeconds(0.3f);
        anim.SetInteger("State", 1);
        yield return Move.MoveBackFromBattler(user, target, startPos, new Vector3(-2f, 0f, 0f), 50);
        anim.SetInteger("State", 0);
    }

    public override void TakeDamage(Battler user, int baseDamage)
    {
        base.TakeDamage(user, baseDamage);
        if (!defending)
        {
            Battler.StartCoroutine(Anim());
        }
    }

    IEnumerator Anim()
    {
        anim.SetInteger("State", 10);
        yield return 0;
        anim.SetInteger("State", 0);
    }

    public override void Victory()
    {
        anim.SetBool("Win", true);
    }

    class Restore : Action
    {
        public Restore(BattleBehavior parent)
        {
            BattleBehavior = parent;
            Name = "Super Restore";
            Description = "Restores 250 HP to a party member.";
            RequiredSP = 20;
            Power = 0;
            ActionTarget = ActionTarget.LivePartyMember;
        }

        public override IEnumerator Run(Battler user, Battler target, IList<Battler> allCharacters, IList<Battler> allEnemies, BattleController bc)
        {
            Animator anim = user.GetComponent<Animator>();
            anim.SetInteger("State", 3);
            yield return new WaitForSeconds(1.4f);
            int duration = 40;
            Material m = target.GetComponentInChildren<Renderer>().material;
            m.EnableKeyword("_EMISSION");

            SpecialEffectsManager.RestoreParticles(target);
            for (int i = 0; i < duration; i++)
            {
                m.SetColor("_EmissionColor", Color.white * 0.6f * Mathf.Sin((float) i / duration * Mathf.PI));
                yield return 0;
            }
            target.BattleBehavior.RestoreHP(user, 250);
            anim.SetInteger("State", 0);
            Debug.Log(user.BattleBehavior.Name + " restored 250 HP to " + target.BattleBehavior.Name);
        }
    }
}
