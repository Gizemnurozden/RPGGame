using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{

    [SerializeField]
    private GameObject weaponPrefab;

   [SerializeField]
   private Transform[] exitPoints;


    private float fieldOfView = 120;

    protected override void Update()
    {
        LookAtTarget();
        base.Update();
    }

    public void Shoot(int exitIndex)
   {
     SpellScript s = Instantiate(weaponPrefab, exitPoints[exitIndex].position, Quaternion.identity).GetComponent<SpellScript>();
      s.Initialize(MyTarget.MyHitBox, damage, this);
   }

    private void LookAtTarget()
    {
        if (MyTarget != null)
        {
            Vector2 directionToTarget = (MyTarget.transform.position - transform.position).normalized;

            Vector2 faceing = new Vector2(MyAnimator.GetFloat("x"), MyAnimator.GetFloat("y"));

            float angleToTarget = Vector2.Angle(faceing, directionToTarget);

            if (angleToTarget > fieldOfView /2)
            {
                MyAnimator.SetFloat("x", directionToTarget.x);
                MyAnimator.SetFloat("y", directionToTarget.y);
            }
        }
    }
}
