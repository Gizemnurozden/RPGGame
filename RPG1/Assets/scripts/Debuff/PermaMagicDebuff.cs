using System;
using UnityEngine.UI;

namespace Assets.Scripts.Debuff
{
    class PermaMagicDebuff : Debuff
    {

        public float MySpeedReduction { get; set; }

        public override string Name => "Magic";

       

        public PermaMagicDebuff(Image icon) : base(icon)
        {
            MyDuration = 3;
        }

        public override void Apply(Character character)
        {

            if (character.CurrentSpeed >= character.Speed)
            {
                character.CurrentSpeed = character.Speed - (character.Speed * (MySpeedReduction / 100));
                base.Apply(character);
            }
           
        }
        public override void Remove()
        {
            character.CurrentSpeed = character.Speed;
            base.Remove();
        }
        public override Debuff Clone()
        {
            PermaMagicDebuff clone = (PermaMagicDebuff)this.MemberwiseClone();

            return clone;
        }
    }
}
