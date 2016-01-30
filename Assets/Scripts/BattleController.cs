using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleController : MonoBehaviour {
    public Lifebar lifebar;
    public BattleMenu BattleMenu;
    public GameObject Canvas;
    public GameObject LifebarPrefab;
    public Text text;
    public Battler[] Battlers;
    private IList<GameObject> Lifebars;
    private IList<Battler> allCharacters;
    private IList<Battler> allEnemies;
    private IList<Battler> allBattlers;
    private Material MagicCircleMaterial;
    private CharacterTurnArgs charTurnArgs;
    private bool waitForCharacterTurn;

	void Start () {
        waitForCharacterTurn = false;
        foreach (Battler b in Battlers)
        {
            b.Initialize();
        }

        allCharacters = Battlers.Where(b => b.BattlerType == BattlerType.Character).ToList();
        allEnemies = Battlers.Where(b => b.BattlerType == BattlerType.Enemy).ToList();
        allBattlers = Battlers.OrderByDescending(b => b.BattleBehavior.Stats.Speed).ToList();
        BattleMenu.allCharacters = allCharacters;
        BattleMenu.allEnemies = allEnemies;

        //Create lifebars for each party member and attach them to the canvas
        Lifebars = new List<GameObject>(allCharacters.Count);
        for (int i = 0; i < allCharacters.Count; i++)
        {
            GameObject lifebar = Instantiate(LifebarPrefab) as GameObject;
            lifebar.gameObject.transform.parent = Canvas.transform;
            lifebar.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -20f - 55f * i);
            lifebar.GetComponent<Lifebar>().Character = allCharacters[i];
            Lifebars.Insert(i, lifebar);
        }
        StartCoroutine(Run());
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
                user.BattleBehavior.SpecialAbilities[index].UseSP(user);
                yield return user.BattleBehavior.SpecialAbilities[index].Run(user, target);
                break;

            case ActionType.Item:
                user.Inventory[index].RemoveFromInventory(user);
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
            foreach (Battler battler in allBattlers)
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
