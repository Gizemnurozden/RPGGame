using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Debuff;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{

    private static UIManager instance;

    public static UIManager MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
            }

            return instance;
        }
    }


    [SerializeField]
    private ActionButton[] actionButtons;

    

    [SerializeField]
    private GameObject targetFrame;

    private Stat healthStat;

    [SerializeField]
    private Text levelText;

    [SerializeField]
    private UnityEngine.UI.Image portraitFrame;


    [SerializeField]
    private CanvasGroup[] menus;

 

    private GameObject[] keybindButtons;

   

    [SerializeField]
    private GameObject tooltip;

    private Text tooltipText;

    [SerializeField]
    private CharacterPanel charPanel;

    [SerializeField]
    private RectTransform tooltipRect;

    [SerializeField]
    private TargetDebuff targetDebuffPrefab;

    [SerializeField]
    private Transform targetDebuffsTranform;

    private List<TargetDebuff> targetDebuffs = new List<TargetDebuff>(); 


    private void Awake()
    {
        keybindButtons = GameObject.FindGameObjectsWithTag("KeyBind");
        tooltipText = tooltip.GetComponentInChildren<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
        healthStat = targetFrame.GetComponentInChildren<Stat>(); //referansı hedefin


    }
   
        // Update is called once per frame
    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenClose(menus[0]);
        }

        if (Input.GetKeyDown(KeyCode.P)) //Spell book
        {
            OpenClose(menus[1]);
        }
        if (Input.GetKeyDown(KeyCode.B)) //çantaları açıp kapama
        {
            InventoryScript.MyInstance.OpenClose();
        }
        if (Input.GetKeyDown(KeyCode.C)) //karakter paneli açıp kapama
        {
            OpenClose(menus[2]);
        }
        if (Input.GetKeyDown(KeyCode.L)) //soru logunu açıp kapama
        {
            OpenClose(menus[3]);
        }
        if (Input.GetKeyDown(KeyCode.Z)) 
        {
            OpenClose(menus[4]);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            OpenClose(menus[5]);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            OpenClose(menus[6]);
        }
        /*
       if (Input.GetKeyDown(KeyCode.Escape)) //escape basınca keybind aktif etme
       {
           OpenClose(keybindMenu);
       }
       if (Input.GetKeyDown(KeyCode.P)) //spell book açıp kapama
       {
           OpenClose(spellBook);
       }
       if (Input.GetKeyDown(KeyCode.B)) //çantaları açıp kapama
       {
           InventoryScript.MyInstance.OpenClose();
       }
       if (Input.GetKeyDown(KeyCode.C)) //çantaları açıp kapama
       {
           charPanel.OpenClose();
       }
       if (Input.GetKeyDown(KeyCode.Z))
       {
           OpenClose(saveMenu);
       }
       if (Input.GetKeyDown(KeyCode.L)) //soru logunu açıp kapama
       {
           QuestLog.MyInstance.OpenClose();
       }
       */

    }





    //hedefin frame özellikleri
    public void ShowTargetFrame(Enemy target) //hedefin framini etkinleştirme
    {
        targetFrame.SetActive(true);

        healthStat.Initialized(target.MyHealth.MyCurrentValue, target.MyHealth.MyMaxValue);

        portraitFrame.sprite = target.MyPortrait;

        levelText.text = target.MyLevel.ToString();

        target.healthChanged += new HealthChanged(UpdateTargetFrame); //bu kod hedefimin sağlık durumunun değiştirği kısmın dinliyor

        target.characterRemoved += new CharacterRemoved(HideTargetFrame); //frami hedefle aynı anda gizle.

        if (target.MyLevel >= Player.MyInstance.MyLevel +5)
        {
            levelText.color = Color.red;
        }
        else if (target.MyLevel == Player.MyInstance.MyLevel + 3 || target.MyLevel == Player.MyInstance.MyLevel + 4)
        {
            levelText.color = new Color32(255, 124, 0, 255);
        }
        else if (target.MyLevel >= Player.MyInstance.MyLevel -2 && target.MyLevel <= Player.MyInstance.MyLevel +2)
        {
            levelText.color = Color.yellow;
        }
        else if (target.MyLevel <= Player.MyInstance.MyLevel - 3 && target.MyLevel > XPManager.CalculateGrayLevel())
        {
            levelText.color = Color.green;
        }
        else
        {
            levelText.color = Color.grey;
        }

    }

    public void HideTargetFrame() //framei sakla
    {
        targetFrame.SetActive(false);
    }

    public void UpdateTargetFrame(float health) //bu fonksiyonun npcdeki healthchangei dinlemesini sağlamalıyım
    {
        healthStat.MyCurrentValue = health;
    }

   public void AddDebuffToTargetFrame(Debuff debuff)
    {
        if (targetFrame.activeSelf)
        {
            TargetDebuff td = Instantiate(targetDebuffPrefab, targetDebuffsTranform);
            td.Initialize(debuff);
            targetDebuffs.Add(td);
        }
    }
    public void RemoveDebuff(Debuff debuff)
    {
        if (targetFrame.activeSelf)
        {
            TargetDebuff td = targetDebuffs.Find(x => x.Debuff.Name == debuff.Name);

            targetDebuffs.Remove(td);
            Destroy(td.gameObject);
        }
    }

    public void UpdateKeyText (string key, KeyCode code)
    {
        Text tmp = Array.Find(keybindButtons, x => x.name == key).GetComponentInChildren<Text>();
        tmp.text = code.ToString();

    }

    public void ClickActionButton(string buttonName)
    {
        Array.Find(actionButtons, x => x.gameObject.name == buttonName).MyButton.onClick.Invoke();
    }

    

    public void OpenClose(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = canvasGroup.alpha > 0 ? 0 : 1;
        canvasGroup.blocksRaycasts = canvasGroup.blocksRaycasts == true ? false : true;
    }

    public void OpenSingle(CanvasGroup canvasGroup)
    {
       

        canvasGroup.alpha = canvasGroup.alpha > 0 ? 0 : 1;
        canvasGroup.blocksRaycasts = canvasGroup.blocksRaycasts == true ? false : true;
    }

    public void CloseSingle(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    public void UpdateStackSize(IClickable clickable) //stacklerime tıklanabilirlik
    {
        if (clickable.MyCount > 1) //eğer slotum birden fazla öğeye sahipse 
        {
            clickable.MyStackText.text = clickable.MyCount.ToString();
            clickable.MyStackText.enabled = true;
            clickable.MyIcon.enabled = true;
        }
        else  //sadece 1 öğeye sahipse 
        {
            clickable.MyStackText.enabled = false;
            clickable.MyIcon.enabled = true;
        }
        if (clickable.MyCount == 0) //eğer slot boşsa iconu sakla, texti sakla
        {
            clickable.MyIcon.enabled = false;
            clickable.MyStackText.enabled = false;
        }
    }

    public void ClearStackCount(IClickable clickable) //üstüne başka bir icon koyduğumda action butonumun yenilensin diye
    {
        clickable.MyStackText.enabled = false;
        clickable.MyIcon.enabled = true;
    }

    public void ShowTooltip(Vector2 pivot, Vector3 position, IDescribable description) //tooltip görünürlüğü için slotscriptte yazdın.
    {
        tooltipRect.pivot = pivot;
        tooltip.SetActive(true);
        tooltip.transform.position = position;
        tooltipText.text = description.GetDescription();
    }

    public void HideTooltip()
    {
        tooltip.SetActive(false);
    }

    public void RefreshTooltip(IDescribable description)
    {
        tooltipText.text = description.GetDescription();
    }
}
