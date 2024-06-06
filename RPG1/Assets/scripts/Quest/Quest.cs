
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class Quest 
{
    [SerializeField]
    private string title;

    [SerializeField]
    private string description;

    [SerializeField]
    private CollectObjective[] collectObjectives;

    [SerializeField]
    private KillObjective[] killObjectives;

    public string MyTitle { get => title; set => title = value; }

    public QuestScript MyQuestScript { get; set; }

    public QuestGiver MyQuestGiver { get; set; }

    [SerializeField]
    private int level;
    [SerializeField]
    private int xp;


    public string MyDescription { get => description; set => description = value; }

    public CollectObjective[] MyCollectObjectives { get => collectObjectives;  }

    public bool IsComplete
    {
        get
        {
            foreach (Objective o in collectObjectives)
            {
                if (!o.IsComplete)
                {
                    return false;
                }
            }

            foreach (Objective o in MyKillObjectives)
            {
                if (!o.IsComplete)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public KillObjective[] MyKillObjectives { get => killObjectives; set => killObjectives = value; }
    public int MyLevel { get => level; set => level = value; }
    public int MyXp { get => xp;  }
}

[System.Serializable]
public abstract class Objective
{
    [SerializeField]
    private int amount;

    private int currentAmount;

    [SerializeField]
    private string type;

    public int MyAmount
    {
        get
        {
            return amount;
        }
       

    }
    public int MyCurrentAmount
    {
        get
        {
            return currentAmount;
        }
        set
        {
            currentAmount = value;
        }
    }

    public string Mytype
    {
        get
        {
            return type;
        }
    }
    public bool IsComplete
    {
        get
        {
            return MyCurrentAmount >= MyAmount;
        }
    }
}


[System.Serializable]
public class CollectObjective : Objective
{
    public void UpdateItemCount(Item item)
    {
        if (Mytype.ToLower() == item.MyTitle.ToLower())
        {
            MyCurrentAmount = InventoryScript.MyInstance.GetItemCount(item.MyTitle);

            if (MyCurrentAmount <= MyAmount)
            {
                MessageFeedManager.MyInstance.WriteMessage(string.Format("{0}: {1}/{2}",item.MyTitle, MyCurrentAmount, MyAmount)); //görevi yaparken ekledniğinde ekrana yazdırma için çapırdık.

            }

            QuestLog.MyInstance.UpdateSelected();
            QuestLog.MyInstance.CheckCompletion();
        }
        
    }


    public void UpdateItemCount() //görevimin gerekliliği varsa açtığımda yapıldı şeklinde gözüksün diye
    {
        
            MyCurrentAmount = InventoryScript.MyInstance.GetItemCount(Mytype);
            QuestLog.MyInstance.UpdateSelected();
            QuestLog.MyInstance.CheckCompletion();
        

    }

    public void Complete() //görev bittikten sonra slottan tamamlanan görevin kısmını kaldır.
    {
        Stack<Item> items = InventoryScript.MyInstance.GetItems(Mytype, MyAmount);
  

        foreach (Item item in items)
        {
      
            item.Remove();
        }
    }
}

[System.Serializable]
public class KillObjective : Objective
{

    public void UpdateKillCount(Character character)
    {
        if (Mytype == character.MyType)
        {
            if (MyCurrentAmount < MyAmount)
            {
                MyCurrentAmount++;
                MessageFeedManager.MyInstance.WriteMessage(string.Format("{0}: {1}/{2}", character.MyType, MyCurrentAmount, MyAmount)); //görevi yaparken ekledniğinde ekrana yazdırma için çapırdık.

                QuestLog.MyInstance.UpdateSelected();
                QuestLog.MyInstance.CheckCompletion();
            }
            
        }
    }
}