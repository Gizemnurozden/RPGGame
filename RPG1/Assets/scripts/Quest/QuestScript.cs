using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestScript : MonoBehaviour
{

    public Quest MyQuest { get; set; }

    public bool markedComplete = false;

   
    public void Select()
    {
        GetComponent<Text>().color = Color.red;
        QuestLog.MyInstance.ShowDescription(MyQuest);
    }
    public void DeSelect()
    {
        GetComponent<Text>().color = Color.white;
    }

    public void IsComplete()
    {
        if (MyQuest.IsComplete && !markedComplete)
        {
            markedComplete = true;
            GetComponent<Text>().text += "(C)";

            MessageFeedManager.MyInstance.WriteMessage(string.Format("{0} (C)", MyQuest.MyTitle));   
        }
        else if (!MyQuest.IsComplete)
        {
            markedComplete = false;
            GetComponent<Text>().text = " [" + MyQuest.MyLevel + "] " + MyQuest.MyTitle;
        }
    }
}
