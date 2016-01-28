using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Lifebar : MonoBehaviour {
    public Text HPNumber;
    public Battler Character
    {
        get
        {
            return character;
        }
        set
        {
            character = value;
            character.BattleBehavior.HPChanged += UpdateLifebar;
            UpdateLifebar(character.BattleBehavior.Stats.CurrentHP, character.BattleBehavior.Stats.MaxHP);
        }
    }
    private Battler character;

    void UpdateLifebar(int newHP, int maxHP)
    {
        HPNumber.text = newHP.ToString() +" / " + maxHP.ToString();
    }
	void Start () {
	    
	}

    void OnDisable()
    {
        if (character != null)
            character.BattleBehavior.HPChanged -= UpdateLifebar;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
