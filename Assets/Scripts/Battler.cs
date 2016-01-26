using UnityEngine;
using System.Collections;

public class Battler : MonoBehaviour
{
    public BattlerID BattlerID;
    public BattleBehavior BattleBehavior { get; private set; }
	// Use this for initialization
	void Start () {
        switch (BattlerID)
        {
            case BattlerID.Character:
                BattleBehavior = new CharacterBehavior(this);
                break;

            case BattlerID.Enemy:
                BattleBehavior = new EnemyBehavior(this);
                break;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
