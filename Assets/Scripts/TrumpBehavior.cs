using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TrumpBehavior : BattleBehavior
{
    private GameObject WallPrefab;
    private GameObject CombPrefab;
    private Animator anim;
    public TrumpBehavior(Battler parent)
    {
        WallPrefab = Resources.Load<GameObject>("Wall");
        CombPrefab = Resources.Load<GameObject>("Comb");
        Battler = parent;
        anim = parent.gameObject.GetComponent<Animator>();
    }

    public override void Initialize()
    {
        base.Initialize();
        Name = "Donald Trump";
        Stats = new Stats()
        {
            MaxHP = 2100,
            MaxSP = 1000,
            CurrentHP = 2100,
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
        SpecialAbilities.Add(new CombOver(this, CombPrefab));
        SpecialAbilities.Add(new YoureFired(this));
    }

    public override TurnArgs ChooseAttack(IList<Battler> allCharacters)
    {
        IList<Battler> liveCharacters = allCharacters.Where(b => b.BattleBehavior.Status.StatusEffect != StatusEffect.Defeated).ToList();
        int index = UnityEngine.Random.Range(0, liveCharacters.Count);
        Battler target = liveCharacters[index];
        float attackType = UnityEngine.Random.Range(0f, 1f);
        //Choose to use a normal attack or a special attack
        ActionType actionType = attackType < 0.5f ? ActionType.Attack : ActionType.Special;
        //ActionType actionType = ActionType.Special;
        int specialIndex = 0;
        ActionTarget actionTarget = ActionTarget.PartyMember;
        Action chosenAction = null;
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
                chosenAction = usableAbilities[specialIndex];
                //Possibly check to see if an attack can be used on a particular character
                actionTarget = chosenAction.ActionTarget;
            }
        }
        TurnArgs args = new TurnArgs
        {
            User = Battler,
            Target = target,
            ActionTarget = actionTarget,
            ActionIndex = -1,
            ActionType = actionType,
            Action = chosenAction
        };
        return args;
    }

    IEnumerator DeathCameraShake()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        float maxAmpl = 0.8f;
        float ampl = maxAmpl;
        int duration = 90;
        for (int i = duration; i >= 0; i--)
        {
            ampl = maxAmpl * (float)i / duration;
            Camera.main.transform.position = cameraPos + Random.insideUnitSphere * ampl;
            yield return 0;
        }

        Camera.main.transform.position = cameraPos;
    }

    public override IEnumerator EnemyDie()
    {
        Renderer r = Battler.GetComponentInChildren<Renderer>();
        //Amount of time to wait after taking damage
        yield return new WaitForSeconds(0.5f);
        Battler.StartCoroutine(SpecialEffectsManager.DeathCircles(Battler));
        r.material.EnableKeyword("_EMISSION");
        for (int i = 1; i <= 80; i++)
        {
            r.material.SetColor("_EmissionColor", Color.white * 0.7f * (float)i / 80);
            yield return 0;
        }
        yield return new WaitForSeconds(2f);
        Color startColor = r.material.color;
        Color currentColor = startColor;
        Color startEmissionColor = r.material.GetColor("_EmissionColor");
        SpecialEffectsManager.DeathParticles2(Battler);
        Battler.StartCoroutine(DeathCameraShake());
        int duration = 110;
        //All of this is necessary just to fade out the object...
        r.material.SetFloat("_Mode", 3);
        r.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        r.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        r.material.SetInt("_ZWrite", 0);
        r.material.DisableKeyword("_ALPHATEST_ON");
        r.material.DisableKeyword("_ALPHABLEND_ON");
        r.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        r.material.renderQueue = 3000;
        for (int i = 1; i <= duration; i++)
        {
            currentColor.a = startColor.a * (1f - ((float)i / duration));
            //Fix
            r.material.color = currentColor;
            Color currentEmissionColor = startEmissionColor;
            currentEmissionColor = startEmissionColor * (1f - ((float)i / duration));
            r.material.SetColor("_EmissionColor", currentEmissionColor);
            yield return 0;
        }
        r.enabled = false;
        yield return new WaitForSeconds(0.5f);
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
            Power = 1.6f;
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

        public override IEnumerator Run(Battler user, Battler target, IList<Battler> allCharacters, IList<Battler> allEnemies, BattleController bc)
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

    class CombOver : Action
    {
        GameObject CombPrefab;
        Vector3 cameraPos;
        public CombOver(BattleBehavior parent, GameObject combPrefab)
        {
            cameraPos = Camera.main.transform.position;
            CombPrefab = combPrefab;
            BattleBehavior = parent;
            Name = "Stylish Comb-Over";
            Description = "";
            RequiredSP = 12;
            Power = 1.2f;
            ActionTarget = ActionTarget.Party;
        }

        public override IEnumerator Run(Battler user, Battler target, IList<Battler> allCharacters, IList<Battler> allEnemies, BattleController battlecontroller)
        {
            Animator anim = user.gameObject.GetComponent<Animator>();
            SpecialEffectsManager.SpecialName(Name);
            Vector3 startPos = allCharacters[0].gameObject.transform.position;
            startPos.y = 2f;
            startPos.z = 10f;
            anim.SetInteger("State", 3);
            yield return new WaitForSeconds(2f);
            Instantiate(CombPrefab, startPos, Quaternion.identity);
            yield return new WaitForSeconds(2.4f);
            IList<Battler> liveCharacters = allCharacters.Where(c => c.BattleBehavior.Status.StatusEffect != StatusEffect.Defeated).ToList();
            foreach (Battler c in liveCharacters)
            {
                int baseDamage = (int)(user.BattleBehavior.Stats.SpAttack * Power * 6) - (c.BattleBehavior.Stats.SpDefense * 3);
                if (baseDamage > 0)
                    baseDamage = new System.Random().Next((int)(baseDamage * 0.9), (int)(baseDamage * 1.1));
                else
                    baseDamage = 0;
                c.BattleBehavior.TakeDamage(user, baseDamage);
            }
            anim.SetInteger("State", 0);
        }
    }

    class YoureFired : Action
    {
        public YoureFired(BattleBehavior parent)
        {
            BattleBehavior = parent;
            Name = "You're Fired!";
            Description = "";
            RequiredSP = 12;
            Power = 1.1f;
            ActionTarget = ActionTarget.LivePartyMember;
        }

        public override IEnumerator Run(Battler user, Battler target, IList<Battler> allCharacters, IList<Battler> allEnemies, BattleController battlecontroller)
        {
            Animator anim = user.gameObject.GetComponent<Animator>();
            SpecialEffectsManager.SpecialName(Name);
            anim.SetInteger("State", 3);
            yield return new WaitForSeconds(2f);
            SpecialEffectsManager.FireParticles(target);
            yield return new WaitForSeconds(1f);
            int baseDamage = (int)(user.BattleBehavior.Stats.SpAttack * Power * 6) - (target.BattleBehavior.Stats.SpDefense * 3);
            if (baseDamage > 0)
                baseDamage = new System.Random().Next((int)(baseDamage * 0.9), (int)(baseDamage * 1.1));
            else
                baseDamage = 0;
            target.BattleBehavior.TakeDamage(user, baseDamage);
            //50% chance of inflicting a burn
            target.BattleBehavior.AddStatusEffect(user, StatusEffect.Burn, 0.5f);
            anim.SetInteger("State", 0);
        }
    }
}
