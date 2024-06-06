using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stat : MonoBehaviour
{

    private Image content;

    [SerializeField]
    private Text statValue;

    [SerializeField]
    private float lerpSpeed;

    private float currentFill;

    public float MyMaxValue { get;  set; }

    private float currentValue;


    private float overflow; // stati ikinci levele geçirdikten sonra resetlerken ikinci levelde birinci olan puanım kalsın diye.
    
    public bool IsFull //xp stat fulleştirmesi kontrolü için prop
    {
        get { return content.fillAmount == 1; }
    }

    public float MyOverflow
    {
        get
        {
            float tmp = overflow;
            overflow = 0;
            return tmp;
        }
    }

    public float MyCurrentValue
    {
        get
        {
            return currentValue;
        }

        set
        {
            //verilen değerlerin kontrolü
          if (value > MyMaxValue)
              {

                overflow = value - MyMaxValue;
                currentValue = MyMaxValue;

              }
             else if (value < 0)
             {
                 currentValue = 0;
             }
             else
             {
                 currentValue = value;
             }
          //sağlık ve mananın dolum görüntüsünün değer ayarlanması
            currentFill = currentValue / MyMaxValue;

            if (statValue != null)
            {
                statValue.text = currentValue + " / " + MyMaxValue;
            }
          
        }
    }
  

    // Start is called before the first frame update


    void Start()
    {
        
        content = GetComponent<Image>();
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleBar();
    }
    public void HandleBar()
    {
        // azalıp artmasının belli bir hız ve zamana göre ayarlanması
        if (currentFill != content.fillAmount)
        {
            content.fillAmount = Mathf.MoveTowards(content.fillAmount, currentFill, Time.deltaTime * lerpSpeed);

        }

    }

    public void Initialized(float currentValue, float maxValue)
    {
        if (content == null)
        {
            content = GetComponent<Image>();
        }

        //maksimum ve değerin ayarlanması
        MyMaxValue = maxValue;
        MyCurrentValue = currentValue;
        content.fillAmount = MyCurrentValue / MyMaxValue; //hedef değişince framedeki can sçeilen hedefle hemen değişsin.dolu olarak gelsin.
    }

    public void Reset() //xp için stati resetlesin diye playerde kullandık.
    {
        content.fillAmount = 0;
    }
}
