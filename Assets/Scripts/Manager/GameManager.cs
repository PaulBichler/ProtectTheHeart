using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int maxPlayers = 4;
    [SerializeField] private int minPlayers = 2;
    [SerializeField] private List<Material> playerMaterials;
    private int currentGameCount = 1;

    private List<int> players;

    public static GameManager Instance { get; private set; }
    public List<PlayerConfiguration> PlayerConfigurations { get; private set; }

    public bool ReadyToStartGame { get; private set; }
    public GameObject PlayerPrefab { get; private set; }

    public Dictionary<int, int> PlayerStandings { get; private set; }


    private void Awake()
    {
        SetInstance();

        PlayerConfigurations = new List<PlayerConfiguration>();
        PlayerPrefab = Resources.Load<GameObject>("Prefabs/Player");

        PlayerStandings = new Dictionary<int, int>();

        players = new List<int>();
    }

    private void OnEnable()
    {
        EventManager.OnAction += HandleEvent;
    }

    private void OnDisable()
    {
        EventManager.OnAction -= HandleEvent;
    }

    private void HandleEvent(EventData data)
    {
        switch (data.EventCategory)
        {
            case EventCategory.GamePlay:
                HandleGameplayEvents(data as GamePlayData);
                break;
        }
    }

    private void HandleGameplayEvents(GamePlayData data)
    {
        switch (data.EventAction)
        {
            case EventAction.PlayerDied:
                HandlePlayerDied(data);
                break;
        }
    }

    private int maxGames = 5;

    private void HandlePlayerDied(GamePlayData data)
    {
        players.Remove(data.PlayerIndex);
        
        bool won = AddToStandings(data);

        if (players.Count != 1) return;

        if (won)
        {
            OurSceneManager.LoadEndScene();
            return;
        }

        ResetPlayers();

        OurSceneManager.LoadGame();
        currentGameCount++;
    }

    private bool AddToStandings(GamePlayData data)
    {
        if (PlayerStandings.ContainsKey(data.KillerIndex))
        {
            var standing = ++PlayerStandings[data.KillerIndex];
            
            if (PlayerStandings.Count == 2)
            {
                var aaa = maxGames / 2;
                return standing > aaa;
            }
            
            return maxGames == currentGameCount;
        }
        else
        {
            PlayerStandings.Add(data.KillerIndex, 0);
            return false;
        }
    }

    private void ResetPlayers()
    {
        players.Clear();
        foreach (PlayerConfiguration playerConfiguration in PlayerConfigurations)
            players.Add(playerConfiguration.PlayerIndex);
    }

    private void SetInstance()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public PlayerConfiguration HandlePlayerJoin(PlayerInput playerInput)
    {
        if (PlayerConfigurations.Count == maxPlayers)
            return null;

        playerInput.transform.SetParent(transform);

        if (PlayerConfigurations.Any(pc => pc.PlayerIndex == playerInput.playerIndex)) return null;
        PlayerConfiguration playerConfig = new PlayerConfiguration(playerInput) {
            playerMaterial = playerMaterials[playerInput.playerIndex]
        };
        PlayerConfigurations.Add(playerConfig);

        players.Add(playerInput.playerIndex);
        PlayerStandings.Add(playerInput.playerIndex, 0);
        return playerConfig;
    }

    public bool ToggleReadyPlayer(int index)
    {
        PlayerConfigurations[index].isReady = !PlayerConfigurations[index].isReady;
        ReadyToStartGame = PlayerConfigurations.Count >= minPlayers && PlayerConfigurations.All(pc => pc.isReady);
        return PlayerConfigurations[index].isReady;
    }

    public void SetPlayerMaterial(int index, Material material)
    {
        PlayerConfigurations[index].playerMaterial = material;
    }

    public void CommitSudoku()
    {
        PlayerStandings.Clear();
        ReadyToStartGame = false;
        PlayerConfigurations.Clear();
        Destroy(gameObject);
    }
}