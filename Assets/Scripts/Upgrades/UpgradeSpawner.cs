using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeSpawner : MonoBehaviour
{
    public float MinInterval;
    public float MaxInterval;
    private readonly List<Transform> UpgradePositions = new List<Transform>();
    private readonly Dictionary<int, GameObject> UpgradesSpawnedDict = new Dictionary<int, GameObject>();
    private float _CurrentInterval;

    private float _Timer;

    private UpgradeDataContainer upgradeDataContainer;
    private GameObject UpgradeDataGO;

    private void Awake()
    {
        foreach (Transform trans in transform)
            UpgradePositions.Add(trans);

        upgradeDataContainer = Resources.Load<UpgradeDataContainer>("Prefabs/Upgrades/UpgradeData");
        UpgradeDataGO = Resources.Load<GameObject>("Prefabs/Upgrades/UpgradePrefab");
    }

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        _Timer += Time.deltaTime;

        foreach (
            KeyValuePair<int, GameObject> keyValuePair in
                UpgradesSpawnedDict.Where(keyValuePair => keyValuePair.Value is null))
            UpgradesSpawnedDict.Remove(keyValuePair.Key);

        if (!(_Timer >= _CurrentInterval)) return;
        SpawnUpgrade();
        ResetTimer();
    }

    private void ResetTimer()
    {
        _CurrentInterval = Mathf.Lerp(MinInterval, MaxInterval, Random.value);
        _Timer = 0;
    }

    private void SpawnUpgrade()
    {
        UpgradeData data = upgradeDataContainer.UpgradeData[Random.Range(0, upgradeDataContainer.UpgradeData.Length)];

        int upgradeIndex = Random.Range(0, UpgradePositions.Count);

        if (UpgradesSpawnedDict.ContainsKey(upgradeIndex))
        {
            GameObject o = UpgradesSpawnedDict[upgradeIndex];
            if (o != null)
                return;

            UpgradesSpawnedDict[upgradeIndex] = Instantiate(UpgradeDataGO, UpgradePositions[upgradeIndex]);
            UpgradesSpawnedDict[upgradeIndex].GetComponent<UpgradeScript>().SetUpgradeData(data);
            return;
        }

        GameObject go = Instantiate(UpgradeDataGO, UpgradePositions[upgradeIndex]);
        go.GetComponent<UpgradeScript>().SetUpgradeData(data);
        UpgradesSpawnedDict.Add(upgradeIndex, go);
    }
}