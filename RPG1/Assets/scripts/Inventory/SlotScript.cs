using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotScript : MonoBehaviour, IPointerClickHandler, IClickable, IPointerEnterHandler, IPointerExitHandler
{
    private ObservableStack<Item> items = new ObservableStack<Item>();

    [SerializeField]
    private Image icon;

    [SerializeField]
    private Text stackSize;

    [SerializeField]
    private Image cover;

    public BagScript MyBag { get; set; } //bu slotta olan item içni referans

    public int MyIndex { get; set; }

    public bool IsEmpty
    {
        get
        {
            return MyItems.Count == 0;
        }
    }

    public bool IsFull
    {
        get
        {
            if (IsEmpty || MyCount < MyItem.MyStackSize)
            {
                return false;
            }
            return true;
        }

    }

    public Item MyItem
    {
        get
        {
            if (!IsEmpty)
            {
                return MyItems.Peek();
            }
            return null;
        }
    }

    public Image MyIcon
    {
        get
        {
            return icon;
        }
        set
        {
            icon = value;
        }

    }

    public int MyCount
    {
        get { return MyItems.Count; }
    }

    public Text MyStackText => stackSize;

    public ObservableStack<Item> MyItems { get => items;  }
    public Image MyCover { get => cover; }

    private void Awake() //slotlara numaralarla öğeyi ekleme ekledikçe sayı artıyor 
    {
        MyItems.OnPop += new UpdateStackEvent(UpdateSlot);
        MyItems.OnPush += new UpdateStackEvent(UpdateSlot);
        MyItems.OnClear += new UpdateStackEvent(UpdateSlot);
    }

    public bool AddItem(Item item)
    {
        MyItems.Push(item);
        icon.sprite = item.MyIcon;
        MyCover.enabled = false;
        icon.color = Color.white;
        item.MySlot = this;

        return true;
    }

    public bool AddItems(ObservableStack<Item> newItems) //öğeyi başka slota koymak için
    {
        if (IsEmpty || newItems.Peek().GetType() == MyItem.GetType())
        {
            int count = newItems.Count;

            for (int i = 0; i < count; i++)
            {
                if (IsFull)
                {
                    return false;
                }

                AddItem(newItems.Pop());
            }
            return true;
        }

        return false;
    }

    public void RemoveItem(Item item)
    {
        if (!IsEmpty)
        {
         
            InventoryScript.MyInstance.OnItemCountChanged(MyItems.Pop());
         
        }
    }

    public void Clear() //itemı temizlemek için handScriptte çağırdım.
    {
        if (MyItems.Count > 0)
        {
            int initCount = MyItems.Count; //silme işleimini itema göre yapar mesela üçlü attın 1 tane değil 3lü siler
            MyCover.enabled = false;
            if (initCount >0)
            {
                for (int i = 0; i < initCount; i++)
                {
                    InventoryScript.MyInstance.OnItemCountChanged(MyItems.Pop());
                }
            }

           
           
        }
    }

    public void OnPointerClick(PointerEventData eventData) //sağa click bastığımda useitem çağırılır ve öğe kaldırırlır slottan.
    {
        if (eventData.button == PointerEventData.InputButton.Left) //öğelerimi sola tıklayarak seçiyorum.
        {
            if (InventoryScript.MyInstance.FromSlot == null && !IsEmpty) //hareket ettirmek istemezsem
            {

                if (HandScript.MyInstance.MyMoveable != null )
                {
                    if (HandScript.MyInstance.MyMoveable is Bag)
                    {
                        if (MyItem is Bag) //tıkladığım öğem çantamı diye kontrol ediyorum.
                        {
                            InventoryScript.MyInstance.SwapBags(HandScript.MyInstance.MyMoveable as Bag, MyItem as Bag);
                        }
                    }
                    else if (HandScript.MyInstance.MyMoveable is Armor)
                    {
                        if (MyItem is Armor && (MyItem as Armor).MyArmorType == (HandScript.MyInstance.MyMoveable as Armor).MyArmorType )
                        {
                            (MyItem as Armor).Equip();
                            
                            HandScript.MyInstance.Drop();
                        }
                    }

                    
                }
                else
                {
                    HandScript.MyInstance.TakeMoveable(MyItem as IMoveable); //öğelerim hareket edebilir hale geiyor.
                    InventoryScript.MyInstance.FromSlot = this; //referans veriyorum.
                }
               
            }
            else if (InventoryScript.MyInstance.FromSlot == null && IsEmpty ) //çantanın tıklanınca kaldırılamsı (stackın)
            {
                if (HandScript.MyInstance.MyMoveable is Bag)
                {
                    Bag bag = (Bag)HandScript.MyInstance.MyMoveable;

                    if (bag.MyBagScript != MyBag && InventoryScript.MyInstance.MyEmptySlotCount - bag.MySlotCount > 0)
                    {
                        AddItem(bag);
                        bag.MyBagButton.RemoveBag(); //çantanın kaldırılması
                        HandScript.MyInstance.Drop();
                      
                    }
                    else if (HandScript.MyInstance.MyMoveable is Armor) //karakter panelinden slota geri koyma işlemi
                    {
                        Armor armor = (Armor)HandScript.MyInstance.MyMoveable;
                        CharacterPanel.MyInstance.MySelectedButton.DequipArmor();
                        AddItem(armor); //vendor panelinden slota koyarken 
                        HandScript.MyInstance.Drop();

                    }
                }
               
               
            }
            else if (InventoryScript.MyInstance.FromSlot != null) // hareket ettirmek istersem
            {
                if (PutItemBack() || MergeItems(InventoryScript.MyInstance.FromSlot) || SwapItems(InventoryScript.MyInstance.FromSlot) || AddItems(InventoryScript.MyInstance.FromSlot.MyItems)) //öğeyi başka yere koymsk için
                {
                    HandScript.MyInstance.Drop();
                    InventoryScript.MyInstance.FromSlot = null;
                }
            }

            
        }
        
        if (eventData.button == PointerEventData.InputButton.Right && HandScript.MyInstance.MyMoveable == null)
        {
            
            UseItem();
        }
    }
    public void UseItem() //öğelerimi ekleyip kaldırbilmek için öğenin kontrolü
    {
        //if koşulunu kaldırdım. geri ekledim.
        if (MyItem is IUseable)
        {
            (MyItem as IUseable).Use();
        }
        else if (MyItem is Armor)
        {
            (MyItem as Armor).Equip();
        }
     
    }

    public bool StackItem(Item item) //eklenen öğelerin kontrolü,aynı isimde mi yığın stack boş mu vs.. 
    {
        if (!IsEmpty && item.name == MyItem.name && MyItems.Count < MyItem.MyStackSize)
        {
            MyItems.Push(item);
            item.MySlot = this;
            return true;
        }
        return false;
    }

    public bool PutItemBack() //öğeyi geri bırak
    {
        MyCover.enabled = false;
        if (InventoryScript.MyInstance.FromSlot == this)
        {
            InventoryScript.MyInstance.FromSlot.MyIcon.enabled = true;
            return true;
        }
        return false;
    }

    private bool SwapItems(SlotScript from)
    {
        from.MyCover.enabled = false;
        if (IsEmpty)
        {
            return false;
        }
        if (from.MyItem.GetType() != MyItem.GetType() || from.MyCount + MyCount > MyItem.MyStackSize)
        {
            //tüm itemleri kopyala ihtiyacımız olan swap from A
            ObservableStack<Item> tmpFrom = new ObservableStack<Item>(from.MyItems);

            //slotı temizle
            from.MyItems.Clear();

            //tüm öğeleri b den al A ya kopyala
            from.AddItems(MyItems);

            //b yi temizle
            MyItems.Clear();

            //itemleri acopyden b ye taşı
            AddItems(tmpFrom);

            return true;
        }
        return false;
    }

    private bool MergeItems(SlotScript from) //öğeleri üst üste koyunca birleştirmek için
    {
        if (IsEmpty)
        {
            return false;
        }
        if (from.MyItem.GetType() == MyItem.GetType() && !IsFull)
        {
            //kaç tane boş slotum var stackim de ona bakar
            int free = MyItem.MyStackSize - MyCount;

            for (int i = 0; i < free; i++)
            {
                AddItem(from.MyItems.Pop());
            }
            return true;
        }
        return false;
    }

    private void UpdateSlot()
    {
        UIManager.MyInstance.UpdateStackSize(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //tooltipi açmak için
        if (!IsEmpty)
        {
            UIManager.MyInstance.ShowTooltip(new Vector2(1, 0), transform.position, MyItem);

        }
      
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Tooltipi saklamak için
        UIManager.MyInstance.HideTooltip();
    }
}
