using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Debuff;
using UnityEngine;
using UnityEngine.UI;

public class WaterShockDebuff : Debuff
{
    public float MySpeedReduction { get; set; }

    public override string Name => "Water";

    public WaterShockDebuff(Image icon) :base(icon)
    {
        MyDuration = 1;
    }

    public override Debuff Clone()
    {
        WaterShockDebuff clone = (WaterShockDebuff)this.MemberwiseClone();

        return clone;
    }
    public override void Apply(Character character)
    {
        (character as Enemy).ChangeState(new StunnedState());
        base.Apply(character);
    }

    public override void Remove()
    {
        (character as Enemy).ChangeState(new PathState());
        base.Remove();
    }
}
