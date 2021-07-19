using System;
using System.Collections;
using GMTK;
using UnityEngine;

public class PlayerShot : MonoBehaviour
{
    [SerializeField] private Transform _startPos;
    [SerializeField] private UpgradeEnum upgradeEnum;

    private GameObject _projectile;
    private IShotStrategy _shotStrategy;

    private bool canShot;
    private float currentTime;
    private WeaponDataContainer weaponDataContainer;

    private Animator playerAnimator;

    private GameObject Projectile => _projectile ? _projectile : LoadProjectile();


    private void Awake()
    {
        weaponDataContainer = Resources.Load<WeaponDataContainer>("WeaponData");

        SetShotStrategy(UpgradeEnum.Basic);
        canShot = true;

        playerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_shotStrategy is null) return;
        if (currentTime > _shotStrategy.GetFireRate())
        {
            canShot = true;
            currentTime = 0;
            playerAnimator.ResetTrigger("Shoot");
            return;
        }

        if (canShot == false) currentTime += Time.deltaTime;
    }

    private void OnValidate()
    {
        SetShotStrategy(upgradeEnum);
    }

    private GameObject LoadProjectile()
    {
        return _projectile = Resources.Load<GameObject>(HelperClass.GetBulletColors(gameObject.layer));
    }

    private void SetShotStrategy(IShotStrategy shotStrategy)
    {
        _shotStrategy = shotStrategy;
    }


    public void Shot()
    {
        if (_shotStrategy is null) return;
        if (!canShot) return;
        _shotStrategy.Shot(transform, Projectile, _startPos.position);
        canShot = false;
        
        playerAnimator.SetTrigger("Shoot");
        GameObject muzzlePrefab = Resources.Load<GameObject>(HelperClass.GetMuzzleColors(gameObject.layer));
        var muzzle = Instantiate(muzzlePrefab, _startPos.position, _startPos.rotation, transform);
        Destroy(muzzle, 1f);
    }

    public void SetShotStrategy(UpgradeEnum upgrade)
    {
        switch (upgrade)
        {
            case UpgradeEnum.Basic:
                SetShotStrategy(new BasicShot(weaponDataContainer.weaponData[0]));
                break;
            case UpgradeEnum.RicochetOne:
                SetShotStrategy(new RicochetOneBasicShot(weaponDataContainer.weaponData[1]));
                break;
            case UpgradeEnum.Explode:
                SetShotStrategy(new ExplodeBasicShot(weaponDataContainer.weaponData[2]));
                break;
            case UpgradeEnum.MultiShot:
                SetShotStrategy(new MultiShot(weaponDataContainer.weaponData[3]));
                break;
            case UpgradeEnum.SpectralShot:
                SetShotStrategy(new SpectralShot(weaponDataContainer.weaponData[4]));
                break;
            case UpgradeEnum.HugeShot:
                SetShotStrategy(new SizeShot(weaponDataContainer.weaponData[5]));
                break;
            case UpgradeEnum.SmallShot:
                SetShotStrategy(new SizeShot(weaponDataContainer.weaponData[6]));
                break;
            case UpgradeEnum.BurstShot:
                SetShotStrategy(new BurstShot(weaponDataContainer.weaponData[7]));
                break;
            case UpgradeEnum.HeatSeekShot:
                SetShotStrategy(new HeatSeekShot(weaponDataContainer.weaponData[8]));
                break;
        }
    }
}