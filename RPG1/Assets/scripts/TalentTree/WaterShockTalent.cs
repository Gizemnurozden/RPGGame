using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterShockTalent : Talent
{
    WaterShockDebuff debuff;

    private string nextRank = string.Empty;

    private float procChance;
    private float procIncrease = 5;

    public void Start()
    {
        debuff = new WaterShockDebuff(icon);
        procChance = 5;
        debuff.ProcChance = procChance;
    }

    public override bool Click()
    {
        if (base.Click())
        {
            debuff.ProcChance = procChance;

            if (MyCurrentCount < 3)
            {
                procChance += procIncrease;
                nextRank = $"<color=#fffff>\n\nNext rank:\n</color><color=#ffd100>Your Water has a {debuff.ProcChance + procIncrease}%\nto stun the target for\n {debuff.MyDuration}second(s)</color>\n";

            }
            else
            {
                nextRank = string.Empty;
            }
            //Give the player the talent's ability
            SpellBook.MyInstance.GetSpell("Water").MyDebuff = debuff;
            UIManager.MyInstance.RefreshTooltip(this);
            return true;
        }
        return false;
    }

    public override string GetDescription()
    {

        return $"WaterShock<color=#ffd100>\nYour Water has a {debuff.ProcChance + procIncrease}%\nto stun the target for\n {debuff.MyDuration} second(s) </color>{nextRank}";


    }
}
