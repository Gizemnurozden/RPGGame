
using UnityEngine;

class StunnedState : IState
{
    public void Enter(Enemy parent)
    {
        parent.Direction = Vector2.zero;
    }

    public void Exit()
    {
      
    }

    public void Update()
    {
       
    }
}
