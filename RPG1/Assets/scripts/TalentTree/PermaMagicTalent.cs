using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Debuff;
using UnityEngine;

public class PermaMagicTalent : Talent
{
    PermaMagicDebuff debuff;

    private float speedReduction = 20;

    private float reductionIncrease = 10;

    private string nextRank = string.Empty;

    public void Start()
    {
        debuff = new PermaMagicDebuff(icon);
        debuff.MySpeedReduction = speedReduction;
    }

    public override bool Click()
    {
        if (base.Click())
        {
            debuff.MySpeedReduction = speedReduction;

            if (MyCurrentCount < 3)
            {
                speedReduction += reductionIncrease;
                nextRank = $"<color=#fffff>\n\nNext rank:\n</color><color=#ffd100>Your Magic applies a debuff\nto the target that\nreduces the movement speed \nby {debuff.MySpeedReduction+reductionIncrease} %</color>\n";

            }
            else
            {
                nextRank = string.Empty;
            }
            //Give the player the talent's ability
            SpellBook.MyInstance.GetSpell("Magic").MyDebuff = debuff;
            UIManager.MyInstance.RefreshTooltip(this);
            return true;
        }
        return false;
    }

    public override string GetDescription()
    {

        return $"PermaMagic<color=#ffd100>\nYour Magic applies a debuff\nto the target that\n reduces the movement speed by  {debuff.MySpeedReduction} % </color>{nextRank}";


    }
}
