using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndLevelManager : MonoBehaviour
{
    private PlayerConfiguration[] playerConfigurations;
    private Dictionary<int, int> playerDictionary;


    private GameObject playerPrefab;
    private List<Transform> playerSpawns;
    private List<int> playerStandings;

    private void Awake()
    {
        playerConfigurations = GameManager.Instance.PlayerConfigurations.ToArray();
        playerDictionary = GameManager.Instance.PlayerStandings;
        playerStandings = new List<int>();
        playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
        playerSpawns = new List<Transform>();

        foreach (Transform child in transform)
            playerSpawns.Add(child);

        StartCoroutine(Wait());
    }

    private void Start()
    {
        CheckPlacement();
        
        int i = 0;
        
        foreach (int playerStanding in playerStandings)
        {
            GameObject player = Instantiate(playerPrefab, playerSpawns[i].position, playerSpawns[i].rotation);
            PlayerController playerController = player.GetComponentInChildren<PlayerController>();

            PlayerConfiguration configuration = GetPlayerIndex(playerStanding);

            if (configuration is null) continue;
            playerController.InputHandler.InitializePlayer(configuration);
            playerController.SetMaterial(configuration.playerMaterial);
            playerController.SetPlayerIndex(configuration.PlayerIndex);
            i++;
        }
    }

    private PlayerConfiguration GetPlayerIndex(int playerStanding)
    {
        return
            playerConfigurations.FirstOrDefault(
                playerConfiguration => playerConfiguration.PlayerIndex == playerStanding);
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(10);
        GameManager.Instance.CommitSudoku();
        OurSceneManager.LoadMenu();
    }

    private void CheckPlacement()
    {
        playerStandings = playerDictionary.OrderByDescending(pair => pair.Value).Select(pair => pair.Key).ToList();
    }
}