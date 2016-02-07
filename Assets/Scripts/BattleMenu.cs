using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleMenu : MonoBehaviour {
    public event CharacterTurnHandler StartCharacterTurn;
    public Text CharacterName;
    public Text DescriptionText;
    public Text[] Text;
    public IList<Battler> allCharacters { get; set; }
    public IList<Battler> allEnemies { get; set; }
    public GameObject MagicCirclePrefab;
    public GameObject CursorPrefab;
    private GameObject MagicCircle;
    private GameObject CharacterCursor;
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
    private Vector3 magicCircleInitScale;
    private float cursorY;
    private bool cursorHover;
    private bool onTurn;

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
        Camera.main.hdr = false;
        onTurn = false;
        cursorHover = false;
        //Cursor position above character: 0, 2, 0
        //Magic circle position at character: 0, -2, 0
        CharacterCursor = Instantiate(CursorPrefab) as GameObject;
        MagicCircle = Instantiate(MagicCirclePrefab) as GameObject;
        MagicCircle.SetActive(false);
        CharacterCursor.SetActive(false);
        magicCircleInitScale = MagicCircle.transform.localScale;
        mainIndex = 0;
        specialIndex = 0;
        itemIndex = 0;
        charIndex = 0;
        enemyIndex = 0;
        listSize = 0;
        MainMenu();
        UpdateMenu();
	}

    IEnumerator SpinCircle()
    {
        MagicCircle.SetActive(true);
        Material MagicCircleMaterial = MagicCircle.GetComponent<MeshRenderer>().material;
        float angle = 0f;
        float alphaCount = 0f;
        for (int i = 0; i < 25; i++)
        {
            angle += 3f;
            Vector3 targetScale = magicCircleInitScale * 0.85f;
            Vector3 scale = magicCircleInitScale + (targetScale - magicCircleInitScale * (i / 25f));
            MagicCircle.transform.localScale = scale;
            MagicCircle.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            float alpha = 0.6f * (i / 25f);
            MagicCircleMaterial.SetColor("_TintColor", new Color(1f, 1f, 1f, alpha));
            yield return 0;
        }
        while (onTurn)
        {
            angle += 3f;
            alphaCount += 0.05f; //0.1f;
            float alpha = 0.4f + 0.2f * Mathf.Cos(alphaCount);
            Vector3 scale = magicCircleInitScale + (magicCircleInitScale * -0.15f * Mathf.Cos(alphaCount));
            MagicCircle.transform.localScale = scale;
            MagicCircle.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            MagicCircleMaterial.SetColor("_TintColor", new Color(1f, 1f, 1f, alpha));
            yield return 0;
        }
        MagicCircle.SetActive(false);
    }

    IEnumerator CursorHover()
    {
        float time = 0;
        while (onTurn)
        {
            if (cursorHover)
            {
                time += 0.1f;
                cursorY = 0.5f * Mathf.Sin(time);
                Battler selected = null;
                if (CurrentMenu == BattleMenuItem.Player)
                {
                    selected = allCharacters[charIndex];
                }
                else if (CurrentMenu == BattleMenuItem.Enemy)
                {
                    selected = allEnemies[enemyIndex];
                }
                //CharacterCursor.transform.localPosition = selected.gameObject.transform.position + new Vector3(0f, 2f + cursorY, 0f);
                Renderer r = selected.gameObject.GetComponentInChildren<Renderer>();
                Debug.Log(r.bounds.size.y);
                CharacterCursor.transform.localPosition = selected.gameObject.transform.position + new Vector3(0f, r.bounds.size.y + 1.5f + cursorY - selected.gameObject.transform.position.y, 0f);
            }
            else
            {
                time = 0;
            }
            yield return 0;
        }
        //while(gameObject.)
    }

    public void CharacterTurn(Battler character)
    {
        //Only attack enemies that are alive
        allEnemies = allEnemies.Where(e => e.BattleBehavior.Status.StatusEffect != StatusEffect.Defeated).ToList();

        onTurn = true;
        cursorHover = false;
        gameObject.SetActive(true);
        currentCharacter = character;
        mainIndex = 0;
        specialIndex = 0;
        itemIndex = 0;
        charIndex = 0;
        enemyIndex = 0;
        MainMenu();
        UpdateMenu();
        UpdateCharacterName();
        Vector3 circlePos = currentCharacter.transform.position;
        circlePos.y = 0.25f;
        MagicCircle.transform.position = circlePos;//= currentCharacter.transform.position + new Vector3(0f, -2f, 0f);
        StartCoroutine(SpinCircle());
        StartCoroutine(CursorHover());
    }

    void UpdateCharacterName()
    {
        switch (CurrentMenu)
        {
            case BattleMenuItem.Main:
                CharacterName.text = currentCharacter.BattleBehavior.Name;
                break;

            case BattleMenuItem.Enemy:
                //Using a normal attack
                if (PrevMenu == BattleMenuItem.Main)
                {
                    CharacterName.text = currentCharacter.BattleBehavior.Name + " > Attack";
                }
                //Using a special attack
                else if (PrevMenu == BattleMenuItem.Special)
                {
                    CharacterName.text = currentCharacter.BattleBehavior.Name + " > Special > " + currentCharacter.BattleBehavior.SpecialAbilities[specialIndex].Name;
                }
                break;

            case BattleMenuItem.Special:
                CharacterName.text = currentCharacter.BattleBehavior.Name + " > Special";
                break;

            case BattleMenuItem.Item:
                CharacterName.text = currentCharacter.BattleBehavior.Name + " > Item";
                break;

            case BattleMenuItem.Player:
                //Using an item
                if (PrevMenu == BattleMenuItem.Item)
                {
                    CharacterName.text = currentCharacter.BattleBehavior.Name + " > Item > " + currentCharacter.Inventory[itemIndex].Name;
                }
                //Using a special healing ability
                else if (PrevMenu == BattleMenuItem.Special)
                {
                    CharacterName.text = currentCharacter.BattleBehavior.Name + " > Special > " + currentCharacter.BattleBehavior.SpecialAbilities[specialIndex].Name;
                }
                break;

            case BattleMenuItem.AllPlayers:
                break;

            case BattleMenuItem.AllEnemies:
                break;
        }
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
        IList<Action> specialAbilities = currentCharacter.BattleBehavior.SpecialAbilities;
        PrevMenu = CurrentMenu;
        CurrentMenu = BattleMenuItem.Special;

        listSize = specialAbilities.Count;
        for (int i = 0; i < maxListSize; i++)
        {
            if (i >= listSize)
            {
                Text[i].text = "";
            }
            else
            {
                Text[i].text = specialAbilities[i].Name + " (" + specialAbilities[i].RequiredSP + ")";
            }
        }
    }

    void EnemyMenu()
    {
        cursorHover = true;
        CharacterCursor.SetActive(true);
        PrevMenu = CurrentMenu;
        CurrentMenu = BattleMenuItem.Enemy;
        MoveCursor(allEnemies[enemyIndex]);
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
        IList<Item> inventory = currentCharacter.Inventory;
        PrevMenu = CurrentMenu;
        CurrentMenu = BattleMenuItem.Item;

        if (inventory.Count > 0)
        {
            listSize = inventory.Count;
            for (int i = 0; i < maxListSize; i++)
            {
                if (i >= listSize)
                {
                    Text[i].text = "";
                }
                else
                {
                    Text[i].text = inventory[i].Name + " (" + inventory[i].Quantity + ")";
                }
            }
        }
        else
        {
            listSize = 1;
            Text[0].text = "No items";
            for (int i = 1; i < maxListSize; i++)
            {
                Text[i].text = "";
            }
        }
    }

    void MoveCursor(Battler character)
    {
        Renderer r = character.gameObject.GetComponentInChildren<Renderer>();
        CharacterCursor.transform.localPosition = character.gameObject.transform.position + new Vector3(0f, r.bounds.size.y + 1.5f + cursorY - character.gameObject.transform.position.y, 0f);
    }

    void CharacterMenu()
    {
        cursorHover = true;
        CharacterCursor.SetActive(true);
        PrevMenu = CurrentMenu;
        CurrentMenu = BattleMenuItem.Player;
        IList<Battler> characterList = allCharacters;
        // if choosing a healing special ability that can only target live party members
        if (PrevMenu == BattleMenuItem.Special)
        {
            Action specialAttack = currentCharacter.BattleBehavior.SpecialAbilities[specialIndex];
            if (specialAttack.ActionTarget == ActionTarget.LivePartyMember)
            {
                characterList = allCharacters.Where(c => c.BattleBehavior.Status.StatusEffect != StatusEffect.Defeated).ToList();
            }
        }
        listSize = characterList.Count;
        if (charIndex >= listSize)
        {
            charIndex = listSize - 1;
        }
        MoveCursor(characterList[charIndex]);

        for (int i = 0; i < maxListSize; i++)
        {
            if (i >= listSize)
            {
                Text[i].text = "";
            }
            else
            {
                Text[i].text = characterList[i].BattleBehavior.Name;
            }
        }
    }

    void UpdateMenu()
    {
        //Update description
        switch (CurrentMenu)
        {
            case BattleMenuItem.Main:
                switch (mainIndex)
                {
                    //Attack
                    case 0:
                        DescriptionText.text = "Attack with equipped weapon.";
                        break;

                    //Special
                    case 1:
                        DescriptionText.text = "Use a special ability.";
                        break;

                    //Item
                    case 2:
                        DescriptionText.text = "Use an item.";
                        break;

                    //Defend
                    case 3:
                        DescriptionText.text = "Reduce the amount of damage taken.";
                        break;
                }
                break;

            case BattleMenuItem.Enemy:
                DescriptionText.text = "";
                break;

            case BattleMenuItem.Special:
                DescriptionText.text = currentCharacter.BattleBehavior.SpecialAbilities[specialIndex].Description;
                break;

            case BattleMenuItem.Item:
                DescriptionText.text = currentCharacter.Inventory[itemIndex].Description;
                break;

            case BattleMenuItem.Player:
                DescriptionText.text = "";
                break;

            case BattleMenuItem.AllPlayers:
                DescriptionText.text = "";
                break;

            case BattleMenuItem.AllEnemies:
                DescriptionText.text = "";
                break;
        }
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

        if (CurrentMenu == BattleMenuItem.Player)
        {
            MoveCursor(allCharacters[charIndex]);
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
                        TurnArgs defendArgs = new TurnArgs
                        {
                            User = currentCharacter,
                            Target = null,
                            ActionTarget = ActionTarget.PartyMember,
                            ActionIndex = 0,
                            ActionType = ActionType.Defend
                        };
                        OnCharacterTurn(defendArgs);
                        Finish();
                        break;
                }
                break;

            //Attack an enemy with a normal or special attack
            case BattleMenuItem.Enemy:
                //Normal attack
                if (PrevMenu == BattleMenuItem.Main)
                {
                    TurnArgs args = new TurnArgs
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
                    TurnArgs args = new TurnArgs
                    {
                        User = currentCharacter,
                        Target = allEnemies[enemyIndex],
                        ActionTarget = ActionTarget.Enemy,
                        ActionIndex = specialIndex,
                        ActionType = ActionType.Special,
                        Action = currentCharacter.BattleBehavior.SpecialAbilities[specialIndex]
                    };
                    OnCharacterTurn(args);
                    Finish();
                }
                cursorHover = false;
                CharacterCursor.SetActive(false);
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
                    else if (specialAttack1.ActionTarget == ActionTarget.PartyMember || specialAttack1.ActionTarget == ActionTarget.LivePartyMember || specialAttack1.ActionTarget == ActionTarget.Party)
                    {
                        CharacterMenu();
                        //Select.Play();
                    }
                }
                break;

            //Use an item
            case BattleMenuItem.Item:
                //TODO: Check if an item affects 1 or multiple party members
                if (currentCharacter.Inventory.Count > 0)
                    CharacterMenu();
                break;

            case BattleMenuItem.Player:
                if (currentCharacter.Inventory.Count == 0)
                {
                    break;
                }
                if (PrevMenu == BattleMenuItem.Item)
                {
                    Item item = currentCharacter.Inventory[itemIndex];
                    TurnArgs itemArgs = new TurnArgs
                    {
                        User = currentCharacter,
                        Target = allCharacters[charIndex],
                        ActionTarget = ActionTarget.PartyMember,
                        ActionIndex = itemIndex,
                        ActionType = ActionType.Item
                    };
                    OnCharacterTurn(itemArgs);
                }
                else if (PrevMenu == BattleMenuItem.Special)
                {
                    TurnArgs args = new TurnArgs
                    {
                        User = currentCharacter,
                        Target = allCharacters[charIndex],
                        ActionTarget = ActionTarget.PartyMember,
                        ActionIndex = specialIndex,
                        ActionType = ActionType.Special,
                        Action = currentCharacter.BattleBehavior.SpecialAbilities[specialIndex]
                    };
                    OnCharacterTurn(args);
                }
                cursorHover = false;
                CharacterCursor.SetActive(false);
                Finish();
                break;

            case BattleMenuItem.AllPlayers:
                break;

            case BattleMenuItem.AllEnemies:
                //Normal attack
                if (PrevMenu == BattleMenuItem.Main)
                {
                    TurnArgs enemyArgs = new TurnArgs
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
                    TurnArgs args = new TurnArgs
                    {
                        User = currentCharacter,
                        Target = null,
                        ActionTarget = ActionTarget.AllEnemies,
                        ActionIndex = specialIndex,
                        ActionType = ActionType.Special,
                        Action = currentCharacter.BattleBehavior.SpecialAbilities[specialIndex]
                    };
                    OnCharacterTurn(args);
                    Finish();
                }
                break;
        }
        UpdateMenu();
        UpdateCharacterName();
    }

    void OnCharacterTurn(TurnArgs args)
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
                cursorHover = false;
                CharacterCursor.SetActive(false);
                break;

            case BattleMenuItem.Special:
                MainMenu();
                break;

            case BattleMenuItem.Item:
                MainMenu();
                break;

            case BattleMenuItem.Player:
                if (PrevMenu == BattleMenuItem.Item)
                {
                    ItemsMenu();
                }
                else if (PrevMenu == BattleMenuItem.Special)
                {
                    SpecialMenu();
                }
                cursorHover = false;
                CharacterCursor.SetActive(false);
                break;

            case BattleMenuItem.AllPlayers:
                break;

            case BattleMenuItem.AllEnemies:
                break;
        }
        UpdateMenu();
        UpdateCharacterName();
    }

    public void Finish()
    {
        onTurn = false;
        MagicCircle.SetActive(false);
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
