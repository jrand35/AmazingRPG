using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class SpecialEffectsManager {
    private static GameObject canvas;
    private static GameObject Canvas
    {
        get
        {
            if (canvas == null)
            {
                canvas = GameObject.Find("Canvas");
            }
            return canvas;
        }
    }
    private static GameObject rpp;
    private static GameObject RestoreParticlesPrefab
    {
        get
        {
            if (rpp == null)
            {
                rpp = Resources.Load<GameObject>("RestoreParticles");
            }
            return rpp;
        }
    }

    private static GameObject dp;
    private static GameObject DeathParticlesPrefab
    {
        get
        {
            if (dp == null)
            {
                dp = Resources.Load<GameObject>("DeathParticles");
            }
            return dp;
        }
    }

    private static GameObject dp2;
    private static GameObject DeathParticlesPrefab2
    {
        get
        {
            if (dp2 == null)
            {
                dp2 = Resources.Load<GameObject>("DeathParticles2");
            }
            return dp2;
        }
    }

    //Current instance
    private static GameObject specialName;
    private static GameObject specialNamePrefab;
    private static GameObject SpecialNamePrefab
    {
        get
        {
            if (specialNamePrefab == null)
            {
                specialNamePrefab = Resources.Load<GameObject>("SpecialName");
            }
            return specialNamePrefab;
        }
    }

    private static GameObject deathCircle;
    private static GameObject DeathCirclePrefab
    {
        get
        {
            if (deathCircle == null)
            {
                deathCircle = Resources.Load<GameObject>("Circle");
            }
            return deathCircle;
        }
    }

    private static GameObject fireParticles;
    private static GameObject FireParticlesPrefab
    {
        get
        {
            if (fireParticles == null)
            {
                fireParticles = Resources.Load<GameObject>("FireParticles");
            }
            return fireParticles;
        }
    }

    public static void RestoreParticles(Battler battler)
    {
        Vector3 pos = battler.gameObject.transform.position;
        pos.y = 0f;
        GameObject restoreParticles = MonoBehaviour.Instantiate(RestoreParticlesPrefab, pos, Quaternion.Euler(270f, 0f, 0f)) as GameObject;
        MonoBehaviour.Destroy(restoreParticles, 3f);
    }

    public static void DeathParticles(Battler battler)
    {
        Vector3 pos = battler.gameObject.transform.position;
        pos.y = 0f;
        GameObject deathParticles = MonoBehaviour.Instantiate(DeathParticlesPrefab, pos, Quaternion.identity) as GameObject;
        MonoBehaviour.Destroy(deathParticles, 3f);
    }

    public static void DeathParticles2(Battler battler)
    {
        Vector3 pos = battler.gameObject.transform.position;
        pos.y = 0f;
        GameObject deathParticles = MonoBehaviour.Instantiate(DeathParticlesPrefab2, pos, Quaternion.identity) as GameObject;
        MonoBehaviour.Destroy(deathParticles, 3f);
    }

    public static IEnumerator SpecialName(string specialAttackName)
    {
        MonoBehaviour.Destroy(specialName);
        specialName = MonoBehaviour.Instantiate(SpecialNamePrefab, Vector3.zero, Quaternion.identity) as GameObject;
        specialName.transform.SetParent(Canvas.transform);
        specialName.GetComponentInChildren<Text>().text = specialAttackName;
        for (int i = 1; i <= 15; i++)
        {
            specialName.transform.GetComponent<RectTransform>().anchoredPosition = Vector3.zero + new Vector3(0f, 100f * (1 - (float)i / 15), 0f);
            yield return 0;
        }
        yield return new WaitForSeconds(1f);
        for (int i = 1; i <= 15; i++)
        {
            specialName.transform.GetComponent<RectTransform>().anchoredPosition = Vector3.zero + new Vector3(0f, 100f * (float)i / 15, 0f);
            yield return 0;
        }
        MonoBehaviour.Destroy(specialName);
    }

    public static IEnumerator DeathCircles(Battler battler)
    {
        int circleCount = 25;
        Vector3 circlePos = battler.gameObject.transform.position;
        circlePos.y = battler.GetComponentInChildren<Renderer>().bounds.center.y;
        for (int i = 0; i < circleCount; i++)
        {
            Vector3 dir = Random.insideUnitSphere;
            Quaternion facing = Quaternion.LookRotation(dir);
            MonoBehaviour.Instantiate(DeathCirclePrefab, circlePos, facing);
            yield return new WaitForSeconds(0.08f);
        }
    }

    public static void FireParticles(Battler battler)
    {
        Vector3 pos = battler.gameObject.transform.position;
        pos.y = 0f;
        GameObject fire = MonoBehaviour.Instantiate(FireParticlesPrefab, pos, Quaternion.Euler(-90f, 0f, 0f)) as GameObject;
        MonoBehaviour.Destroy(fire, 5f);
    }
}
