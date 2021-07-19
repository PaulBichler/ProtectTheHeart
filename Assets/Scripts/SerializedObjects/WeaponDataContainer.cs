using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData", order = 1)]
public class WeaponDataContainer : ScriptableObject
{
    public WeaponData[] weaponData;
}