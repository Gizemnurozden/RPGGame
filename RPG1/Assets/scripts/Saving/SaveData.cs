using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData //oyunun kaydedilmiş verileri
{
    

    public PlayerData MyPlayerData { get; set; }

    public List<ChestData> MyChestData { get; set; }

    public List<EquipmentData> MyEquipmentData { get; set; }

    public InventoryData MyInventoryData { get; set; }

    public List<ActionButtonData> MyActionButtonData { get; set; }

    public List<QuestData> MyQuestData { get; set; }

    public List<QuestGiverData> MyQuestGiverData { get; set; }

    public DateTime MyDateTime { get; set; }

    public string MyScene { get; set; }

    public SaveData() 
    {
        MyInventoryData = new InventoryData();
        MyChestData = new List<ChestData>();
        MyActionButtonData = new List<ActionButtonData>();
        MyEquipmentData = new List<EquipmentData>();
        MyQuestData = new List<QuestData>();
        MyQuestGiverData = new List<QuestGiverData>();
        MyDateTime = DateTime.Now;
    }

}

[Serializable]
public class PlayerData //oyuncu verilerini kaydeder.
{
    public int MyLevel { get; set; } //leveli kaydetmek için

    public float MyXp { get; set; }

    public float MyMaxXP { get; set; }

    public float MyHealth { get; set; }

    public float MyMaxHealth { get; set; }

    public float MyMana { get; set; }

    public float MyMaxMana { get; set; }

    public float MyX { get; set; }

    public float MyY { get; set; }

   
    public PlayerData(int level, float xp, float maxXp,float health,float maxHealth, float mana, float maxMana,Vector2 position)
    {
        this.MyLevel = level;
        this.MyXp = xp;
        this.MyMaxXP = maxXp;
        this.MyHealth = health;
        this.MyMaxHealth = maxHealth;
        this.MyMana = mana;
        this.MyMaxMana = maxMana;
        this.MyX = position.x;
        this.MyY = position.y;
    }
}

[Serializable]
public class ItemData  //itemlerin verileri
{
    public string MyTitle { get; set; }

    public int MyStackCount { get; set; }

    public int  MySlotIndex { get; set; }

    public int MyBagIndex { get; set; }

    public ItemData(string title, int stackCount = 0,int slotIndex = 0, int bagIndex=0 )
    {
        MyTitle = title;
        MyStackCount = stackCount;
        MySlotIndex = slotIndex;
        MyBagIndex = bagIndex;
        
    }
    
}

[Serializable]
public class ChestData //hazine verileri
{
    public string MyName { get; set; }

    public List<ItemData> MyItems { get; set; }

    public ChestData(string name)
    {
        MyName = name;

        MyItems = new List<ItemData>();
    }
}

[Serializable]
public class InventoryData //çanta için inventory
{
    public List<BagData> MyBags { get; set; }

    public List<ItemData> MyItems { get; set; }

    public InventoryData()
    {
        MyBags = new List<BagData>();
        MyItems = new List<ItemData>();
    }
}

[Serializable]
public class BagData //çanta verileri
{
    public int MySlotCount { get; set; }

    public int MyBagIndex { get; set; }

    public BagData(int count, int index)
    {
        MySlotCount = count;
        MyBagIndex = index;
    }
}

[Serializable]
public class EquipmentData //karakter paneli için ekipmanlar verileri
{
    public string MyTitle { get; set; }

    public string MyType { get; set; }

    public EquipmentData(string title, string  type)
    {
        MyTitle = title;
        MyType = type;
    }
}

[Serializable]
public class ActionButtonData
{
    public string MyAction { get; set; }

    public bool IsItem { get; set; }

    public int MyIndex { get; set; }

    public ActionButtonData(string action, bool isItem, int index)
    {
        this.MyAction = action;
        this.IsItem = IsItem;
        this.MyIndex = index;
    }
}

[Serializable]
public class QuestData
{
    public string MyTitle { get; set; }

    public string MyDescription { get; set; }

    public CollectObjective[] MyCollectObjectives { get; set; }

    public KillObjective[] MyKillObjectives { get; set; }

    public int MyQuestGiverID { get; set; }

   

    public QuestData(string title, string description, CollectObjective[] collectObjectives, KillObjective[] killObjective,int questGiverID)
    {
        MyTitle = title;
        MyDescription = description;
        MyCollectObjectives = collectObjectives;
        MyKillObjectives = killObjective;
        MyQuestGiverID = questGiverID;
     
    }
}

[Serializable]
public class QuestGiverData
{
    public List<string> MyCompletedQuests { get; set; }

    public int MyQuestGiverID { get; set; }

    public QuestGiverData(int questGiverID, List<string> completedQuests)
    {
        this.MyQuestGiverID = questGiverID;
        MyCompletedQuests = completedQuests;
    }
}