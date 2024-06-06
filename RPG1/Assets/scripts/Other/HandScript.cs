using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandScript : MonoBehaviour
{
    private static HandScript instance;

    public static HandScript MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HandScript>();
            }

            return instance;
        }
    }
    public IMoveable MyMoveable { get; set; }

    private Image icon;

    [SerializeField]
    private Vector3 offset;


    
    void Start()
    {
        icon = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        icon.transform.position = Input.mousePosition + offset;

        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && MyInstance.MyMoveable != null)
        {
            DeleteItem();
        }
       
    }

    public void TakeMoveable(IMoveable moveable)
    {
        this.MyMoveable = moveable;
        icon.sprite = moveable.MyIcon;
        icon.enabled = true;
    }
    public IMoveable Put()
    {
        IMoveable tmp = MyMoveable;

        MyMoveable = null;

        icon.enabled = false;

        return tmp;

    }

    public void Drop() //item(öğemi) seçtikten sonra bırakmak için
    {
        MyMoveable = null;
        icon.enabled = false;
        InventoryScript.MyInstance.FromSlot = null;
    }

    public void DeleteItem() //öğeyi sil
    {
        
        if (MyMoveable is Item )
        {
            Item item = (Item)MyMoveable;
            if (item.MySlot != null)
            {
                item.MySlot.Clear();
            }
            else if (item.MyCharButton != null)
            {
                item.MyCharButton.DequipArmor();
            }
           
        }

        Drop();

        InventoryScript.MyInstance.FromSlot = null;
    }
}
