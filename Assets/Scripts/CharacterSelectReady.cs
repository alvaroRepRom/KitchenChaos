using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }

    public event EventHandler OnreadyChanged;

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
        SetPlayerReadyClientRpc( serverRpcParams.Receive.SenderClientId );
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
            KitchenGameLobby.Instance.DeleteLobby();
            Loader.LoadNetwork( Loader.Scene.GameScene );
        }
    }


    [ClientRpc]
    private void SetPlayerReadyClientRpc( ulong clientId )
    {
        playerReadyDictionary[clientId] = true;

        OnreadyChanged?.Invoke( this, EventArgs.Empty );
    }

    public bool IsPlayerReady( ulong clientId )
    {
        return playerReadyDictionary.ContainsKey( clientId ) && playerReadyDictionary[clientId];
    }
}
