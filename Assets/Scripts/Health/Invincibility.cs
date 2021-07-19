using UnityEngine;

public class Invincibility : BasicHeart
{

    public Invincibility(PlayerHeart playerHeart, int playerIndex) : base(playerHeart, playerIndex)
    {
    }

    public override void DoDamage(int index)
    {
        float rand = Random.value;

        if (rand > 0.5f)
            base.DoDamage(index);
    }
}