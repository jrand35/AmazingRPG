using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TrumpBehavior : BattleBehavior
{
    private GameObject WallPrefab;
    private Animator anim;
    public TrumpBehavior(Battler parent)
    {
        WallPrefab = Resources.Load<GameObject>("Wall");
        Battler = parent;
        anim = parent.gameObject.GetComponent<Animator>();
    }

    public override void Initialize()
    {
        base.Initialize();
        Name = "Donald Trump";
        Stats = new Stats()
        {
            MaxHP = 1500,
            MaxSP = 1000,
            CurrentHP = 1500,
            CurrentSP = 1000,
            Attack = 25,
            Defense = 22,
            SpAttack = 26,
            SpDefense = 20,
            Speed = 30,
            Luck = 20
        };
        SpecialAbilities = new List<Action>();
        SpecialAbilities.Add(new GreatWallOfMexico(this, WallPrefab));
    }

    public override TurnArgs ChooseAttack(IList<Battler> allCharacters)
    {
        IList<Battler> liveCharacters = allCharacters.Where(b => b.BattleBehavior.Status.StatusEffect != StatusEffect.Defeated).ToList();
        int index = UnityEngine.Random.Range(0, liveCharacters.Count);
        Battler target = liveCharacters[index];
        float attackType = UnityEngine.Random.Range(0f, 1f);
        //Choose to use a normal attack or a special attack
        //ActionType actionType = attackType < 0.75f ? ActionType.Attack : ActionType.Special;
        ActionType actionType = ActionType.Special;
        int specialIndex = 0;
        ActionTarget actionTarget = ActionTarget.PartyMember;
        if (actionType == ActionType.Special)
        {
            //Check to see if you have enough SP
            IList<Action> usableAbilities = SpecialAbilities.Where(a => Stats.CurrentSP >= a.RequiredSP).ToList();
            if (usableAbilities.Count == 0)
            {
                actionType = ActionType.Attack;
            }
            //Pick a special attack to use out of all the usable attacks, then get its index in the SpecialAbilities list
            else
            {
                specialIndex = UnityEngine.Random.Range(0, usableAbilities.Count);
                Action chosenAction = usableAbilities[specialIndex];
                specialIndex = SpecialAbilities.IndexOf(chosenAction);
                //Possibly check to see if an attack can be used on a particular character
                actionTarget = SpecialAbilities[specialIndex].ActionTarget;
            }
        }
        TurnArgs args = new TurnArgs
        {
            User = Battler,
            Target = target,
            ActionTarget = actionTarget,
            ActionIndex = specialIndex,
            ActionType = actionType,
        };
        return args;
    }

    public override IEnumerator StandardAttack(Battler user, Battler target)
    {
        anim.SetInteger("State", 1);
        Vector3 startPos = user.gameObject.transform.position;
        yield return Move.MoveInFrontOfBattler(user, target, startPos, new Vector3(2f, 0f, 0f), 70);
        anim.SetInteger("State", 2);
        int baseDamage = (user.BattleBehavior.Stats.Attack * 6) - (target.BattleBehavior.Stats.Defense * 3);
        if (baseDamage > 0)
            baseDamage = new System.Random().Next((int)(baseDamage * 0.9), (int)(baseDamage * 1.1));
        else
            baseDamage = 0;
        yield return new WaitForSeconds(0.9f);
        target.BattleBehavior.TakeDamage(user, baseDamage);
        yield return new WaitForSeconds(0.3f);
        anim.SetInteger("State", 1);
        yield return Move.MoveBackFromBattler(user, target, startPos, new Vector3(2f, 0f, 0f), 70);
        anim.SetInteger("State", 0);
    }

    class GreatWallOfMexico : Action
    {
        GameObject WallPrefab;
        Vector3 cameraPos;
        public GreatWallOfMexico(BattleBehavior parent, GameObject wallPrefab)
        {
            cameraPos = Camera.main.transform.position;
            WallPrefab = wallPrefab;
            BattleBehavior = parent;
            Name = "Great Wall of Mexico";
            Description = "";
            RequiredSP = 15;
            Power = 2f;
            ActionTarget = ActionTarget.LivePartyMember;
        }

        IEnumerator CameraShake()
        {
            float maxAmpl = 0.5f;
            float ampl = maxAmpl;
            int duration1 = 70;
            int duration2 = 40;
            for (int i = duration1; i >= 0; i--)
            {
                Camera.main.transform.position = cameraPos + Random.insideUnitSphere * ampl;
                yield return 0;
            }
            for (int i = duration2; i >= 0; i--)
            {
                ampl = maxAmpl * (float)i / duration2;
                Camera.main.transform.position = cameraPos + Random.insideUnitSphere * ampl;
                yield return 0;
            }

            Camera.main.transform.position = cameraPos;
        }

        IEnumerator MakeWalls(Battler target)
        {
            int count = 15;
            for (int i = 0; i < count; i++)
            {
                Vector3 pos = target.gameObject.transform.position;
                pos.x = 20 - (i * 3);
                GameObject wall = Instantiate(WallPrefab, pos, Quaternion.identity) as GameObject;
                yield return new WaitForSeconds(0.07f);
            }
        }

        public override IEnumerator Run(Battler user, Battler target, BattleController bc)
        {
            SpecialEffectsManager.SpecialName(Name);
            Animator anim = user.gameObject.GetComponent<Animator>();
            anim.SetInteger("State", 3);
            yield return new WaitForSeconds(2f);
            bc.StartCoroutine(CameraShake());
            yield return MakeWalls(target);
            yield return new WaitForSeconds(0.75f);
            anim.SetInteger("State", 0);
            int baseDamage = (int)(user.BattleBehavior.Stats.SpAttack * Power * 6) - (target.BattleBehavior.Stats.SpDefense * 3);
            if (baseDamage > 0)
                baseDamage = new System.Random().Next((int)(baseDamage * 0.9), (int)(baseDamage * 1.1));
            else
                baseDamage = 0;
            target.BattleBehavior.TakeDamage(user, baseDamage);
        }
    }
}
