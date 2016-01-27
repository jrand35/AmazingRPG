using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleController : MonoBehaviour {
    public BattleMenu BattleMenu;
    public GameObject MagicCircle;
    public Text text;
    public Battler[] Battlers;
    private Material MagicCircleMaterial;
    private CharacterTurnArgs charTurnArgs;
    private bool waitForCharacterTurn;

	void Start () {
        waitForCharacterTurn = false;
        StartCoroutine(Run());
        MagicCircleMaterial = MagicCircle.GetComponent<MeshRenderer>().material;
        StartCoroutine(SpinCircle());

        IList<Battler> allCharacters = Battlers.Where(b => b.BattlerType == BattlerType.Character).ToList();
        IList<Battler> allEnemies = Battlers.Where(b => b.BattlerType == BattlerType.Enemy).ToList();
        BattleMenu.allCharacters = allCharacters;
        BattleMenu.allEnemies = allEnemies;
	}

    void OnCharacterTurn(CharacterTurnArgs args)
    {
        waitForCharacterTurn = false;
        charTurnArgs = args;
    }

    void OnEnable()
    {
        BattleMenu.StartCharacterTurn += OnCharacterTurn;
    }

    void OnDisable()
    {
        BattleMenu.StartCharacterTurn -= OnCharacterTurn;
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
                if (battler.BattlerType == BattlerType.Character)
                {
                    waitForCharacterTurn = true;
                    BattleMenu.CharacterTurn(battler);
                    while (waitForCharacterTurn)
                    {
                        yield return 0;
                    }
                }
                else
                {
                    //yield return battler.BattleBehavior.StandardAttack(battler, battler);
                }
            }
            yield return 0;
        }
    }
}
