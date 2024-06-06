
using UnityEngine;

class FollowState : IState
{
    private Enemy parent;

    public void Enter(Enemy parent)
    {
        Player.MyInstance.AddAttacker(parent);
        this.parent = parent;
        
    }

    public void Exit()
    {
        parent.Direction = Vector2.zero;
        parent.MyRigidbody.velocity = Vector2.zero;
        
    }

    public void Update()
    {

        if (parent.MyTarget != null) //parent.ısattcking ben ekledim.
        {
            //hedefin yönünü bulur
            parent.Direction = (parent.MyTarget.transform.position - parent.transform.position).normalized;

            //düşmanı hedefedoğru hareket ettirir.
            parent.transform.position = Vector2.MoveTowards(parent.transform.position, parent.MyTarget.transform.position, parent.CurrentSpeed * Time.deltaTime);

            float distance = Vector2.Distance(parent.
                MyTarget.transform.position, parent.transform.position);

            if (distance <= parent.MyAttackRange)
            {
                parent.ChangeState(new AttackState());
            }
        }
        if (!parent.InRange || !parent.CanSeePlayer())
        {
            parent.ChangeState(new EvadeState());
        }
        
       
    }
}

