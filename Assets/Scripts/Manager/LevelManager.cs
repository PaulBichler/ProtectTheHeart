using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private PlayerConfiguration[] playerConfigurations;

    private GameObject playerPrefab;
    private List<Transform> playerSpawns;

    private void Awake()
    {
        playerConfigurations = GameManager.Instance.PlayerConfigurations.ToArray();
        playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
        playerSpawns = new List<Transform>();

        foreach (Transform child in transform)
            playerSpawns.Add(child);
    }

    private void Start()
    {
        for (int i = 0; i < playerConfigurations.Length; i++)
        {
            GameObject player = Instantiate(playerPrefab, playerSpawns[i].position, playerSpawns[i].rotation);
            player.name = "Player " + i;
            PlayerController playerController = player.GetComponentInChildren<PlayerController>();

            playerController.InputHandler.InitializePlayer(playerConfigurations[i]);
            playerController.SetMaterial(playerConfigurations[i].playerMaterial);
            playerController.SetPlayerIndex(playerConfigurations[i].PlayerIndex);

            player.transform.GetChild(0).SetLayerRecursive(i + 6);
        }
    }
}