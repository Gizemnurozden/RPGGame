using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public delegate void ItemCountChanged(Item item);

public class InventoryScript : MonoBehaviour
{
    public event ItemCountChanged itemCountChangedEvent;

    private static InventoryScript instance;

    public static InventoryScript MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InventoryScript>();
            }

            return instance;
        }
    }

    private SlotScript fromSlot;

    private List<Bag> bags = new List<Bag>();

    [SerializeField]
    private BagButton[] bagButtons;

    [SerializeField]
    private Item[] items;

    public bool CanAddBag
    {
        get { return MyBags.Count < 5; }
    }

    public int MyEmptySlotCount //boş olan slotun sayısı
    {
        get
        {
            int count = 0;
            foreach (Bag bag  in MyBags)
            {
                count += bag.MyBagScript.MyEmptySlotCount;
            }

            return count;
        }
    }

    public int MyTotalSlotCount //total
    {
        get
        {
            int count = 0;

            foreach (Bag bag in MyBags)
            {
                count += bag.MyBagScript.MySlots.Count;
            }
            return count;
        }
    }

    public int MyFullSlotCount //toplam
    {
        get
        {
            return MyTotalSlotCount - MyEmptySlotCount;
        }
    }


    public SlotScript FromSlot
    {
        get
        {
            return fromSlot;
        }
        set
        {
            fromSlot = value;

            if (value != null)
            {
                fromSlot.MyCover.enabled = true;
            }
           
        }
    }

    public List<Bag> MyBags { get => bags;}

    private void Awake() //çantayı etkinleştirme
    {
        Bag bag = (Bag)Instantiate(items[6]);
        bag.Initialize(20);
        bag.Use();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
      
            Bag bag = (Bag)Instantiate(items[6]);
            bag.Initialize(20);
            bag.Use();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Bag bag = (Bag)Instantiate(items[6]);
            bag.Initialize(20);
            AddItem(bag);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            HealthPotion potion = (HealthPotion)Instantiate(items[7]);
            AddItem(potion);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            GoldNugget goldNugget = (GoldNugget)Instantiate(items[8]);
            AddItem(goldNugget);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            AddItem((Armor)Instantiate(items[0]));
            AddItem((Armor)Instantiate(items[1]));
            AddItem((Armor)Instantiate(items[2]));
            AddItem((Armor)Instantiate(items[3]));
            AddItem((Armor)Instantiate(items[4]));
            AddItem((Armor)Instantiate(items[5]));
     

        }

    }

    public void AddBag(Bag bag) //çanta ekleme
    {
        foreach (BagButton bagButton in bagButtons)
        {
            if (bagButton.MyBag == null)
            {
                bagButton.MyBag = bag;
                MyBags.Add(bag);
                bag.MyBagButton = bagButton;
                bag.MyBagScript.transform.SetSiblingIndex(bagButton.MyBagIndex);
                break;
            }
        }
    }

    public void AddBag(Bag bag, BagButton bagButton)
    {
        MyBags.Add(bag);
        bagButton.MyBag = bag;
        bag.MyBagScript.transform.SetSiblingIndex(bagButton.MyBagIndex);

    }

    public void AddBag(Bag bag, int bagIndex) //dataya kaydederken güncelleyebilmek için index.
    {
        bag.SetupScript();
        MyBags.Add(bag);
        bag.MyBagScript.MyBagIndex = bagIndex;
        bag.MyBagButton = bagButtons[bagIndex];
        bagButtons[bagIndex].MyBag = bag;
    }

    public void RemoveBag(Bag bag) //çantanın içinden kaldırmak için koyduğumda ikinci stack açılmasın diye 
    {
        MyBags.Remove(bag);
        Destroy(bag.MyBagScript.gameObject);
    }

    public void SwapBags(Bag oldBag, Bag newBag) //slotlara çantaları eklemek için kullandık. aşağıdaki çantaya tıklayınca ekleyebilme özelliği için
    {
        int newSlotCount = (MyTotalSlotCount - oldBag.MySlotCount) + newBag.MySlotCount;

        if (newSlotCount-MyFullSlotCount >= 0)
        {
            //do Swap

            List<Item> bagItems = oldBag.MyBagScript.GetItems();

            RemoveBag(oldBag);

            newBag.MyBagButton = oldBag.MyBagButton;

            newBag.Use();

            foreach (Item item in bagItems)
            {
                if (item != newBag) //kopya olmadığından emin oluyoruz. 
                {
                    AddItem(item);
                }
            }



            AddItem(oldBag);

            HandScript.MyInstance.Drop();

            MyInstance.fromSlot = null;
        }
    }

    public bool AddItem(Item item) //öğe ekleme çantaya
    {
        if (item.MyStackSize > 0)
        {
            if (PlaceInStack(item))
            {
                return true;
            }
        }
        return PlaceInEmpty(item);
    }

    private bool PlaceInEmpty(Item item)
    {
        foreach (Bag bag in MyBags)
        {
            if (bag.MyBagScript.AddItem(item))
            {
                OnItemCountChanged(item);
                return true;
            }
        }
        return false;
    }

    private bool PlaceInStack(Item item) //öğeleri koymak için 
    {
        foreach (Bag bag in MyBags)
        {
            foreach (SlotScript slots in bag.MyBagScript.MySlots)
            {
                if (slots.StackItem(item))
                {
                    OnItemCountChanged(item);
                    return true;
                }
            }
        }
        return false;
    }

    public void PlaceInSpecific(Item item, int slotIndex, int bagIndex)
    {
        bags[bagIndex].MyBagScript.MySlots[slotIndex].AddItem(item);
    }

    public void OpenClose() //çantayı açıp kapama
    {
        bool closedBag = MyBags.Find(x => !x.MyBagScript.IsOpen);

        //ıf closed bag == true, then open all closed bags
        //if closed bag == false then close all open bags

        foreach (Bag bag in MyBags)
        {
            if (bag.MyBagScript.IsOpen != closedBag)
            {
                bag.MyBagScript.OpenClose();
            }
        }
    }

    public List<SlotScript> GetAllItems()
    {
        List<SlotScript> slots = new List<SlotScript>();

        foreach (Bag bag in MyBags)
        {
            foreach (SlotScript slot in bag.MyBagScript.MySlots)
            {
                if (!slot.IsEmpty)
                {
                    slots.Add(slot);
                }
            }
        }
        return slots;
    }

    public Stack<IUseable> GetUseables(IUseable type) //iksirim action barda kullanılabilsin diye
    {
        Stack<IUseable> useables = new Stack<IUseable>();

        foreach (Bag bag in MyBags)
        {
            foreach (SlotScript slot in bag.MyBagScript.MySlots)
            {
                if (!slot.IsEmpty && slot.MyItem.GetType() == type.GetType())
                {
                    foreach (Item item in slot.MyItems)
                    {
                        useables.Push(item as IUseable);
                    }
                }
            }
        }

        return useables;
    }

    public IUseable GetUseable(string type)
    {
        Stack<IUseable> useables = new Stack<IUseable>();

        foreach (Bag bag in MyBags)
        {
            foreach (SlotScript slot in bag.MyBagScript.MySlots)
            {
                if (!slot.IsEmpty && slot.MyItem.MyTitle == type)
                {
                    return (slot.MyItem as IUseable);
                }
            }
        }

        return null;
    }

    public int GetItemCount (string type) //soru panelinde kullandık Quest'te.
    {
        int itemCount = 0;

        foreach (Bag bag in MyBags)
        {
            foreach (SlotScript slot in bag.MyBagScript.MySlots)
            {
                if (!slot.IsEmpty && slot.MyItem.MyTitle == type)
                {
                    itemCount += slot.MyItems.Count;
                }
            }
        }
        return itemCount;
    }

    public Stack<Item> GetItems(string type,int count)
    {
        Stack<Item> items = new Stack<Item>();

        foreach (Bag bag in MyBags)
        {
            foreach (SlotScript slot in bag.MyBagScript.MySlots)
            {
               
                if (!slot.IsEmpty && slot.MyItem.MyTitle == type)
                {
                   
                    foreach (Item item in slot.MyItems)
                    {
                        items.Push(item);

                        if (items.Count == count)
                        {
                           
                            return items;
                        }
                    }
                }
            }
        }
        return items;
    }

    public void RemoveItem(Item item)
    {
        foreach (Bag bag in MyBags)
        {
            foreach (SlotScript slot in bag.MyBagScript.MySlots)
            {

                if (!slot.IsEmpty && slot.MyItem.MyTitle == item.MyTitle)
                {
                    slot.RemoveItem(item);
                    break;
                }
            }
        }
       
    }

    public void OnItemCountChanged(Item item)
    {
        if (itemCountChangedEvent != null)
        {
            itemCountChangedEvent.Invoke(item);
        }
    }

}
