using DG.Tweening;
using UnityEngine;

public class BasicHeart : IHeartStrategy
{
    protected readonly PlayerHeart playerHeart;
    private readonly int playerIndex;

    public BasicHeart(PlayerHeart playerHeart, int playerIndex)
    {
        this.playerIndex = playerIndex;
        this.playerHeart = playerHeart;
        playerHeart.transform.localScale = Vector3.one;
    }


    public virtual void DoDamage(int index)
    {
        SoundManager.Provider.PlayRandomSoundFromPack(SoundType.Die);

        playerHeart.GetComponent<Collider>().enabled = false;
        
        CameraShake.Instance.Shake(.5f, .6f);
        
        playerHeart.playerController.Die();

        playerHeart.transform.DOShakeScale(1f, Vector3.one * 5).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            EventManager.InvokeOnAction(new GamePlayData(EventAction.PlayerDied, playerIndex, index));
            playerHeart.CommitSudoku();
        });
    }
}