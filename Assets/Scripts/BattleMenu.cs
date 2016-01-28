using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattleMenu : MonoBehaviour {
    public event CharacterTurnHandler StartCharacterTurn;
    public Text[] Text;
    public IList<Battler> allCharacters { get; set; }
    public IList<Battler> allEnemies { get; set; }
    private BattleMenuItem CurrentMenu;
    private BattleMenuItem PrevMenu;
    private Battler currentCharacter;
    private int mainIndex;
    private int specialIndex;
    private int itemIndex;
    private int charIndex;
    private int enemyIndex;
    private int listSize;
    private int maxListSize = 4;

    //The index of the current menu
    private int currentIndex
    {
        get
        {
            switch (CurrentMenu)
            {
                case BattleMenuItem.Main:
                    return mainIndex;

                case BattleMenuItem.Enemy:
                    return enemyIndex;

                case BattleMenuItem.Special:
                    return specialIndex;

                case BattleMenuItem.Item:
                    return itemIndex;

                case BattleMenuItem.Player:
                    return charIndex;
                default:
                    return 0;
            }
        }
        set
        {
            switch (CurrentMenu)
            {
                case BattleMenuItem.Main:
                    mainIndex = value;
                    break;

                case BattleMenuItem.Enemy:
                    enemyIndex = value;
                    break;

                case BattleMenuItem.Special:
                    specialIndex = value;
                    break;

                case BattleMenuItem.Item:
                    itemIndex = value;
                    break;

                case BattleMenuItem.Player:
                    charIndex = value;
                    break;
                default:
                    break;
            }
        }
    }
	// Use this for initialization
	void Start () {
        mainIndex = 0;
        specialIndex = 0;
        itemIndex = 0;
        charIndex = 0;
        enemyIndex = 0;
        listSize = 0;
        MainMenu();
        UpdateMenu();
	}

    public void CharacterTurn(Battler character)
    {
        gameObject.SetActive(true);
        currentCharacter = character;
        mainIndex = 0;
        specialIndex = 0;
        itemIndex = 0;
        charIndex = 0;
        enemyIndex = 0;
        MainMenu();
        UpdateMenu();
    }

    void MainMenu()
    {
        PrevMenu = CurrentMenu;
        CurrentMenu = BattleMenuItem.Main;
        switch (PrevMenu)
        {
            case BattleMenuItem.Enemy:
                //mainIndex = 0;
                break;

            case BattleMenuItem.Special:
                //index = 1;
                break;

            case BattleMenuItem.Item:
                //index = 2;
                break;
        }
        listSize = 4;
        Text[0].text = "Attack";
        Text[1].text = "Special";
        Text[2].text = "Item";
        Text[3].text = "Defend";
    }

    void SpecialMenu()
    {

    }

    void EnemyMenu()
    {
        PrevMenu = CurrentMenu;
        CurrentMenu = BattleMenuItem.Enemy;
        //If choosing an enemy to attack with a special attack
        switch (PrevMenu)
        {
            //Using a normal attack
            case BattleMenuItem.Main:
                listSize = allEnemies.Count;
                for (int i = 0; i < maxListSize; i++)
                {
                    if (i >= listSize)
                    {
                        Text[i].text = "";
                    }
                    else
                    {
                        Text[i].text = allEnemies[i].BattleBehavior.Name;
                    }
                }
                break;

            //Using a special attack
            case BattleMenuItem.Special:
                Action specialAttack = currentCharacter.BattleBehavior.SpecialAbilities[specialIndex];
                //Individual enemy
                if (specialAttack.ActionTarget == ActionTarget.Enemy)
                {
                    listSize = allEnemies.Count;
                    for (int i = 0; i < maxListSize; i++)
                    {
                        if (i >= listSize)
                        {
                            Text[i].text = "";
                        }
                        else
                        {
                            Text[i].text = allEnemies[i].BattleBehavior.Name;
                        }
                    }
                }
                //All enemies
                else if (specialAttack.ActionTarget == ActionTarget.AllEnemies)
                {
                    listSize = 1;
                    Text[0].text = "All";
                    for (int i = 1; i < maxListSize; i++)
                    {
                        Text[i].text = "";
                    }
                }
                break;
        }
    }

    void ItemsMenu()
    {

    }

    void CharacterMenu()
    {

    }

    void UpdateMenu()
    {
        for (int i = 0; i < maxListSize; i++)
        {
            //See which menu the player is currently in
            if (i == currentIndex)
            {
                Text[i].color = new Color(1f, 1f, 1f);
            }
            else
            {
                Text[i].color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
    }

    void Advance()
    {
        switch (CurrentMenu)
        {
            case BattleMenuItem.Main:
                switch (mainIndex)
                {
                    //Attack
                    case 0:
                        EnemyMenu();
                        //Select.Play();
                        break;

                    //Special
                    case 1:
                        SpecialMenu();
                        //Select.Play();
                        break;

                    //Item
                    case 2:
                        ItemsMenu();
                        //Select.Play();
                        break;

                    //Defend
                    case 3:
                        break;
                }
                break;

            //Attack an enemy with a normal or special attack
            case BattleMenuItem.Enemy:
                //Normal attack
                if (PrevMenu == BattleMenuItem.Main)
                {
                    CharacterTurnArgs args = new CharacterTurnArgs
                    {
                        User = currentCharacter,
                        Target = allEnemies[enemyIndex],
                        ActionTarget = ActionTarget.Enemy,
                        ActionIndex = 0,
                        ActionType = ActionType.Attack
                    };
                    OnCharacterTurn(args);
                    Finish();
                }
                //Special attack
                else if (PrevMenu == BattleMenuItem.Special)
                {
                    CharacterTurnArgs args = new CharacterTurnArgs
                    {
                        User = currentCharacter,
                        Target = allEnemies[enemyIndex],
                        ActionTarget = ActionTarget.Enemy,
                        ActionIndex = specialIndex,
                        ActionType = ActionType.Special
                    };
                    OnCharacterTurn(args);
                    Finish();
                }
                break;

            //Choose a special attack
            case BattleMenuItem.Special:
                Action specialAttack1 = currentCharacter.BattleBehavior.SpecialAbilities[specialIndex];
                //Choosing a single or multiple enemies handled in EnemyMenu()
                //Make sure the character has enough SP
                if (currentCharacter.BattleBehavior.Stats.CurrentSP >= specialAttack1.RequiredSP)
                {
                    if (specialAttack1.ActionTarget == ActionTarget.Enemy || specialAttack1.ActionTarget == ActionTarget.AllEnemies)
                    {
                        EnemyMenu();
                        //Select.Play();
                    }
                    else if (specialAttack1.ActionTarget == ActionTarget.PartyMember || specialAttack1.ActionTarget == ActionTarget.Party)
                    {
                        CharacterMenu();
                        //Select.Play();
                    }
                }
                break;

            //Use an item
            case BattleMenuItem.Item:
                if (currentCharacter.Inventory.Count == 0)
                {
                    break;
                }
                Item item = currentCharacter.Inventory[itemIndex];
                CharacterTurnArgs itemArgs = new CharacterTurnArgs
                {
                    User = currentCharacter,
                    Target = currentCharacter,
                    ActionTarget = ActionTarget.PartyMember,
                    ActionIndex = itemIndex,
                    ActionType = ActionType.Item
                };
                OnCharacterTurn(itemArgs);
                Finish();
                break;

            case BattleMenuItem.Player:
                break;

            case BattleMenuItem.AllPlayers:
                break;

            case BattleMenuItem.AllEnemies:
                //Normal attack
                if (PrevMenu == BattleMenuItem.Main)
                {
                    CharacterTurnArgs enemyArgs = new CharacterTurnArgs
                    {
                        User = currentCharacter,
                        Target = null,
                        ActionTarget = ActionTarget.AllEnemies,
                        ActionIndex = 0,
                        ActionType = ActionType.Attack
                    };
                    OnCharacterTurn(enemyArgs);
                    Finish();
                }

                //Special attack
                else if (PrevMenu == BattleMenuItem.Special)
                {
                    CharacterTurnArgs args = new CharacterTurnArgs
                    {
                        User = currentCharacter,
                        Target = null,
                        ActionTarget = ActionTarget.AllEnemies,
                        ActionIndex = specialIndex,
                        ActionType = ActionType.Special
                    };
                    OnCharacterTurn(args);
                    Finish();
                }
                break;
        }
        UpdateMenu();
    }

    void OnCharacterTurn(CharacterTurnArgs args)
    {
        if (StartCharacterTurn != null)
        {
            StartCharacterTurn(args);
        }
    }

    void Back()
    {
        switch (CurrentMenu)
        {
            case BattleMenuItem.Main:
                break;

            case BattleMenuItem.Enemy:
                if (PrevMenu == BattleMenuItem.Main)
                {
                    MainMenu();
                }
                else if (PrevMenu == BattleMenuItem.Special)
                {
                    SpecialMenu();
                }
                break;

            case BattleMenuItem.Special:
                MainMenu();
                break;

            case BattleMenuItem.Item:
                MainMenu();
                break;

            case BattleMenuItem.Player:
                break;

            case BattleMenuItem.AllPlayers:
                break;

            case BattleMenuItem.AllEnemies:
                break;
        }
        UpdateMenu();
    }

    void Finish()
    {
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentIndex++;
            if (currentIndex >= listSize)
                currentIndex -= listSize;
            UpdateMenu();
            //CursorSound.Play();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex += listSize;
            UpdateMenu();
            //CursorSound.Play();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            Advance();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            Back();
        }
	}
}
