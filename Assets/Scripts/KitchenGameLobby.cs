using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Runtime.CompilerServices;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;

public class KitchenGameLobby : MonoBehaviour
{
    public static KitchenGameLobby Instance { get; private set; }


    private Lobby joinedLobby;

    private float heartBeatTimer;


    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad( gameObject );

        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication()
    {
        if ( UnityServices.State != ServicesInitializationState.Initialized )
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile( Random.Range( 0 , 10000 ).ToString() );

            await UnityServices.InitializeAsync( initializationOptions );
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private void Update()
    {
        HandleHeartBeat();
    }

    private void HandleHeartBeat()
    {
        if ( IsLobbyHost() )
        {
            heartBeatTimer -= Time.deltaTime;
            if ( heartBeatTimer <= 0 )
            {
                float maxHeartBeatTimer = 15f;
                heartBeatTimer = maxHeartBeatTimer;

                LobbyService.Instance.SendHeartbeatPingAsync( joinedLobby.Id );
            }
        }
    }

    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public async void CreateLobby( string lobbyName , bool isPrivate )
    {
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync( lobbyName ,
                KitchenGameMultiplayer.MAX_PLAYER_AMOUNT ,
                new CreateLobbyOptions
                {
                    IsPrivate = isPrivate
                } );

            KitchenGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork( Loader.Scene.CharacterSelectScene );
        }
        catch (LobbyServiceException e )
        {
            Debug.LogError( e );
        }

    }

    public async void QuickJoin()
    {
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch ( LobbyServiceException e )
        {
            Debug.LogError( e );
        }
    }


    public async void JoinLobbyCode( string lobbyCode )
    {
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync( lobbyCode );

            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch ( LobbyServiceException e )
        {
            Debug.LogError( e );
        }
    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }

}
