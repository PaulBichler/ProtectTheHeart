using DG.Tweening;
using UnityEngine;

public class DamageHeart : MonoBehaviour, IDamage
{
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    public void DoDamage(int index)
    {
        transform.DOShakeScale(1f, Vector3.one * 5).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            Debug.Log($"index = {index}");
            EventManager.InvokeOnAction(new GamePlayData(EventAction.PlayerDied, playerController.PlayerIndex, index));
            Destroy(gameObject);
        });
    }
}