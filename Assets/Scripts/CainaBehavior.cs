using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CainaBehavior : BattleBehavior
{
    private bool defending;
    public Animator anim;
    public CainaBehavior(Battler parent)
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
        Name = "Caina";
        Stats = new Stats
        {
            MaxHP = 650,
            MaxSP = 350,
            CurrentHP = 650,
            CurrentSP = 350,
            Attack = 25,
            Defense = 27,
            SpAttack = 30,
            SpDefense = 36,
            Speed = 22,
            Luck = 30
        };
        SpecialAbilities = new List<Action>();
        SpecialAbilities.Add(new Restore(this));
        SpecialAbilities.Add(new ShootingStars(this));
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
        if (!Defending)
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
            Name = "Restore";
            Description = "Restores 100 HP to a party member.";
            RequiredSP = 8;
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
                m.SetColor("_EmissionColor", Color.white * 0.6f * Mathf.Sin((float)i / duration * Mathf.PI));
                yield return 0;
            }
            target.BattleBehavior.RestoreHP(user, 100);
            anim.SetInteger("State", 0);
            Debug.Log(user.BattleBehavior.Name + " restored 100 HP to " + target.BattleBehavior.Name);
        }
    }

    class ShootingStars : Action
    {
        int dimDuration = 30;
        float minDim = 0.1f;
        int starCount = 15;
        GameObject StarPrefab;
        Light light;
        Animator anim;

        public ShootingStars(BattleBehavior parent)
        {
            BattleBehavior = parent;
            Name = "Shooting Stars";
            Description = "Attack all enemies with a barrage of stars.";
            RequiredSP = 25;
            Power = 2f;
            ActionTarget = ActionTarget.Enemy;  //Change
            light = GameObject.FindGameObjectWithTag("Light").GetComponent<Light>();
            StarPrefab = Resources.Load<GameObject>("Star");
        }

        IEnumerator Dim()
        {
            for (int i = 0; i < dimDuration; i++)
            {
                light.intensity = 1f - (1f - minDim) * ((float)(i + 1) / dimDuration);
                yield return 0;
            }
            light.intensity = minDim;
        }

        IEnumerator Brighten()
        {
            for (int i = 0; i < dimDuration; i++)
            {
                light.intensity = minDim + (1f - minDim) * ((float)(i + 1) / dimDuration);
                yield return 0;
            }
            light.intensity = 1f;
        }

        void CreateStar(Battler user, Battler target)
        {
            float hue = Random.Range(0f, 1f);
            Color color = Color.HSVToRGB(hue, 1f, 1f);
            color *= 1.1f;
            Vector3 starPos = user.gameObject.transform.position;
            starPos.x -= 20f;
            starPos.y = 20f;
            starPos.z += Random.Range(-20f, 20f);
            Vector3 targetPos = target.gameObject.transform.position;
            targetPos.z = starPos.z;
            Quaternion angle = Quaternion.Euler(0f, 180f, 0f);
            GameObject star = Instantiate(StarPrefab, starPos, angle) as GameObject;
            Material starMaterial = star.GetComponent<MeshRenderer>().material;
            //Changes color to blue
            starMaterial.SetColor("_EmissionColor", color);//new Color(0f, 0f, 1.2f));
            Rigidbody rb = star.GetComponent<Rigidbody>();
            var heading = targetPos - starPos;
            var distance = heading.magnitude;
            var direction = heading / distance;
            rb.AddForce(direction * 1500f);
            Destroy(star, 5f);
        }

        public override IEnumerator Run(Battler user, Battler target, IList<Battler> allCharacters, IList<Battler> allEnemies, BattleController bc)
        {
            anim = user.GetComponent<Animator>();
            anim.SetInteger("State", 3);
            yield return new WaitForSeconds(1.4f);
            Camera.main.hdr = true;
            Light light = GameObject.FindGameObjectWithTag("Light").GetComponent<Light>();
            yield return new WaitForSeconds(0.3f);
            yield return Dim();
            for (int i = 0; i < starCount; i++)
            {
                CreateStar(user, target);
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(3f);
            int baseDamage = (int)((user.BattleBehavior.Stats.SpAttack * 6 * Power) - (target.BattleBehavior.Stats.SpDefense * 3));
            baseDamage = new System.Random().Next((int)(baseDamage * 0.9), (int)(baseDamage * 1.1));
            target.BattleBehavior.TakeDamage(user, baseDamage);
            yield return Brighten();
            Camera.main.hdr = false;
            anim.SetInteger("State", 0);
        }
    }
}