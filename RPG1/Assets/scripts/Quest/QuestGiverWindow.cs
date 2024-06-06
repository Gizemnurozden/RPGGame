using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestGiverWindow : Window
{
    private static QuestGiverWindow instance;

    private QuestGiver questGiver;

    [SerializeField]
    private GameObject questPrefab;

    [SerializeField]
    private Transform questArea;

    [SerializeField]
    private GameObject backBtn, acceptBtn, completeBtn, questDescription;

  

    private List<GameObject> quests = new List<GameObject>();

    private Quest selectedQuest;

    public static QuestGiverWindow MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<QuestGiverWindow>();
            }
            return instance;
        }
    }

    public void ShowQuest(QuestGiver questGiver)
    {
        this.questGiver = questGiver;

        foreach (GameObject go in quests)
        {
            Destroy(go);
        }

        questArea.gameObject.SetActive(true);
        questDescription.SetActive(false);

        foreach (Quest quest in questGiver.MyQuests)
        {
            
            if (quest != null)
            {
                GameObject go = Instantiate(questPrefab, questArea); //soruları al

                go.GetComponent<Text>().text = " [" + quest.MyLevel + "] " +quest.MyTitle + "<color=#ffbb04> <size=9>! </size> </color>"; //görevimin başlığı 

                go.GetComponent<QGQuestScript>().MyQuest = quest; //soru scriptim 

                quests.Add(go); //soruyu ekle



               if (QuestLog.MyInstance.HasQuest(quest) && quest.IsComplete) //eğer görev tamamlanmışsa
                {
                    go.GetComponent<Text>().text = quest.MyTitle + "<color=#ffbb04><size=9>?</size></color>"; //yanına C yazdır
                }
                else if (QuestLog.MyInstance.HasQuest(quest)) //eğer görevi seçmişsem
                {
                    Color c = go.GetComponent<Text>().color; //renigini soldur

                    c.a = 0.5f;
                    go.GetComponent<Text>().color = c;
                    go.GetComponent<Text>().text = quest.MyTitle + "<color=#c0c0c0ff><size=9>?</size></color>";

                }
            }
            
           
        }
    }

    public override void Open(NPC npc)
    {
        ShowQuest(npc as QuestGiver);
        base.Open(npc);
    }

    public void ShowQuestInfo(Quest quest)
    {

        this.selectedQuest = quest;

        if (QuestLog.MyInstance.HasQuest(quest) && quest.IsComplete)
        {
            acceptBtn.SetActive(false);
            completeBtn.SetActive(true);
        }
        else if (!QuestLog.MyInstance.HasQuest(quest))
        {
            acceptBtn.SetActive(true);  
        }

        backBtn.SetActive(true);
       
        questArea.gameObject.SetActive(false);
        questDescription.SetActive(true);

        
        
        string description = quest.MyDescription;

        string objectives = string.Empty;

        foreach (Objective obj in quest.MyCollectObjectives) 
        {
            objectives += obj.Mytype + ": " + obj.MyCurrentAmount + "/" + obj.MyAmount + "\n";
        }

        questDescription.GetComponent<Text>().text = string.Format("<b>{0}</b>\n<size=9>{1}</size><size=9>{2}</size>", quest.MyTitle, description, objectives);

    }

    public void Back()
    {
        backBtn.SetActive(false);
        acceptBtn.SetActive(false);
        completeBtn.SetActive(false);
        ShowQuest(questGiver);
    }

    public void Accept()
    {
        QuestLog.MyInstance.AcceptQuest(selectedQuest);
        Back();

    }

    public override void Close() //complete butonu kaldırmak için.
    {
        completeBtn.SetActive(false);
        base.Close();
    }

    public void CompleteQuest() //görevi tamamlama fonksiyonu
    {
        if (selectedQuest.IsComplete)
        {
            for (int i = 0; i < questGiver.MyQuests.Length; i++)
            {
                if (selectedQuest == questGiver.MyQuests[i])
                {
                    questGiver.MyCompltedQuests.Add(selectedQuest.MyTitle);
                    questGiver.MyQuests[i] = null;
                    selectedQuest.MyQuestGiver.UpdateQuestStatus();
                }
            }
        }
         //bu döngüler görevler tamamlandıktan sonra görevleri yapınca mesaj olarak ekranda gözükmeye devam etmesin diye eklendi.
        foreach (CollectObjective o in selectedQuest.MyCollectObjectives)
        {
           
            InventoryScript.MyInstance.itemCountChangedEvent -= new ItemCountChanged(o.UpdateItemCount);
            o.Complete();
        }

        foreach (KillObjective o in selectedQuest.MyKillObjectives)
        {
           GameManager.MyInstance.killConfirmedEvent -= new KillConfirmed(o.UpdateKillCount);
        }

        Player.MyInstance.GainXP(XPManager.CalculateXP(selectedQuest)); //görevi tamamladıktan sonra puan eklensin.

        QuestLog.MyInstance.RemoveQuest(selectedQuest.MyQuestScript);
        Back();
    }
}
