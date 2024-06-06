
using UnityEngine;
using UnityEngine.EventSystems;

public class ImprovedFireball : Talent
{
    public override bool Click()
    {
        if (base.Click())
        {
            //Give the player the talent's ability
            SpellBook.MyInstance.GetSpell("Fireball").MyCastTime -= 0.1f;
            return true;
        }
        return false;
    }

    public override string GetDescription()
    {
        return string.Format("Improved Fireball\n<color=#ffd100>Reduces the castingtime\nof your Fireball by .01 sec. </color>");
    }

   
}
