using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class BattleBehavior : MonoBehaviour {
    public static event HPTextHandler HPText;
    public static event SpecialEffectsHandler SpecialEffects;
    public static event Action<BattleBehavior> Death;
    public event SpecialEffectsHandler a;
    public event CharacterHPHandler HPChanged;
    public string Name { get; protected set; }
    public Stats Stats { get; set; }
    public Status Status { get; set; }
    public IList<Action> SpecialAbilities { get; protected set; }
    public virtual bool Defending { get; set; }
    protected Battler Battler { get; set; }
    //For letting an enemy choose a random target
    protected Battler Target { get; set; }
	// Use this for initialization
	void Start () {
	}

    public virtual void Initialize()
    {
        Status = new Status();
    }

    //Used by enemies
    public virtual void ChooseTarget(IList<Battler> battlers)
    {

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
                Death(this);
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
