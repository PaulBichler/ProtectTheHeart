using System.Collections;
using UnityEngine;

public class DoubleDash : BasicMove
{
    public DoubleDash(PlayerMovement movement, float dashTime = 0.25f, float dashSpeed = 20) : base(movement, dashTime, dashSpeed)
    {
    }

    public override IEnumerator Dash()
    {
        yield return base.Dash();

        yield return base.Dash();
    }
}