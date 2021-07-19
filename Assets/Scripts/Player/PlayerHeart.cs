using UnityEngine;

public class PlayerHeart : MonoBehaviour, IDamage
{
    private IHeartStrategy heartStrategy;

    public PlayerController playerController;

    private void Awake()
    {
    }

    public void SetPlayerController(PlayerController controller)
    {
        playerController = controller;
        SetHeartStrategy(UpgradeEnum.BasicHeart);
    }
    
    public void DoDamage(int index)
    {
        heartStrategy.DoDamage(index);
    }

    public void SetHeartStrategy(UpgradeEnum upgrade)
    {
        switch (upgrade)
        {
            case UpgradeEnum.BasicHeart:
                SetHeartStrategy(new BasicHeart(this, playerController.PlayerIndex));
                break;
            case UpgradeEnum.Healthier:
                SetHeartStrategy(new HealthyHeart(this, playerController.PlayerIndex));
                break;
            case UpgradeEnum.Invincibility:
                SetHeartStrategy(new Invincibility(this, playerController.PlayerIndex));
                break;
        }
    }

    private void SetHeartStrategy(IHeartStrategy strategy)
    {
        heartStrategy = strategy;
    }

    public void CommitSudoku()
    {
        Destroy(gameObject);
    }
}