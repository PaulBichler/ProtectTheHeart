using System.Collections;
using UnityEngine;

public class BasicMove : IMoveStrategy
{
    private readonly PlayerMovement movement;
    private readonly float dashTime ;
    private readonly float dashSpeed ;

    public BasicMove(PlayerMovement movement, float dashTime = 0.25f, float dashSpeed = 20f)
    {
        this.movement = movement;
        this.dashTime = dashTime;
        this.dashSpeed = dashSpeed;
    }
    
    public virtual IEnumerator Dash()
    {
        float startTime = Time.time;
        Vector3 dir = movement.MoveDirection;

        while (Time.time < startTime + dashTime)
        {
            movement.RB.AddForce(dir * (dashSpeed * 2000));
            yield return null;
        }
    }
}