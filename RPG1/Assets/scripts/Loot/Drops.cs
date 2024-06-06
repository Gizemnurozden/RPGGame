
public class Drops 
{
    public Item MyItem { get; set; }

    public LootTable MyLootTable { get; set; }

    public Drops(Item item, LootTable lootTable)
    {
        MyLootTable = lootTable;
        MyItem = item;
    }

    public void Remove()
    {
        MyLootTable.MyDroppedItems.Remove(this);
    }
}
