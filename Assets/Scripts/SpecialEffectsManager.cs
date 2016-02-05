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

    private static GameObject specialName;
    private static GameObject SpecialNamePrefab
    {
        get
        {
            if (specialName == null)
            {
                specialName = Resources.Load<GameObject>("SpecialName");
            }
            return specialName;
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

    public static void SpecialName(string specialAttackName)
    {
        GameObject name = MonoBehaviour.Instantiate(SpecialNamePrefab, Vector3.zero, Quaternion.identity) as GameObject;
        name.transform.SetParent(Canvas.transform);
        name.transform.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        name.GetComponentInChildren<Text>().text = specialAttackName;
        MonoBehaviour.Destroy(name, 2f);
    }
}
