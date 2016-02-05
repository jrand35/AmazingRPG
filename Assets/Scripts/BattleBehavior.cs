using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class BattleBehavior : MonoBehaviour {
    public static event HPTextHandler HPText;
    public static event SpecialEffectsHandler SpecialEffects;
    public static event Action<BattleBehavior, BattlerType> Death;
    public event SpecialEffectsHandler a;
    public event CharacterHPHandler HPChanged;
    public string Name { get; protected set; }
    public Stats Stats { get; set; }
    public Status Status { get; set; }
    public IList<Action> SpecialAbilities { get; protected set; }
    public virtual bool Defending { get; set; }
    protected Battler Battler { get; set; }
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
    //50% normal attack, 50% special ability
    public virtual TurnArgs ChooseAttack(IList<Battler> allCharacters)
    {
        IList<Battler> liveCharacters = allCharacters.Where(b => b.BattleBehavior.Status.StatusEffect != StatusEffect.Defeated).ToList();
        int index = UnityEngine.Random.Range(0, liveCharacters.Count);
        Battler target = liveCharacters[index];
        float attackType = UnityEngine.Random.Range(0f, 1f);
        //Choose to use a normal attack or a special attack
        ActionType actionType = attackType < 0.5f ? ActionType.Attack : ActionType.Special;
        int specialIndex = 0;
        ActionTarget actionTarget = ActionTarget.PartyMember;
        if (actionType == ActionType.Special)
        {
            specialIndex = UnityEngine.Random.Range(0, SpecialAbilities.Count);
            actionTarget = SpecialAbilities[specialIndex].ActionTarget;
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
	
	// Update is called once per frame
	void Update () {
	
	}
}
