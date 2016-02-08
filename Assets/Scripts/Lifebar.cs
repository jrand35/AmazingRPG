using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Lifebar : MonoBehaviour {
    public Text HPNumber;
    public Text SPNumber;
    public Text characterName;
    public Text statusText;
    public Image portrait;
    public Battler Character
    {
        get
        {
            return character;
        }
        set
        {
            character = value;
            character.BattleBehavior.HPChanged += UpdateHP;
            character.BattleBehavior.SPChanged += UpdateSP;
            BattleBehavior.Burned += BurnStatus;
            BattleBehavior.Death += DeathStatus;
            UpdateHP(character.BattleBehavior.Stats.CurrentHP, character.BattleBehavior.Stats.MaxHP);
            UpdateSP(character.BattleBehavior.Stats.CurrentSP, character.BattleBehavior.Stats.MaxSP);
            characterName.text = character.BattleBehavior.Name;

            switch (character.BattlerID)
            {
                //Josh
                case BattlerID.Character1:
                    portrait.sprite = Resources.Load<Sprite>("JoshPortrait");
                    break;

                //Caina
                case BattlerID.Character2:
                    break;

                //Steve
                case BattlerID.Character3:
                    portrait.sprite = Resources.Load<Sprite>("StevePortrait");
                    break;
            }
        }
    }
    private Battler character;

    void UpdateHP(int newHP, int maxHP)
    {
        HPNumber.text = newHP.ToString() + " / " + maxHP.ToString();
    }

    void UpdateSP(int newSP, int maxSP)
    {
        SPNumber.text = newSP.ToString() + " / " + maxSP.ToString();
    }

    //Only burn is implemented
    void BurnStatus(Battler battler)
    {
        if (battler == Character)
        {
            statusText.text = "Burn";
            statusText.color = new Color(1f, 0.25f, 0f);
        }
    }

    void DeathStatus(BattleBehavior behavior, BattlerType type)
    {
        if (behavior.Name == Character.BattleBehavior.Name)
        {
            statusText.text = "Fainted";
            statusText.color = Color.white * 0.85f;
        }
    }

	void Start () {
        statusText.text = "";
	}

    void OnDisable()
    {
        if (character != null)
        {
            character.BattleBehavior.HPChanged -= UpdateHP;
            character.BattleBehavior.SPChanged -= UpdateSP;
            BattleBehavior.Burned -= BurnStatus;
            BattleBehavior.Death -= DeathStatus;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
