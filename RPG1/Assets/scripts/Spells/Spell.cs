using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Debuff;
using UnityEngine;

[Serializable]
public class Spell : IUseable, IMoveable,IDescribable,ICastable
{
    [SerializeField]
    private string title;

    [SerializeField]
    private float duration;

    [SerializeField]
    private float damage;

    [SerializeField]
    private float range;

    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float castTime;

    [SerializeField]
    private GameObject spellPrefab;

    [SerializeField]
    private Color barColor;

    [SerializeField]
    private bool needsTarget;


    [SerializeField]
    private string description;

    public Debuff MyDebuff { get; set; }

    public string MyTitle { get => title; }
    public float MyDamage { get => Mathf.Ceil(damage); set => damage = value; }
    public Sprite MyIcon { get => icon; }
    public float MySpeed { get => speed; }
    
    public GameObject MySpellPrefab { get => spellPrefab; }
    public Color MyBarColor { get => barColor; }
    public bool NeedsTarget { get => needsTarget; }

    public float MyCastTime
    {
        get
        {
             return castTime;
        }
        set { castTime = value; }

    }

    public float MyRange { get => range; set => range = value; }
    public float MyDuration { get => duration; set => duration = value; }

    public string GetDescription()
    {

        if (!needsTarget)
        {
            return $"{title}<color=#ffd100>\n{description}\nthat does{damage/MyDuration} damage\n every second for {MyDuration} seconds </color>";

        }
        else
        {

            return string.Format("{0}\nCast time: {1} second(s)\n<color=#ffd111>{2}\nthat causes {3} damage</color>", title, castTime, description, MyDamage);

        }



      
    }

    public void Use()
    {
        Player.MyInstance.CastSpell(this);
    }
}

  
