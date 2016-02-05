using UnityEngine;
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
            MaxSP = 400,
            CurrentHP = 400,
            CurrentSP = 400,
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
        yield return Move.MoveInFrontOfBattler(user, target, startPos, new Vector3(-2f, 0f, 0f));
        anim.SetInteger("State", 2);
        int baseDamage = (user.BattleBehavior.Stats.Attack * 6) - (target.BattleBehavior.Stats.Defense * 3);
        baseDamage = new System.Random().Next((int)(baseDamage * 0.9), (int)(baseDamage * 1.1));
        yield return new WaitForSeconds(0.5f);
        target.BattleBehavior.TakeDamage(user, baseDamage);
        yield return new WaitForSeconds(0.3f);
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
            target.BattleBehavior.RestoreHP(user, 100);
            anim.SetInteger("State", 0);
            Debug.Log(user.BattleBehavior.Name + " restored 100 HP to " + target.BattleBehavior.Name);
        }
    }
}
