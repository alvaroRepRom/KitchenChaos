using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameManager : NetworkBehaviour
{
    public static KitchenGameManager Instance { get; private set; }


    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnPaused;
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnpaused;
    public event EventHandler OnLocalPlayerReadyChanged;


    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    private NetworkVariable<State> state = new NetworkVariable<State>( State.WaitingToStart );
    private bool isLocalPlayerReady;
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingToStartTimer = new NetworkVariable<float>(0f);
    private float gamePlayingToStartTimerMax = 90f;
    private bool isLocalGamePaused = false;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPauseDictionary;
    private bool autoTestGamePausedState;


    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong , bool>();
        playerPauseDictionary = new Dictionary<ulong , bool>();
    }


    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

        if ( IsServer )
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback( ulong clientId )
    {
        autoTestGamePausedState = true;
    }

    private void IsGamePaused_OnValueChanged( bool previousValue , bool newValue )
    {
        if ( isGamePaused.Value )
        {
            Time.timeScale = 0f;
            OnMultiplayerGamePaused?.Invoke( this , EventArgs.Empty );
        }
        else
        {
            Time.timeScale = 1f;
            OnMultiplayerGameUnpaused?.Invoke( this , EventArgs.Empty );
        }
    }

    private void State_OnValueChanged( State previousValue , State newValue )
    {
        OnStateChanged?.Invoke( this , EventArgs.Empty );
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction( object sender , EventArgs e )
    {
        if ( state.Value == State.WaitingToStart )
        {
            isLocalPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke( this , EventArgs.Empty );
            SetPlayerReadyServerRpc();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc( ServerRpcParams serverRpcParams = default )
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool isAllPlayersReady = true;

        foreach ( ulong clientId in NetworkManager.Singleton.ConnectedClientsIds )
        {
            if ( !playerReadyDictionary.ContainsKey( clientId ) || !playerReadyDictionary[clientId] )
            {
                isAllPlayersReady = false;
                break;
            }
        }

        if ( isAllPlayersReady )
        {
            state.Value = State.CountdownToStart;
        }
    }



    private void GameInput_OnPauseAction( object sender , EventArgs e )
    {
        TogglePauseGame();
    }

    private void Update()
    {
        if ( !IsServer ) return;

        switch ( state.Value )
        {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if ( countdownToStartTimer.Value < 0 )
                {
                    state.Value = State.GamePlaying;
                    gamePlayingToStartTimer.Value = gamePlayingToStartTimerMax;
                }
                break;
            case State.GamePlaying:
                gamePlayingToStartTimer.Value -= Time.deltaTime;
                if ( gamePlayingToStartTimer.Value < 0 )
                {
                    state.Value = State.GameOver;
                }
                break;
            case State.GameOver:
                break;
        }
    }

    private void LateUpdate()
    {
        if ( autoTestGamePausedState )
        {
            autoTestGamePausedState = false;
            TestGamePausedState();
        }
    }


    public bool IsGamePlaying() => state.Value == State.GamePlaying;
    public bool IsCountdownToStartActive() => state.Value == State.CountdownToStart;
    public float CountdownToStartTimer() => countdownToStartTimer.Value;
    public bool IsGameOver() => state.Value == State.GameOver;
    public bool IsPlayerLocalReady() => isLocalPlayerReady;
    public float GetGamePlayingTimeNormalized() => 1 - gamePlayingToStartTimer.Value / gamePlayingToStartTimerMax;


    public void TogglePauseGame()
    {
        isLocalGamePaused = !isLocalGamePaused;
        if ( isLocalGamePaused )
        {
            PauseGameServerRpc();
            OnLocalGamePaused?.Invoke( this , EventArgs.Empty );
        }
        else
        {
            UnPauseGameServerRpc();
            OnLocalGameUnPaused?.Invoke( this , EventArgs.Empty );
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc( ServerRpcParams serverRpcParams = default )
    {
        playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = true;

        TestGamePausedState();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void UnPauseGameServerRpc( ServerRpcParams serverRpcParams = default )
    {
        playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = false;

        TestGamePausedState();
    }


    private void TestGamePausedState()
    {
        foreach ( ulong clientID in NetworkManager.Singleton.ConnectedClientsIds )
        {
            if ( playerPauseDictionary.ContainsKey( clientID ) && playerPauseDictionary[clientID] )
            {
                isGamePaused.Value = true;
                return;
            }
        }
        isGamePaused.Value = false;
    }
}
