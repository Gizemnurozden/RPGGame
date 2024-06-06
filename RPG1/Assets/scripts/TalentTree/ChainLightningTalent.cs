using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightningTalent : Talent
{
    public override bool Click()
    {
        if (base.Click())
        {
            SpellBook.MyInstance.LearnSpell("ChainLightning");
        }

        return false;
    }

    public override string GetDescription()
    {
        return $"Chain lightning<color=#ffd100>\nStrikes an enemy\nwith chain \nlightning</color>";

    }
}
