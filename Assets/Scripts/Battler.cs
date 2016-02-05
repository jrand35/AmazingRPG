using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Battler : MonoBehaviour
{
    public BattlerID BattlerID;
    public BattlerType BattlerType { get; private set; }
    public BattleBehavior BattleBehavior { get; private set; }
    public IList<Item> Inventory { get; private set; }
    public int BattlerIndex { get; set; }
	// Use this for initialization
	void Start () {

	}

    public void Initialize()
    {
        Inventory = new List<Item>();
        Inventory.Add(new PotionItem { Quantity = 5 });
        switch (BattlerID)
        {
            case BattlerID.Character1:
                BattlerType = BattlerType.Character;
                BattleBehavior = new JoshBehavior(this);
                BattleBehavior.Initialize();
                break;

            case BattlerID.Character2:
                BattlerType = BattlerType.Character;
                BattleBehavior = new SteveBehavior(this);
                BattleBehavior.Initialize();
                break;

            case BattlerID.Rat:
                BattlerType = BattlerType.Enemy;
                BattleBehavior = new RatBehavior(this);
                BattleBehavior.Initialize();
                break;
        }
    }

    //These two functions are used in BattleBehavior
    //public void RestoreHP(Battler user, int amount)
    //{
    //    BattleBehavior.Stats.CurrentHP += amount;
    //    if (BattleBehavior.Stats.CurrentHP > BattleBehavior.Stats.MaxHP)
    //    {
    //        BattleBehavior.Stats.CurrentHP = BattleBehavior.Stats.MaxHP;
    //    }
    //}

    //public void TakeDamage(Battler user, int amount)
    //{
    //    Debug.Log(BattleBehavior.Name + " took " + amount.ToString() + " damage");
    //    BattleBehavior.Stats.CurrentHP -= amount;
    //    if (BattleBehavior.Stats.CurrentHP < 0)
    //    {
    //        BattleBehavior.Stats.CurrentHP = 0;
    //    }
    //}
	
	// Update is called once per frame
	void Update () {
	
	}
}
