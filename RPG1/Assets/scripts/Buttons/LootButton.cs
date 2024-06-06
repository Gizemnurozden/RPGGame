using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LootButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    [SerializeField]
    private Image icon;

    [SerializeField]
    private Text title;

    public Item MyLoot { get; set; }

    private LootWindow lootWindow;

    private void Awake()
    {
        lootWindow = GetComponentInParent<LootWindow>();
    }

    public void OnPointerClick(PointerEventData eventData) //çantaya loottan eklemek için.
    {
        if (InventoryScript.MyInstance.AddItem(MyLoot))
        {
            gameObject.SetActive(false);

            lootWindow.TakeLoot(MyLoot);

            UIManager.MyInstance.HideTooltip(); //toolun içinde öğe kalmayınca saklasın diye.
               
        }
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.MyInstance.ShowTooltip(new Vector2(1, 0), transform.position, MyLoot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.MyInstance.HideTooltip();
    }


    public Image MyIcon { get => icon; }
    public Text MyTitle { get => title; }
}
