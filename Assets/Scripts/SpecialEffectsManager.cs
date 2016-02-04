using UnityEngine;
using System.Collections;

public static class SpecialEffectsManager {
    private static GameObject rpp;
    public static GameObject RestoreParticlesPrefab
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

    public static void RestoreParticles(Battler battler)
    {
        Vector3 pos = battler.gameObject.transform.position;
        pos.y = 0f;
        GameObject restoreParticles = MonoBehaviour.Instantiate(RestoreParticlesPrefab, pos, Quaternion.Euler(270f, 0f, 0f)) as GameObject;
        MonoBehaviour.Destroy(restoreParticles, 3f);
    }
}
