﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class BattleController : MonoBehaviour {
    public Lifebar lifebar;
    public BattleMenu BattleMenu;
    public GameObject GameOverPrefab;
    public GameObject Canvas;
    public GameObject LifebarPrefab;
    public GameObject ShieldPrefab;
    public GameObject DamageNumberPrefab;
    public GameObject RestoreParticlesPrefab;
    public Text text;
    public Battler[] Battlers;
    private IList<GameObject> Lifebars;
    private IList<Battler> allCharacters;
    private IList<Battler> allEnemies;
    private IList<Battler> allBattlers;
    private IList<Shield> Shields;
    private Material MagicCircleMaterial;
    private CharacterTurnArgs charTurnArgs;
    private bool wait;
    private bool battleOver;

	void Start () {
        wait = false;
        battleOver = false;
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
        Shields = new List<Shield>(allCharacters.Count);
        for (int i = 0; i < allCharacters.Count; i++)
        {
            allCharacters[i].BattlerIndex = i;
            //Attach lifebars
            GameObject lifebar = Instantiate(LifebarPrefab) as GameObject;
            lifebar.gameObject.transform.parent = Canvas.transform;
            lifebar.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, -20f - 55f * i);
            lifebar.GetComponent<Lifebar>().Character = allCharacters[i];
            Lifebars.Insert(i, lifebar);

            //Attach shields
            GameObject shield = Instantiate(ShieldPrefab, allCharacters[i].gameObject.transform.position + new Vector3(1f, 0f, 0f), Quaternion.identity) as GameObject;
            shield.gameObject.transform.SetParent(allCharacters[i].gameObject.transform);
            shield.gameObject.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            Vector3 pos = shield.gameObject.transform.position;
            pos.y = 2f;
            shield.gameObject.transform.position = pos;
            Shields.Insert(i, shield.GetComponent<Shield>());
        }
        StartCoroutine(Run());
	}

    void OnCharacterTurn(CharacterTurnArgs args)
    {
        wait = false;
        charTurnArgs = args;
    }

    void OnEnable()
    {
        BattleMenu.StartCharacterTurn += OnCharacterTurn;
        BattleBehavior.HPText += HPText;
        BattleBehavior.Death += BattlerDied;
    }

    void OnDisable()
    {
        BattleMenu.StartCharacterTurn -= OnCharacterTurn;
        BattleBehavior.HPText -= HPText;
        BattleBehavior.Death -= BattlerDied;
    }

    void BattlerDied(BattleBehavior battler, BattlerType type)
    {
        StartCoroutine(DeathRoutine(battler, type));
    }

    IEnumerator DeathRoutine(BattleBehavior battler, BattlerType type)
    {
        wait = true;
        if (type == BattlerType.Character)
        {
            yield return battler.CharacterDie();
        }
        else if (type == BattlerType.Enemy)
        {
            yield return battler.EnemyDie();
        }
        wait = false;
    }

    void HPText(Battler battler, int dHealth)
    {
        
        Vector3 position = Camera.main.WorldToScreenPoint(battler.gameObject.transform.position);
        position.y += 30f;
        GameObject num = Instantiate(DamageNumberPrefab) as GameObject;
        num.transform.SetParent(Canvas.transform);
        num.transform.position = position;
        DamageNumber dn = num.GetComponent<DamageNumber>();
        dn.Damage = Mathf.Abs(dHealth);
        if (dHealth > 0)
        {
            dn.Color = Color.green;
        }
    }
	
	void Update () {

	}

    IEnumerator PerformAction(Battler user, ActionType ActionType, int actionIndex, Battler target)
    {
        switch (ActionType)
        {
            case ActionType.Attack:
                yield return user.BattleBehavior.StandardAttack(user, target);
                break;

            case ActionType.Special:
                user.BattleBehavior.SpecialAbilities[actionIndex].UseSP(user);
                yield return user.BattleBehavior.SpecialAbilities[actionIndex].Run(user, target);
                break;

            case ActionType.Item:
                user.Inventory[actionIndex].RemoveFromInventory(user);
                user.Inventory[actionIndex].Use(user, target);
                break;

            case ActionType.Defend:
                Shield shield = Shields[user.BattlerIndex];
                user.BattleBehavior.Defending = true;
                shield.StartCoroutine(shield.Enter());
                break;
        }
    }

    bool AllCharactersDefeated()
    {
        foreach (Battler character in allCharacters)
        {
            if (character.BattleBehavior.Status.StatusEffect != StatusEffect.Defeated)
            {
                return false;
            }
        }
        return true;
    }

    bool AllEnemiesDefeated()
    {
        foreach (Battler enemy in allEnemies)
        {
            if (enemy.BattleBehavior.Status.StatusEffect != StatusEffect.Defeated)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator Run()
    {
        float alpha = 0f;
        int count = 0;
        while (!battleOver)
        {
            count++;
            alpha = 0.5f;
            //int battlerIndex = 0;
            foreach (Battler battler in allBattlers)
            {
                //Check if all enemies are dead
                if (AllEnemiesDefeated())
                {
                    Debug.Log("Victory!");
                    battleOver = true;
                    yield break;
                }
                else if (AllCharactersDefeated())
                {
                    Debug.Log("Game Over...");
                    battleOver = true;
                    GameObject gameOver = Instantiate(GameOverPrefab) as GameObject;
                    gameOver.transform.SetParent(Canvas.transform);
                    gameOver.transform.localPosition = Vector3.zero;
                    yield break;
                }
                //Skip this character if it is dead
                if (battler.BattleBehavior.Status.StatusEffect == StatusEffect.Defeated)
                {
                    continue;
                }
                if (battler.BattlerType == BattlerType.Character)
                {
                    Shield battlerShield = Shields[battler.BattlerIndex];
                    wait = true;
                    //Get rid of the shield
                    battler.BattleBehavior.Defending = false;
                    if (battlerShield.Active)
                    {
                        battlerShield.StartCoroutine(battlerShield.Exit());
                    }
                    
                    BattleMenu.CharacterTurn(battler);
                    while (wait)
                    {
                        yield return 0;
                    }
                    yield return PerformAction(charTurnArgs.User, charTurnArgs.ActionType, charTurnArgs.ActionIndex, charTurnArgs.Target);
                }
                else
                {
                    battler.BattleBehavior.ChooseTarget(allCharacters);
                    yield return battler.BattleBehavior.StandardAttack(battler, null);
                }
                //If a battler is dying
                while (wait)
                {
                    yield return 0;
                }
            }
            yield return 0;
        }
    }
}
