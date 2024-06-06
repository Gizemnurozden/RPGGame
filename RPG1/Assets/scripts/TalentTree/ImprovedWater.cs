using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImprovedWater : Talent
{
    private int percent = 5;
    public override bool Click()
    {
        if (base.Click())
        {
            Spell water = SpellBook.MyInstance.GetSpell("Water");

            water.MyDamage += (water.MyDamage / 100) * percent;
            return true;
        }
        return false;
    }

    public override string GetDescription()
    {
        return string.Format($"Improved Water\n<color=#ffd100>Increas the damage\nof your Water by {percent}% </color>");
    }

   
}
