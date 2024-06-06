
using System.Collections.Generic;
using UnityEngine;

public class LootTable : MonoBehaviour
{
    [SerializeField]
    protected Loot[] loot;

    private List<Item> droppedItems = new List<Item>();

    public List<Drops> MyDroppedItems { get; set; }

    private bool rolled = false;


    public List<Drops> GetLoot()
    {
        if (!rolled)
        {
            MyDroppedItems = new List<Drops>();
            RollLoot();
        }

        return MyDroppedItems;
    }

    protected virtual void RollLoot()
    {
        foreach (Loot item in loot)
        {
            int roll = Random.Range(0, 100);
            if (roll <= item.MyDropChance)
            {
                MyDroppedItems.Add( new Drops(item.MyItem,this));
            }
        }

        rolled = true;
    }
}
