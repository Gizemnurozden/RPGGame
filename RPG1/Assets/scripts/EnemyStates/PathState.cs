
using UnityEngine;

public class PathState : IState
{
 

    private Vector3 destination;

    private Vector3 current;

    private Vector3 goal;

    private Transform transform;

    private Enemy parent;

    private Vector3 targetPos;

    public void Enter(Enemy _parent)
    {

        parent = _parent;

        transform = parent.transform.parent;

        targetPos = Player.MyInstance.MyCurrentTile.position;

        if (targetPos != parent.MyCurrentTile.position)
        {
            
            parent.MyPath = parent.MyAstar.Algorithm(parent.MyCurrentTile.position, targetPos);

        }
        if (parent.MyPath != null && parent.MyPath.Count > 0)
        {
            current = parent.MyPath.Pop();
           
            destination = parent.MyPath.Pop();

           // this.goal = parent.MyCurrentTile.position;
        }
        else
        {
           
            parent.ChangeState(new EvadeState());
        }

    }

    public void Exit()
    {
        parent.MyPath = null;
    }

    

    public void Update()
    {
        if (parent.MyPath != null && parent.MyPath.Count > 0)
        {
            transform.position = Vector2.MoveTowards( transform.position, destination, parent.CurrentSpeed * Time.deltaTime);

            parent.ActivateLayer("WalkLayer");
            /*
            Vector3Int dest = parent.MyAstar.MyTilemap.WorldToCell(destination);
            Vector3Int cur = parent.MyAstar.MyTilemap.WorldToCell(current);

            float distance = Vector2.Distance(destination, transform.position);

           float totalDistance = Vector2.Distance(parent.MyTarget.transform.position, transform.position);
            
            if (cur.y > dest.y)
            {
                parent.Direction = Vector2.down;
            }
            else if (cur.y < dest.y)
            {
                parent.Direction = Vector2.up;
            }
            if (cur.y == dest.y)
            {
                if (cur.x > dest.x)
                {
                    parent.Direction = Vector2.left;
                }
                else if (cur.x < dest.x)
                {
                    parent.Direction = Vector2.right;
                }
            }
            if (totalDistance <= parent.MyAttackRange)
            {
                
                parent.ChangeState(new AttackState());
            }
           
            else if (Player.MyInstance.MyCurrentTile.position == parent.MyCurrentTile.position)
            {
                
                parent.ChangeState(new FollowState());
            }
            
            if (distance <= 0f)
            {
                if (parent.MyPath.Count > 0)
                {
                    current = destination;
                    destination = parent.MyPath.Pop();

                    if (targetPos != Player.MyInstance.MyCurrentTile.position) //oyuncu hareket ediyo demektir.
                    {
                        Debug.Log("here");
                        parent.ChangeState(new PathState());
                    }

                }
                else
                {
                   
                    parent.MyPath = null;
                    parent.ChangeState(new PathState());
                }
            }

            */
            if (Vector2.Distance(transform.position, destination) <= 0.1f)
            {
                current = destination;

                if (parent.MyPath.Count > 0)
                {
                    destination = parent.MyPath.Pop();
                }
                else
                {
                    parent.MyPath = null;
                    parent.ChangeState(new PathState());
                }
            }
        }
        else
        {
            parent.ChangeState(new FollowState()); //pathstate
        }
    }
}
