using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattleMenu : MonoBehaviour {
    public Text[] Text;
    public BattleMenuItem CurrentMenu;
    public BattleMenuItem PrevMenu;
    public Battler currentCharacter;
    public IList<Battler> allCharacters;
    public IList<Battler> allEnemies;
    private int mainIndex;
    private int specialIndex;
    private int itemIndex;
    private int charIndex;
    private int enemyIndex;
    private int listSize;
    private int maxListSize = 6;
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

    void MainMenu()
    {

    }

    void UpdateMenu()
    {

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
