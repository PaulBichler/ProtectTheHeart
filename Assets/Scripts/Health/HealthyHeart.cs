public class HealthyHeart : BasicHeart
{
    private bool isDying;

    public HealthyHeart(PlayerHeart playerHeart, int playerIndex) : base(playerHeart, playerIndex)
    {
        isDying = false;
        ChangeHeart();
    }


    private void ChangeHeart()
    {
        playerHeart.transform.localScale *= 2;
    }

    public override void DoDamage(int index)
    {
        if (isDying)
            base.DoDamage(index);
        isDying = true;
        SoundManager.Provider.PlayRandomSoundFromPack(SoundType.Die);
    }
}