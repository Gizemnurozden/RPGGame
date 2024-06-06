
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour, IPointerClickHandler, IClickable, IPointerEnterHandler,IPointerExitHandler
{
    

    public IUseable MyUseable { get; set; }

    [SerializeField]
    private Text stackSize;

    private Stack<IUseable> useables = new Stack<IUseable>();

    private int count;

    public Button MyButton { get; private set; }

    public Image MyIcon { get => icon; set => icon = value; }

    public int MyCount
    {
        get
        {
            return count;
        }
    }

    public Text MyStackText
    {
        get { return stackSize; }
    }

    public Stack<IUseable> MyUseables
    {
        get
        {
            return useables;
        }
        set
        {
            if (value.Count > 0)
            {
                MyUseable = value.Peek();
            }

            else
            {
                MyUseable = null;
            }
            useables = value;
        }
    }

    [SerializeField]
    private Image icon;

    void Start()
    {
        MyButton = GetComponent<Button>();
        MyButton.onClick.AddListener(OnClick);
        InventoryScript.MyInstance.itemCountChangedEvent += new ItemCountChanged(UpdateItemCount);
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (HandScript.MyInstance.MyMoveable != null && HandScript.MyInstance.MyMoveable is IUseable)
            {
                SetUseable(HandScript.MyInstance.MyMoveable as IUseable);
            }
        }
    }

    public void OnClick()
    {
        if (HandScript.MyInstance.MyMoveable == null)
        {
            if (MyUseable != null)
            {
                MyUseable.Use();
            }
            else if (MyUseables != null && MyUseables.Count > 0) //iksirim action kısmında kullanılabilsin diye
            {
                MyUseables.Peek().Use();
            }
        }

       
    }

    public void SetUseable(IUseable useable)
    {
        if (useable is Item)
        {
            MyUseables = InventoryScript.MyInstance.GetUseables(useable); ////iksirim action kısmında kullanılabilsin diye
            if (InventoryScript.MyInstance.FromSlot != null)
            {
                InventoryScript.MyInstance.FromSlot.MyCover.enabled = false;
                InventoryScript.MyInstance.FromSlot.MyIcon.enabled = true;
                InventoryScript.MyInstance.FromSlot = null;
            }
         
            
        }
        else
        {
            MyUseables.Clear(); //action buttonlarında sağlığın üstüne yeni bi büyü koyunca sağlığın kullanılırdlığını kaldırığ yeni koyduğumu etkinleştirmek için yağtık.
           this.MyUseable = useable;
        }

        count = MyUseables.Count;
        UpdateVisual(useable as IMoveable); //görseli güncelle
        UIManager.MyInstance.RefreshTooltip(MyUseable as IDescribable); //action buttonda ilaç olamsına rağmen ateş etmemsi için 

    }

    public void UpdateVisual(IMoveable moveable) //görseli güncelle, ikonu koy.
    {
        if (HandScript.MyInstance.MyMoveable != null)
        {
            HandScript.MyInstance.Drop();
        }


        MyIcon.sprite = moveable.MyIcon;
        MyIcon.enabled = true;

        if (count > 1)
        {
            UIManager.MyInstance.UpdateStackSize(this);
        }
        else if (MyUseable is Spell)
        {
            UIManager.MyInstance.ClearStackCount(this);
        }
    }

    public void UpdateItemCount(Item item)
    {
        if (item is IUseable && MyUseables.Count > 0)
        {
            if (MyUseables.Peek().GetType() == item.GetType())
            {
                MyUseables = InventoryScript.MyInstance.GetUseables(item as IUseable);

                count = MyUseables.Count;

                UIManager.MyInstance.UpdateStackSize(this);
            }
        }

    }

    public void OnPointerExit(PointerEventData eventData) //üzerine gelince bilgi vermesi için
    {
        IDescribable tmp = null;

        if (MyUseable != null && MyUseable is IDescribable)
        {
            tmp = (IDescribable)MyUseable;
            //UIManager.MyInstance.ShowTooltip(transform.position);
        }
        else if (MyUseables.Count > 0)
        {
           // UIManager.MyInstance.ShowTooltip(transform.position);
        }
        if (tmp != null)
        {
            UIManager.MyInstance.ShowTooltip(new Vector2(1,0), transform.position, tmp);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.MyInstance.HideTooltip();
    }
}
