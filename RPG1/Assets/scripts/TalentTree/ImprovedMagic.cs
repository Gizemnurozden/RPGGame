
using UnityEngine;
using UnityEngine.EventSystems;

public class ImprovedMagic : Talent
{ 
    public override bool Click()
    {
        if (base.Click())
        {
            //Give the player the talent's ability
            SpellBook.MyInstance.GetSpell("Magic").MyRange += 1;
            return true;
        }
        return false;
    }

    public override string GetDescription()
    {
        return string.Format("Improved Magic\n<color=#ffd100>Increases the range\nof your Magic by 1 </color>");
    }

}
