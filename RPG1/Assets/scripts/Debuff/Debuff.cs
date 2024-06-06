using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Debuff
{

    public abstract class Debuff
    {
        public float MyDuration { get; set; }

        public float ProcChance { get; set; }

        public float Elapsed { get; set; }

        protected Character character;

        public Image MyIcon { get; set; }

        public abstract string Name
        {
            get;
        }

        public Debuff(Image image)
        {
            this.MyIcon = image;
        }

        public virtual void Apply(Character character)
        {
            this.character = character;
            character.ApplyDebuff(this);
            UIManager.MyInstance.AddDebuffToTargetFrame(this);
        }
        public virtual void Remove()
        {
            character.RemoveDebuff(this);
            Elapsed = 0;
        }
        public virtual void Update()
        {
            Elapsed += Time.deltaTime;

            if (Elapsed >= MyDuration)
            {
                Remove();
            }

        }

        public abstract Debuff Clone();
      
    }

}