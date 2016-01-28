using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleController : MonoBehaviour {
    public Lifebar lifebar;
    public BattleMenu BattleMenu;
    public GameObject MagicCircle;
    public Text text;
    public Battler[] Battlers;
    private IList<Battler> allCharacters;
    private IList<Battler> allEnemies;
    private Material MagicCircleMaterial;
    private CharacterTurnArgs charTurnArgs;
    private bool waitForCharacterTurn;

	void Start () {
        waitForCharacterTurn = false;
        StartCoroutine(Run());
        MagicCircleMaterial = MagicCircle.GetComponent<MeshRenderer>().material;
        StartCoroutine(SpinCircle());

        allCharacters = Battlers.Where(b => b.BattlerType == BattlerType.Character).ToList();
        allEnemies = Battlers.Where(b => b.BattlerType == BattlerType.Enemy).ToList();
        BattleMenu.allCharacters = allCharacters;
        BattleMenu.allEnemies = allEnemies;

        lifebar.Character = allCharacters[0];
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
        Vector3 initScale = MagicCircle.transform.localScale;
        float angle = 0f;
        float alphaCount = 0f;
        while (true)
        {
            angle += 3f;
            alphaCount += 0.05f; //0.1f;
            float alpha = 0.4f + 0.2f * Mathf.Cos(alphaCount);
            Vector3 scale = initScale + (initScale * -0.15f * Mathf.Cos(alphaCount));
            MagicCircle.transform.localScale = scale;
            MagicCircle.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            MagicCircleMaterial.SetColor("_TintColor", new Color(1f, 1f, 1f, alpha));
            yield return 0;
        }
    }
	
	void Update () {

	}

    IEnumerator PerformAction(Battler user, ActionType ActionType, int index, Battler target)
    {
        switch (ActionType)
        {
            case ActionType.Attack:
                yield return user.BattleBehavior.StandardAttack(user, target);
                break;

            case ActionType.Special:
                yield return user.BattleBehavior.SpecialAbilities[index].Run(user, target);
                break;

            case ActionType.Item:
                user.Inventory[index].Use(user, target);
                break;

            case ActionType.Defend:
                break;
        }
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
                    yield return PerformAction(charTurnArgs.User, charTurnArgs.ActionType, charTurnArgs.ActionIndex, charTurnArgs.Target);
                }
                else
                {
                    yield return battler.BattleBehavior.StandardAttack(allEnemies[0], allCharacters[0]);
                }
            }
            yield return 0;
        }
    }
}
