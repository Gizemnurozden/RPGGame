using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : ScriptableObject, IMoveable,IDescribable
{
    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private int stackSize;

    private SlotScript slot;

    private CharButton charButton;

    [SerializeField]
    private string title; //tooltipte başlık oluşsun diye prefabten başlık girerek oluşturduk.

    [SerializeField]
    private Quality quality;

    [SerializeField]
    private int price;

    public Sprite MyIcon { get => icon; }

    public int MyStackSize
    {
        get
        {
            return stackSize;
        }
    }

    public SlotScript MySlot { get => slot; set => slot = value; }
    public Quality MyQuality { get => quality;}
    public string MyTitle { get => title;  }

    public CharButton MyCharButton
    {
        get
        {
            return charButton;
        }
        set
        {
            MySlot = null;
            charButton = value;
        }
    }

    public int MyPrice { get => price;  }

    public virtual string GetDescription()
    {

        return string.Format("<color={0}>{1}</color>", QualityColor.MyColors[MyQuality], MyTitle); //başlığı döndürür.prefabte yazdığın 
    }

    public void Remove()
    {
        if (MySlot != null)
        {
            MySlot.RemoveItem(this);
           
        }
    }
}
