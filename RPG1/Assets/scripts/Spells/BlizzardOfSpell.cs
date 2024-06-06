using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlizzardOfSpell : AOESpell
{
    
    public override void Enter(Enemy enemy)
    {
        enemy.CurrentSpeed = 0;   //enemy.Speed
        base.Enter(enemy);
    }


    public override void Exit(Enemy enemy)
    {
        enemy.CurrentSpeed = enemy.Speed;
        base.Exit(enemy);
    }

}
