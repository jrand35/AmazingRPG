using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class BattleBehavior : MonoBehaviour {
    public static event HPTextHandler HPText;
    public static event SpecialEffectsHandler SpecialEffects;
    public static event Action<BattleBehavior, BattlerType> Death;
    public static event Action<Battler> Burned;
    public event SpecialEffectsHandler a;
    public event CharacterHPHandler HPChanged;
    public event CharacterHPHandler SPChanged;
    public string Name { get; protected set; }
    public Stats Stats { get; set; }
    public Status Status { get; set; }
    public IList<Action> SpecialAbilities { get; protected set; }
    public virtual bool Defending { get; set; }
    public Battler Battler { get; set; }
    //For letting an enemy choose a random target
    //protected Battler Target { get; set; }
	// Use this for initialization
	void Start () {
	}

    public virtual void Initialize()
    {
        Status = new Status();
    }

    //Used by enemies
    //75% normal attack, 25% special ability
    public virtual TurnArgs ChooseAttack(IList<Battler> allCharacters)
    {
        IList<Battler> liveCharacters = allCharacters.Where(b => b.BattleBehavior.Status.StatusEffect != StatusEffect.Defeated).ToList();
        int index = UnityEngine.Random.Range(0, liveCharacters.Count);
        Battler target = liveCharacters[index];
        float attackType = UnityEngine.Random.Range(0f, 1f);
        //Choose to use a normal attack or a special attack
        ActionType actionType = attackType < 0.75f ? ActionType.Attack : ActionType.Special;
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
            Action = chosenAction
        };
        return args;
    }
    
    public virtual Battler ChooseTarget(IList<Battler> battlers)
    {
        return null;
    }

    public virtual IEnumerator CharacterDie()
    {
        yield return new WaitForSeconds(0.5f);
        Animator anim = Battler.GetComponent<Animator>();
        anim.SetInteger("State", -1);
        yield return new WaitForSeconds(1.5f);
    }

    public virtual IEnumerator EnemyDie()
    {
        //Amount of time to wait after taking damage
        yield return new WaitForSeconds(0.5f);
        Renderer r = Battler.GetComponentInChildren<Renderer>();
        Color startColor = r.material.color;
        Color currentColor = startColor;
        int duration = 55;
        SpecialEffectsManager.DeathParticles(Battler);
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
            yield return 0;
        }
        r.enabled = false;
    }

    public virtual IEnumerator StandardAttack(Battler user, Battler target)
    {
        yield return 0;
    }

    public virtual void TakeDamage(Battler user, int baseDamage)
    {
        if (Defending)
        {
            baseDamage /= 2;
        }
        Debug.Log("Took damage " + baseDamage.ToString());
        Stats.CurrentHP -= baseDamage;
        if (Stats.CurrentHP < 0)
        {
            Stats.CurrentHP = 0;
        }
        if (HPChanged != null)
        {
            HPChanged(Stats.CurrentHP, Stats.MaxHP);
        }
        if (HPText != null)
        {
            HPText(Battler, -baseDamage);
        }

        if (Stats.CurrentHP == 0)
        {
            Status.SetStatus(StatusEffect.Defeated);
            if (Death != null)
            {
                Death(this, Battler.BattlerType);
            }
        }
    }

    public virtual void AddStatusEffect(Battler user, StatusEffect effect, float chance)
    {
        float rand = UnityEngine.Random.Range(0f, 1f);
        if (rand <= chance)
        {
            bool result = Status.SetStatus(effect);
            if (result) {
                if (effect == StatusEffect.Burn)
                {
                    if (Burned != null)
                    {
                        Burned(Battler);
                    }
                }
            }
        }
    }

    void TakeDirectDamage(Battler user, int baseDamage)
    {
        Debug.Log("Took damage " + baseDamage.ToString());
        Stats.CurrentHP -= baseDamage;
        if (Stats.CurrentHP < 0)
        {
            Stats.CurrentHP = 0;
        }
        if (HPChanged != null)
        {
            HPChanged(Stats.CurrentHP, Stats.MaxHP);
        }
        if (HPText != null)
        {
            HPText(Battler, -baseDamage);
        }

        if (Stats.CurrentHP == 0)
        {
            Status.SetStatus(StatusEffect.Defeated);
            if (Death != null)
            {
                Death(this, Battler.BattlerType);
            }
        }
    }

    public virtual void EndOfTurn()
    {
        if (Status.StatusEffect == StatusEffect.Burn)
        {
            float rand = UnityEngine.Random.Range(0.95f, 1.05f);
            float baseDmg = Stats.MaxHP * 0.1f * rand;
            int dmg = Mathf.RoundToInt(baseDmg);
            TakeDirectDamage(Battler, dmg);
        }
    }

    public void SPEvent(int sp, int maxSP)
    {
        if (SPChanged != null)
        {
            SPChanged(sp, maxSP);
        }
    }

    public virtual void RestoreHP(Battler user, int amount)
    {
        Debug.Log("Restored HP");
        Stats.CurrentHP += amount;
        if (Stats.CurrentHP > Stats.MaxHP)
        {
            Stats.CurrentHP = Stats.MaxHP;
        }
        if (HPChanged != null)
        {
            HPChanged(Stats.CurrentHP, Stats.MaxHP);
        }
        if (HPText != null)
        {
            HPText(Battler, amount);
        }
    }

    public virtual void Victory()
    {
        ;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
