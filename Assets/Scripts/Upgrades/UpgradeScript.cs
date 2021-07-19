using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class UpgradeScript : MonoBehaviour
{
    private static readonly int Color = Shader.PropertyToID("Color_f5f60154132a4cffba29c65eb104b6c4");
    private static readonly int Texture2D = Shader.PropertyToID("Texture2D_d6c8ef4ed772428397c28e34fb1ee6c1");
    private GameObject Popup;
    private new Collider collider;
    private TweenerCore<Quaternion, Vector3, QuaternionOptions> loopTween;
    private new Renderer renderer;

    private UpgradeData upgradeData;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        collider = GetComponent<Collider>();
        Popup = Resources.Load<GameObject>("Prefabs/Popup");
    }

    private void Start()
    {
        loopTween =
            transform.DOLocalRotate(new Vector3(45, 45, 45), 5).SetLoops(-1, LoopType.Incremental).SetEase(
                Ease.InOutSine);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("Player")) return;
        var popup =Instantiate(Popup);
        popup.GetComponent<UpgradePopup>().Setup(other.gameObject, upgradeData.UpgradeTexture, Enum.GetName(typeof(UpgradeEnum), upgradeData.UpgradeEnum).SplitCamelCase(), upgradeData.Color);
        collider.enabled = false;
        OnUpgradeAcquired(other.gameObject);
        SoundManager.Provider.PlayRandomSoundFromPack(SoundType.Powerup, 0.8f);
        transform.DOScale(Vector3.zero, 1f).SetEase(Ease.OutExpo).OnComplete(() => { loopTween.Kill(); });

        loopTween.OnKill(() => Destroy(gameObject));
    }


    public void SetUpgradeData(UpgradeData data)
    {
        upgradeData = data;
        renderer.material.SetTexture(Texture2D, upgradeData.UpgradeTexture);
        renderer.material.SetColor(Color, upgradeData.Color);
    }


    private void OnUpgradeAcquired(GameObject other)
    {
        if (upgradeData.UpgradeEnum > UpgradeEnum.ShotEnumsBegin && upgradeData.UpgradeEnum < UpgradeEnum.ShotEnumsEnd)
        {
            other.transform.root.GetComponentInChildren<PlayerController>().Shot.SetShotStrategy(
                upgradeData.UpgradeEnum);
            return;
        }

        if (upgradeData.UpgradeEnum > UpgradeEnum.HeartEnumsBegin &&
            upgradeData.UpgradeEnum < UpgradeEnum.HeartEnumsEnd)
        {
            other.transform.root.GetComponentInChildren<PlayerController>().Heart.SetHeartStrategy(
                upgradeData.UpgradeEnum);
            return;
        }

        if (upgradeData.UpgradeEnum > UpgradeEnum.MoveEnumsBegin && upgradeData.UpgradeEnum < UpgradeEnum.MoveEnumsEnd)
            other.transform.root.GetComponentInChildren<PlayerController>().Movement.SetMoveStrategy(
                upgradeData.UpgradeEnum);
    }
}