
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VendorWindow : Window
{
    

    [SerializeField]
    private VendorButton[] vendorButtons;

    [SerializeField]
    private Text pageNumber;

    private List<List<VendorItem>> pages = new List<List<VendorItem>>();

    private Vendor vendor;

 
    private int pageIndex;

    public void CreatePages(VendorItem[] items) //sayfayı oluşturarak vendorbuttonda tanımladığım addıtemi döngüye alıyorum
    {
        pages.Clear();

        List<VendorItem> page = new List<VendorItem>();

        for (int i = 0; i < items.Length; i++)
        {
            page.Add(items[i]);

            if (page.Count == 10 || i == items.Length -1)
            {
                pages.Add(page);
                page = new List<VendorItem>();
            }
        }
        AddItems();
    }


    public void AddItems() //birden fazla item eklersem sayfaları oluşturmak için yazdık.
    {

        pageNumber.text = pageIndex + 1 + "/" + pages.Count; //sayfanın değişme numarasını değiştirmek için

        if (pages.Count >0)
        {
            for (int i = 0; i < pages[pageIndex].Count; i++)
            {
                if (pages[pageIndex][i] != null)
                {
                    vendorButtons[i].AddItem(pages[pageIndex][i]);
                }
            }
        }
    }
  

    public void NextPage()
    {
        if (pageIndex < pages.Count -1)
        {
            ClearButtons();
            pageIndex++;
            AddItems();
        }
    }
    public void PreviousPage()
    {
        ClearButtons();
        pageIndex--;
        AddItems();
    }

    public void ClearButtons()
    {
        foreach (VendorButton btn in vendorButtons)
        {
            btn.gameObject.SetActive(false);
        }
    }

    public override void Open(NPC npc)
    {
        CreatePages((npc as Vendor).MyItems);
        base.Open(npc);
    }
}
