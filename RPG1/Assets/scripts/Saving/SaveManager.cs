using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    [SerializeField]
    private Item[] items;

    private Chest[] chests;

    private CharButton[] equipment;

    [SerializeField]
    private ActionButton[] actionButtons;

    [SerializeField]
    private SavedGame[] saveSlots;

    [SerializeField]
    private GameObject dialogue;

    [SerializeField]
    private Text dialogueText;

    private SavedGame current;


    private string action;

    private void Awake()
    {
        chests = FindObjectsOfType<Chest>();
        equipment = FindObjectsOfType<CharButton>();

        foreach (SavedGame saved in saveSlots)
        {
            //kaydettiğimiz dosyaları burada göstermemiz gerekiyor.
            ShowSavedFiles(saved);
        }

       
    }
    //Debug.Log(Application.persistentDataPath); dosyamın konumuna baktım.

    private void Start()
    {

        if (PlayerPrefs.HasKey("Load"))
        {
            Load(saveSlots[PlayerPrefs.GetInt("Load")]);
            PlayerPrefs.DeleteKey("Load");
        }
        else
        {
            //default

            Player.MyInstance.SetDefaultValues();
        }
    }

    public void ShowDialogue(GameObject clickButton) //
    {
        action = clickButton.name;

        switch (action)
        {
            case "Load":
                dialogueText.text = "Load game?";
                break;
            case "Save":
                dialogueText.text = "Save game?";
                break;
            case "Delete":
                dialogueText.text = "Delete savefile?";
                break;
           
        }
        current = clickButton.GetComponentInParent<SavedGame>();
        dialogue.SetActive(true);
    }

    public void ExecuteAction()
    {
        switch (action)
        {
            case "Load":
                LoadScene(current);
                break;
            case "Save":
                Save(current);
                break;
            case "Delete":
                Delete(current);
                break;

        }

        CloseDialogue();
    }

    private void LoadScene(SavedGame savedGame)
    {
        if (File.Exists(Application.persistentDataPath + "/" + savedGame.gameObject.name + ".dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + savedGame.gameObject.name + ".dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();

            PlayerPrefs.SetInt("Load", savedGame.MyIndex);
            SceneManager.LoadScene(data.MyScene);
        }
    }

    public void CloseDialogue()

    {
        dialogue.SetActive(false);
    }


    private void Delete(SavedGame savedGame) //kaydettiğim savedgame penceresindekileri silmek için.
    {
        File.Delete(Application.persistentDataPath + "/" + savedGame.gameObject.name + ".dat");
        savedGame.HideVisuals();
    }

    private void ShowSavedFiles(SavedGame savedGame) //save penceresinde kaydettiklerimi görebilmek için
    {
        if (File.Exists(Application.persistentDataPath + "/" + savedGame.gameObject.name + ".dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + savedGame.gameObject.name + ".dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            savedGame.ShowInfo(data);
        }
    }

    public  void Save(SavedGame savedGame)
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter(); //veri depolama ,veri iletme için kullanılır

            FileStream file = File.Open(Application.persistentDataPath + "/" + savedGame.gameObject.name+ ".dat",FileMode.Create); //dosya açmak için kullanılır. Açtığım dosyada Application.persistentDataPath ifadesi kalıcı veri dizinini temsil eder.güncellemden etkinlenmez.
            //savetest.dat oluşturulacak dosyanın adı. file open dosya açma, filemode.openorcreate varsa bu isimli dosyasyı açar yoksa yaratır.create direkt yaratır. bu ifadeler oyunun ilerlemesini kaydetmek içni kullanılır.

            SaveData data = new SaveData();

            data.MyScene = SceneManager.GetActiveScene().name;

            SaveEquipment(data); //ekipmanları kaydet.karakter paneli için

            SaveBags(data); //çantayı kaydet.

            SaveInventory(data); //çantanın içnideki itemleri kaydet.

            SavePlayer(data); //playeri kaydet.

            SaveChests(data); // hazineyi kaydet.

            SaveActionButtons(data); //acksiyon butonlarını kaydet.

            SaveQuests(data); //Soru logu için soruları kaydeder.

            SaveQuestGivers(data); //soru veren

            bf.Serialize(file, data); //dosyaya verileri yazdırır.

            file.Close(); //dosyasyı kapatır.

            ShowSavedFiles(savedGame);
        }
        catch (System.Exception )
        {
            Delete(savedGame);
           
            PlayerPrefs.DeleteKey("Load");
        }
    }

    private void SavePlayer(SaveData data) //oyuncuyu kaydetme
    {
        data.MyPlayerData = new PlayerData(Player.MyInstance.MyLevel,
            Player.MyInstance.MyXp.MyCurrentValue,Player.MyInstance.MyXp.MyMaxValue,
            Player.MyInstance.MyHealth.MyCurrentValue,Player.MyInstance.MyHealth.MyMaxValue,
            Player.MyInstance.MyMana.MyCurrentValue,Player.MyInstance.MyMana.MyMaxValue,
            Player.MyInstance.transform.position);
    }

    private void SaveChests(SaveData data) //hazineyi kaydetme
    {
        for (int i = 0; i < chests.Length; i++)
        {
            data.MyChestData.Add(new ChestData(chests[i].name));

            foreach (Item item in chests[i].MyItems)
            {
                if (chests[i].MyItems.Count > 0)
                {
                    data.MyChestData[i].MyItems.Add(new ItemData(item.MyTitle, item.MySlot.MyItems.Count, item.MySlot.MyIndex));

                }
            }
        }
    }

    private void SaveBags(SaveData data) //çantayı kaydetme 
    {
        for (int i = 1; i < InventoryScript.MyInstance.MyBags.Count; i++)
        {
            data.MyInventoryData.MyBags.Add(new BagData(InventoryScript.MyInstance.MyBags[i].MySlotCount, InventoryScript.MyInstance.MyBags[i].MyBagButton.MyBagIndex ));
        }
    }

    private void SaveEquipment(SaveData data) //karakter panelinin içindeki ekipmanalrı kaydet
    {
        foreach (CharButton charButton in equipment)
        {
            if (charButton.MyEquippedArmor != null)
            {
                data.MyEquipmentData.Add(new EquipmentData(charButton.MyEquippedArmor.MyTitle, charButton.name));
            }
        }
    }

    private void SaveActionButtons(SaveData data) // aksiyon butonlaruını kaydet.
    {
        for (int i = 0; i < actionButtons.Length; i++)
        {
            if (actionButtons[i].MyUseable != null)
            {
                ActionButtonData action;

                if (actionButtons[i].MyUseable is Spell)
                {
                     action = new ActionButtonData((actionButtons[i].MyUseable as Spell).MyTitle,false,i);
                }
                else
                {
                     action = new ActionButtonData((actionButtons[i].MyUseable as Item).MyTitle, true, i);


                }

                data.MyActionButtonData.Add(action);
            }
        }
    }

    private void SaveInventory(SaveData data) //çantanın içindeki kaydet.
    {
        List<SlotScript> slots = InventoryScript.MyInstance.GetAllItems();

        foreach (SlotScript slot in slots)
        {
            data.MyInventoryData.MyItems.Add(new ItemData(slot.MyItem.MyTitle, slot.MyItems.Count, slot.MyIndex, slot.MyBag.MyBagIndex));

        }
    }


    private void SaveQuests(SaveData data) //questlog için
    {
        foreach (Quest quest in QuestLog.MyInstance.MyQuests)
        {
            data.MyQuestData.Add(new QuestData(quest.MyTitle, quest.MyDescription, quest.MyCollectObjectives, quest.MyKillObjectives,quest.MyQuestGiver.MyQuestGiverID));
        }
    }

    private void SaveQuestGivers(SaveData data) //soru veren için
    {
        QuestGiver[] questGivers = FindObjectsOfType<QuestGiver>();

        foreach (QuestGiver questGiver in questGivers)
        {
            data.MyQuestGiverData.Add(new QuestGiverData(questGiver.MyQuestGiverID, questGiver.MyCompltedQuests));
        }
    }

    private void Load(SavedGame savedGame) //son güncel durumu getirir.
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Open(Application.persistentDataPath + "/" + savedGame.gameObject.name + ".dat", FileMode.Open);

            SaveData data = (SaveData)bf.Deserialize(file); //dosyaya yazılan verileri okunabilir hale getirdik

            file.Close();

            LoadPlayer(data); //playeri getir.

            LoadChests(data); //hazineyi getir.

            LoadBags(data); //çantayı getir

            LoadInventory(data);

            LoadEquipment(data); //ekipmanları getir.

            LoadActionButtons(data); //aksiyon butonu getir.

            LoadQuests(data); //soruları loga getir.

            LoadQuestGiver(data); //soruları questgivera getir.
                            
                           
        }
        catch (System.Exception)
        {
            //this is for handling errors
            Delete(savedGame);
            PlayerPrefs.DeleteKey("Load");
            SceneManager.LoadScene(0);
          
        }
    }

    private void LoadPlayer(SaveData data) //oyunucunun günceller son durumu getirir
    {
        Player.MyInstance.MyLevel = data.MyPlayerData.MyLevel; //leveli kaydetmek için
        Player.MyInstance.UpdateLevel(); 
        Player.MyInstance.MyHealth.Initialized(data.MyPlayerData.MyHealth, data.MyPlayerData.MyMaxHealth); //sağlığı kaydetmek için
        Player.MyInstance.MyMana.Initialized(data.MyPlayerData.MyMana, data.MyPlayerData.MyMaxMana); //mana kaydetmek için
        Player.MyInstance.MyXp.Initialized(data.MyPlayerData.MyXp, data.MyPlayerData.MyMaxXP); // xp puanımı kaydetmek içn
        Player.MyInstance.transform.position = new Vector2(data.MyPlayerData.MyX, data.MyPlayerData.MyY); //pozisyonumu kadyetmek içni

    }

    private void LoadChests(SaveData data) // hazinenin son durum güncelliğini getirir
    {
        foreach (ChestData chest in data.MyChestData)
        {
            Chest c = Array.Find(chests, x => x.name == chest.MyName);

            foreach (ItemData itemData in chest.MyItems)
            {
                Item item = Instantiate( Array.Find(items, x => x.MyTitle == itemData.MyTitle));
                item.MySlot = c.MyBag.MySlots.Find(x => x.MyIndex == itemData.MySlotIndex);
                c.MyItems.Add(item);
            }
        }
    }

    private void LoadBags(SaveData data) //çantayı kaydet.
    {
        foreach (BagData bagData in data.MyInventoryData.MyBags)
        {
            Bag newBag = (Bag)Instantiate(items[0]);

            newBag.Initialize(bagData.MySlotCount);

            InventoryScript.MyInstance.AddBag(newBag, bagData.MyBagIndex);
        }
    }

    private void LoadEquipment (SaveData data) //karakter paneli için
    {
        foreach (EquipmentData equipmentData in data.MyEquipmentData)
        {
            CharButton cb = Array.Find(equipment, x => x.name == equipmentData.MyType);

            cb.EquipArmor(Array.Find(items, x => x.MyTitle == equipmentData.MyTitle) as Armor);

        }
    }

    private void LoadActionButtons(SaveData data) //aksiyon butonlarım
    {
        foreach (ActionButtonData buttonData in data.MyActionButtonData)
        {
            if (buttonData.IsItem)
            {
                actionButtons[buttonData.MyIndex].SetUseable(InventoryScript.MyInstance.GetUseable(buttonData.MyAction));
            }
            else
            {
                actionButtons[buttonData.MyIndex].SetUseable(SpellBook.MyInstance.GetSpell(buttonData.MyAction));
            }
        }

    }

    private void LoadInventory(SaveData data) //çantanın içindekileri kaydetmek için
    {
        foreach (ItemData itemData in data.MyInventoryData.MyItems)
        {
            Item item =Instantiate( Array.Find(items, x => x.MyTitle == itemData.MyTitle));

            for (int i = 0; i < itemData.MyStackCount; i++)
            {
                InventoryScript.MyInstance.PlaceInSpecific(item, itemData.MySlotIndex, itemData.MyBagIndex);
            }
        }
        
        
    }

    private void LoadQuests(SaveData data) //questlog için
    {
        QuestGiver[] questGivers = FindObjectsOfType<QuestGiver>();

        foreach (QuestData questData in data.MyQuestData)
        {
            QuestGiver qg = Array.Find(questGivers, x => x.MyQuestGiverID == questData.MyQuestGiverID);
            Quest q = Array.Find(qg.MyQuests, x => x.MyTitle == questData.MyTitle);
            q.MyQuestGiver = qg;
            q.MyKillObjectives = questData.MyKillObjectives; //görevim tamamlanmadan kaudetsemde yaptığım kadarı kaydolsun diye.
            QuestLog.MyInstance.AcceptQuest(q);
        }
        

        
    }

    private void LoadQuestGiver(SaveData data) //questgiver için
    {
        QuestGiver[] questGivers = FindObjectsOfType<QuestGiver>();

        foreach (QuestGiverData questGiverData in data.MyQuestGiverData)
        {
            QuestGiver questGiver = Array.Find(questGivers, x => x.MyQuestGiverID == questGiverData.MyQuestGiverID);
            questGiver.MyCompltedQuests = questGiverData.MyCompletedQuests;
            questGiver.UpdateQuestStatus();


        }
    }
}
