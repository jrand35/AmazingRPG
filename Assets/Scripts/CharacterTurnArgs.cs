﻿using UnityEngine;
using System.Collections;

public class CharacterTurnArgs : MonoBehaviour {
    public Battler User { get; set; }
    public Battler Target { get; set; }
    public ActionTarget ActionTarget { get; set; }
    public int ActionIndex { get; set; }
    public ActionType ActionType { get; set; }
}
