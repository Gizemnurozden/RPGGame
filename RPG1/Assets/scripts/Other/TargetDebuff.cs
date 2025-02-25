using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Debuff;
using UnityEngine;
using UnityEngine.UI;

public class TargetDebuff : MonoBehaviour
{
    [SerializeField]
    private Image durationImage;

    [SerializeField]
    private Image icon;

    public Debuff Debuff { get; private set; }

    public void Initialize(Debuff debuff)
    {
        this.Debuff = debuff;
        this.icon.sprite = debuff.MyIcon.sprite;
        this.durationImage.fillAmount = 0;
    }
   

    // Update is called once per frame
    void Update()
    {
        durationImage.fillAmount = Debuff.Elapsed / Debuff.MyDuration;
    }
}
