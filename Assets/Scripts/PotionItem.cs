using UnityEngine;
using System.Collections;

public class PotionItem : Item {
    public PotionItem()
    {
        Name = "Potion";
        Description = "Restores 50 HP";
    }

    public override void Use(Battler user, Battler recipient)
    {
        recipient.RestoreHP(user, 50);
    }
}
