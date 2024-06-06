using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestLog : MonoBehaviour
{

    [SerializeField]
    private GameObject questPrefab;

    [SerializeField]
    private Transform questParent;

    private Quest selected;

    [SerializeField]
    private Text questDescription;

    private static QuestLog instance;

    private List<Quest> quests = new List<Quest>();

    private List<QuestScript> questScripts = new List<QuestScript>();

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Text questCountTxt;

    [SerializeField]
    private int maxCount;

   
    private int currentCount;

    public static QuestLog MyInstance
    { get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<QuestLog>();

            }
           return instance;
        }
    }

    public List<Quest> MyQuests { get => quests; set => quests = value; }

    private void Start()
    {
        questCountTxt.text = currentCount + "/" + maxCount;
    }

    

    public void AcceptQuest(Quest quest)
    {
        if (currentCount < maxCount)
        {
            currentCount++;

            questCountTxt.text = currentCount + "/" + maxCount;


            foreach (CollectObjective o in quest.MyCollectObjectives)
            {
                InventoryScript.MyInstance.itemCountChangedEvent += new ItemCountChanged(o.UpdateItemCount);

                o.UpdateItemCount(); //yapılmış görev direkt yapıldı olarak gözüksün diye
            }

            foreach (KillObjective o in quest.MyKillObjectives)
            {
                GameManager.MyInstance.killConfirmedEvent += new KillConfirmed(o.UpdateKillCount);
            }
            MyQuests.Add(quest);

            GameObject go = Instantiate(questPrefab, questParent);

            QuestScript qs = go.GetComponent<QuestScript>();
            quest.MyQuestScript = qs;
            qs.MyQuest = quest;
            questScripts.Add(qs);


            go.GetComponent<Text>().text = quest.MyTitle;

            CheckCompletion(); //görev yapıldı mı diye kontrol
        }

        
    }



    public void UpdateSelected()
    {
        ShowDescription(selected);
    }



    public void ShowDescription(Quest quest)
    {
        if (quest != null)
        {

            if (selected != null && selected != quest)
            {
                selected.MyQuestScript.DeSelect();
            }
            string objectives = string.Empty;

            selected = quest;

            string title = quest.MyTitle;

            foreach (Objective obj in quest.MyCollectObjectives)
            {
                objectives += obj.Mytype + ": " + obj.MyCurrentAmount + "/" + obj.MyAmount + "\n";
            }

            questDescription.text = string.Format("<b>{0}</b>\n<size=10>{1}</size>\nObjectives\n<size=10>{2}</size>", title, quest.MyDescription, objectives);

            foreach (Objective obj in quest.MyKillObjectives)
            {
                objectives += obj.Mytype + ": " + obj.MyCurrentAmount + "/" + obj.MyAmount + "\n";
            }

            questDescription.text = string.Format("<b>{0}</b>\n<size=10>{1}</size>\nObjectives\n<size=10>{2}</size>", title, quest.MyDescription, objectives);

        }

    }

    public void CheckCompletion()
    {
        foreach (QuestScript qs in questScripts )
        {
            qs.MyQuest.MyQuestGiver.UpdateQuestStatus();

            qs.IsComplete();
        }
    }

    public void OpenClose()
    {
        if (canvasGroup.alpha == 1)
        {

            Close();

        }
        else
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
        }
    }
    public void Close()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    public void AbandonQuest() //quest logdan kaldır. listeden soruyu kaldırdığını hatırlaöası için.
    {
        foreach (CollectObjective o in selected.MyCollectObjectives)
        {
            Debug.Log("awsdd");
            InventoryScript.MyInstance.itemCountChangedEvent -= new ItemCountChanged(o.UpdateItemCount);
            o.Complete();
        }

        foreach (KillObjective o in selected.MyKillObjectives)
        {
            GameManager.MyInstance.killConfirmedEvent -= new KillConfirmed(o.UpdateKillCount);
        }

        RemoveQuest(selected.MyQuestScript);
    }

    public void RemoveQuest(QuestScript qs)
    {
        questScripts.Remove(qs);
        Destroy(qs.gameObject);
        MyQuests.Remove(qs.MyQuest);
        questDescription.text = string.Empty;
        selected = null; //deselected
        currentCount--;
        questCountTxt.text = currentCount + "/" + maxCount;
        qs.MyQuest.MyQuestGiver.UpdateQuestStatus();
        qs = null;

    }

    public bool HasQuest(Quest quest) //zaten var olan ve kabul ettiğim görevleri tekrar vermemek için.
    {
        return MyQuests.Exists(x => x.MyTitle == quest.MyTitle);
    }
}
