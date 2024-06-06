using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageFeedManager : MonoBehaviour //ekrana görevleri yaparken mesaj yazdırma
{
    public static MessageFeedManager instance;


    [SerializeField]
    private GameObject messagePrefab;


    public static MessageFeedManager MyInstance
    {
        get
        {
            instance = FindObjectOfType<MessageFeedManager>();
            return instance;
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WriteMessage(string message) //ekrana yazı yazma 
    {


       GameObject go = Instantiate(messagePrefab, transform);

        go.GetComponent<Text>().text = message;

        go.transform.SetAsFirstSibling(); //mesaj en yenisi en üsttebolsun

        Destroy(go, 2); //iki saniye sonra sil ekrandan
    }

    public void WriteMessage(string message,Color color) //ekrana yazı yazma 
    {


        GameObject go = Instantiate(messagePrefab, transform);
        Text t = go.GetComponent<Text>();


        t.text = message;
        t.color = color;

        go.transform.SetAsFirstSibling(); //mesaj en yenisi en üsttebolsun

        Destroy(go, 2); //iki saniye sonra sil ekrandan
    }
}
