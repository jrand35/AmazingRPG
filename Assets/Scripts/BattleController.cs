using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleController : MonoBehaviour {
    public GameObject MagicCircle;
    public Text text;
    public Battler[] Battlers;
    private Material MagicCircleMaterial;
	void Start () {
        StartCoroutine(Run());
        MagicCircleMaterial = MagicCircle.GetComponent<MeshRenderer>().material;
        StartCoroutine(SpinCircle());
	}

    IEnumerator SpinCircle()
    {
        float angle = 0f;
        float alphaCount = 0f;
        while (true)
        {
            angle += 3f;
            alphaCount += 0.1f;
            float alpha = 0.4f + 0.2f * Mathf.Cos(alphaCount);
            MagicCircle.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            MagicCircleMaterial.SetColor("_TintColor", new Color(1f, 1f, 1f, alpha));
            yield return 0;
        }
    }
	
	void Update () {

	}

    IEnumerator Run()
    {
        while (true)
        {
            foreach (Battler battler in Battlers)
            {
                if (battler.BattlerID == BattlerID.Character)
                {
                    yield return battler.BattleBehavior.SpecialAbilities[0].Run(battler, battler);
                }
                else
                {
                    yield return battler.BattleBehavior.StandardAttack(battler, battler);
                }
            }
            yield return 0;
        }
    }
}
