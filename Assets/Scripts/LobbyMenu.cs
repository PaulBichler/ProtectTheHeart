using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private List<PlayerSlot> playerSlots;
    [SerializeField] private Text startGameTimerText;
    [SerializeField] private float startGameTimer;

    private float currentStartGameTimer;
    public static LobbyMenu Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
            Destroy(this);
        else
            Instance = this;

        currentStartGameTimer = startGameTimer;
    }

    private void Update()
    {
        if (GameManager.Instance.ReadyToStartGame)
        {
            if (!startGameTimerText.gameObject.activeSelf)
                startGameTimerText.gameObject.SetActive(true);

            startGameTimerText.text = ((int) currentStartGameTimer).ToString();

            if (currentStartGameTimer <= 0)
                OurSceneManager.LoadGame();
            else
                currentStartGameTimer -= Time.deltaTime;
        }
        else if (startGameTimerText.gameObject.activeSelf && currentStartGameTimer > 0)
        {
            startGameTimerText.gameObject.SetActive(false);
            currentStartGameTimer = startGameTimer;
        }
    }

    public void HandlePlayerJoin(PlayerInput playerInput, PlayerConfiguration config)
    {
        if (config is null) return;

        PlayerSlot slot = playerSlots[playerInput.playerIndex];
        slot.joinPrompt.gameObject.SetActive(false);
        slot.ready.SetActive(false);
        slot.unready.SetActive(true);

        //spawn player
        GameObject prefab = GameManager.Instance.PlayerPrefab;
        int playerIndex = playerInput.playerIndex;
        GameObject player = Instantiate(prefab, playerSlots[playerIndex].spawn.position,
            playerSlots[playerIndex].spawn.rotation, playerSlots[playerIndex].spawn);
        player.transform.GetChild(0).SetLayerRecursive(6 + playerIndex);
        PlayerController playerController = player.GetComponentInChildren<PlayerController>();

        playerController.SetPlayerIndex(playerIndex);
        playerController.InputHandler.InitializePlayer(config);
        playerController.SetMaterial(config.playerMaterial);
    }

    public void SetReady(int playerIndex)
    {
        bool ready = GameManager.Instance.ToggleReadyPlayer(playerIndex);
        PlayerSlot slot = playerSlots[playerIndex];
        slot.ready.SetActive(ready);
        slot.unready.SetActive(!ready);
    }

    [Serializable]
    public struct PlayerSlot
    {
        public Transform spawn;
        public Text joinPrompt;
        public GameObject ready;
        public GameObject unready;
    }
}