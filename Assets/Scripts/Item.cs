using UnityEngine;
using System.Collections;

public abstract class Item : MonoBehaviour {
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public int Quantity { get; set; }
    public virtual void Use(Battler user, Battler recipient)
    {

    }
    public void RemoveFromInventory(Battler user)
    {
        Quantity--;
        if (Quantity <= 0)
        {
            user.Inventory.Remove(this);
        }
    }
}
