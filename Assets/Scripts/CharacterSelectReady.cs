using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }


    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }


    public void SetPlayerRead()
    {
        SetPlayerReadyServerRpc();
    }


    [ServerRpc( RequireOwnership = false )]
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
            Loader.LoadNetwork( Loader.Scene.GameScene );
        }
    }
}
