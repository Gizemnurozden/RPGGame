
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootWindow : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private static LootWindow instance;

    public static LootWindow MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<LootWindow>();
            }
            return instance;
        }
    }



    [SerializeField]
    private LootButton[] lootButtons;

    private List<Drops> droppedLoot = new List<Drops>();

    private List<List<Drops>> pages = new List<List<Drops>>();

    [SerializeField]
    private Item[] items;

    private int pageIndex = 0;

    [SerializeField]
    private Text pageNumber;

    [SerializeField]
    private GameObject nextBtn, previousBtn;

    private Enemy currentEnemy;

    public IInteractable MyInteractable { get; set; }

    public bool IsOpen
    {
        get { return canvasGroup.alpha > 0; }
    }

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetCurrentEnemy(Enemy enemy)
    {
        currentEnemy = enemy;
    }
   

    public void CreatePages(List<Drops> items) //canavar öldüğünde sayfaların çıkması ve kaç sayfa çıkacağını ayarlaması için oluşturduğumuz fonksiyon
    {
        if (!IsOpen)
        {
            List<Drops> page = new List<Drops>();

            droppedLoot = items;

            for (int i = 0; i < items.Count; i++)
            {
                page.Add(items[i]);

                if (page.Count == 4 || i == items.Count - 1)
                {
                    pages.Add(page);
                    page = new List<Drops>();
                }
            }
            AddLoot();

            Open();
        }
        
    }

    private void AddLoot()
    {
        if (pages.Count > 0)
        {

            //Sayfa numaraları
            pageNumber.text = pageIndex + 1 + "/" + pages.Count; //loot penceresinin sayfa textini counta göre ayarlama


            //İleri ve geri buton
            previousBtn.SetActive(pageIndex > 0); //indexin 0 dan büyükse geri butonu aktifleştirir

            nextBtn.SetActive(pages.Count > 1 && pageIndex < pages.Count - 1); //son sayfada olup olmadığını anlar ve ileri butonu etkinleştirir.

            for (int i = 0; i < pages[pageIndex].Count; i++)
            {

                if (pages[pageIndex][i] != null)
                {
                    //Set the loot buttons icon
                    lootButtons[i].MyIcon.sprite = pages[pageIndex][i].MyItem.MyIcon;

                    lootButtons[i].MyLoot = pages[pageIndex][i].MyItem;

                    //Make sure the loot buttons is visible
                    lootButtons[i].gameObject.SetActive(true);

                    string title = string.Format("<color={0}>{1}</color>", QualityColor.MyColors[pages[pageIndex][i].MyItem.MyQuality], pages[pageIndex][i].MyItem.MyTitle);

                    //Set the title
                    lootButtons[i].MyTitle.text = title;
                }
               
            }
        }
      

             
    }

    public void ClearButtons() //eklenen öğeleri tekrar tekrar eklemesin diye oluşturduğumuz fonksiyon
    {
        foreach (LootButton  btn in lootButtons)
        {
            btn.gameObject.SetActive(false);
        }

    }

    public void NextPage()
    {
        //birden fazla sayfamızın olup olmadığını kontrol ediyoruz.
        if (pageIndex < pages.Count -1)
        {
            pageIndex++;
            ClearButtons();
            AddLoot();
        }
    }

    public void PreviousPage()
    {
        //geri gidilecek daha fazla sayfa olup olmadığnı kontrol ediyoruz.
        if (pageIndex >0)
        {
            pageIndex--;
            ClearButtons();
            AddLoot();
        }

    }

    public void TakeLoot(Item loot) //sayfalardan lootları kaldırır. Loot button sayfasında çağıracağız.
    {


        Drops drop = pages[pageIndex].Find(x => x.MyItem == loot);

        pages[pageIndex].Remove(drop);

        drop.Remove();

        if (pages[pageIndex].Count == 0)
        {
            //boş sayfaları kaldırır.
            pages.Remove(pages[pageIndex]);

            if (pageIndex == pages.Count && pageIndex > 0)
            {
                pageIndex--;
            }
            AddLoot();
        }
    }

    public void Close()
    {
        pageIndex = 0;
        pages.Clear();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        ClearButtons();

        if (MyInteractable != null)
        {
            MyInteractable.StopInteract();
        }
        MyInteractable = null;

        if (currentEnemy != null)
        {
            currentEnemy.OnLootWindowClosed();
            currentEnemy = null;
        }

        
    }

 
    public void Open()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;

    }
}
