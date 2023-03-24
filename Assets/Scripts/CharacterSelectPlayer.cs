using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private PlayerVisual playerVisual;

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnreadyChanged += CharacterSelectReady_OnreadyChanged;
        
        UpdatePlayer();
    }

    private void CharacterSelectReady_OnreadyChanged( object sender , System.EventArgs e )
    {
        UpdatePlayer();
    }

    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged( object sender , System.EventArgs e )
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if ( KitchenGameMultiplayer.Instance.IsPlayerIndexConnected( playerIndex ) )
        {
            Show();

            PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex( playerIndex );

            readyGameObject.SetActive( CharacterSelectReady.Instance.IsPlayerReady( playerData.clientId ) );

            playerVisual.SetPlayerColor( KitchenGameMultiplayer.Instance.GetPlayerColor( playerData.colorId ) );
        }
        else
        {
            Hide();
        }
    }


    private void Show()
    {
        gameObject.SetActive( true );
    }

    private void Hide()
    {
        gameObject.SetActive( false );
    }    
}
