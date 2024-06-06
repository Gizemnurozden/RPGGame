using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Debuff
{
    class IgniteDebuff: Debuff
    {
        public float MyTickDamage { get; set; }

        public override string Name
        {
            get { return "Ignite"; }
        }

        private float elapsed;

        public IgniteDebuff(Image icon) : base(icon)
        {

            MyDuration = 20;
         

        }

        public override void Update()
        {

            elapsed += Time.deltaTime;

            if (elapsed >= MyDuration/MyTickDamage)
            {
                character.TakeDamage(MyTickDamage, null);
                elapsed = 0;
            }

            base.Update();
        }

        public override void Remove()
        {
            elapsed = 0;

            base.Remove();
        }

        public override Debuff Clone()
        {
            IgniteDebuff clone =(IgniteDebuff) this.MemberwiseClone();
            return clone;
        }
    }

}


