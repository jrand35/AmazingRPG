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
        //MagicCircleMaterial.SetColor("_TintColor", new Color(0f, 0f, 1f, 1f));
        StartCoroutine(SpinCircle());
	}

    IEnumerator SpinCircle()
    {
        float angle = 0f;
        while (true)
        {
            angle += 3f;
            MagicCircle.transform.rotation = Quaternion.Euler(0f, angle, 0f);
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
