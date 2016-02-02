using UnityEngine;
using System.Collections;

public delegate void CharacterTurnHandler(CharacterTurnArgs args);
public delegate void HPTextHandler(Battler battler, int damage);
public delegate void CharacterHPHandler(int newHP, int maxHP);