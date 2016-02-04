using UnityEngine;
using System.Collections;

public static class SpecialEffectsManager {
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
}
