using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JoshBehavior : BattleBehavior
{
    public JoshBehavior(Battler parent)
    {
        Battler = parent;
    }

    public override void Initialize()
    {
        base.Initialize();
        Name = "Josh";
        Stats = new Stats
        {
            MaxHP = 500,
            MaxSP = 500,
            CurrentHP = 500,
            CurrentSP = 500,
            Attack = 28,
            Defense = 22,
            SpAttack = 30,
            SpDefense = 32,
            Speed = 24,
            Luck = 20
        };
        SpecialAbilities = new List<Action>();
        SpecialAbilities.Add(new Restore(this));
        SpecialAbilities.Add(new ShootingStars(this));
    }

    public override IEnumerator StandardAttack(Battler user, Battler target)
    {
        Vector3 startPos = user.gameObject.transform.position;
        yield return Move.MoveInFrontOfBattler(user, target, startPos, new Vector3(-2f, 0f, 0f), 50);
        int baseDamage = (user.BattleBehavior.Stats.Attack * 6) - (target.BattleBehavior.Stats.Defense * 3);
        baseDamage = new System.Random().Next((int)(baseDamage * 0.9), (int)(baseDamage * 1.1));
        target.BattleBehavior.TakeDamage(user, baseDamage);
        yield return new WaitForSeconds(0.5f);
        yield return Move.MoveBackFromBattler(user, target, startPos, new Vector3(-2f, 0f, 0f), 50);
    }

    class Restore : Action
    {
        public Restore(BattleBehavior parent)
        {
            BattleBehavior = parent;
            Name = "Restore";
            Description = "Restores 200 HP to a party member.";
            RequiredSP = 5;
            Power = 0;
            ActionTarget = ActionTarget.LivePartyMember;
        }

        public override IEnumerator Run(Battler user, Battler target)
        {
            int duration = 40;
            Material m = target.GetComponentInChildren<Renderer>().material;
            m.EnableKeyword("_EMISSION");

            SpecialEffectsManager.RestoreParticles(target);
            for (int i = 0; i < duration; i++)
            {
                m.SetColor("_EmissionColor", Color.white * 0.6f * Mathf.Sin((float)i / duration * Mathf.PI));
                yield return 0;
            }
            target.BattleBehavior.RestoreHP(user, 200);
            Debug.Log(user.BattleBehavior.Name + " restored 200 HP to " + target.BattleBehavior.Name);
        }
    }

    class ShootingStars : Action
    {
        int dimDuration = 30;
        float minDim = 0.1f;
        int starCount = 15;
        GameObject StarPrefab;
        Light light;
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

        public override IEnumerator Run(Battler user, Battler target)
        {
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
        }
    }
}