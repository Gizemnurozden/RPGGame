using System.Collections;

using UnityEngine;

public class AttackState : IState
{
    private Enemy parent;

    private float attackCooldown = 1;

    private float extraRange = .1f;

    public void Enter(Enemy parent)
    {
        this.parent = parent;
        parent.MyRigidbody.velocity = Vector2.zero;
        parent.Direction = Vector2.zero;
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        if (parent.MyAttackTime >= attackCooldown && !parent.IsAttacking)
        {
            parent.MyAttackTime = 0;

            parent.StartCoroutine(Attack());
        }


        if (parent.MyTarget != null)
        {
            //hedefle düşman arasındaki mesafeyi hesaplar.
          
            float distance = Vector2.Distance(parent.MyTarget.transform.position, parent.transform.position);

            if (distance >= parent.MyAttackRange+ extraRange && !parent.IsAttacking) //eğer mesafem attackrangeden büyükse hareket et.
            {
                if (parent is RangedEnemy)
                {
                    parent.ChangeState(new PathState());
                }
                else
                {
                    parent.ChangeState(new FollowState()); //hedefi takip et
                }
                
                
                
            }

            //We need to chech range and attack
        }
        else //hedefi kaybedersek duracağımız ıdle
        {
            parent.ChangeState(new IdleState());
        }
    }

    public IEnumerator Attack()
    {
        parent.IsAttacking = true;

        parent.MyAnimator.SetTrigger("attack");

        yield return new WaitForSeconds(parent.MyAnimator.GetCurrentAnimatorStateInfo(2).length);

        parent.IsAttacking = false;

    }
}
