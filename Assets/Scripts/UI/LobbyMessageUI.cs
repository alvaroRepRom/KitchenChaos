using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;


    private void Awake()
    {
        closeButton.onClick.AddListener( Hide );
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted += KitchenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed += KitchenGameLobby_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted += KitchenGameLobby_OnJoinStarted;
        KitchenGameLobby.Instance.OnQuickJoinFailed += KitchenGameLobby_OnQuickJoinFailed;
        KitchenGameLobby.Instance.OnJoinFailed += KitchenGameLobby_OnJoinFailed;

        Hide();
    }

    private void KitchenGameLobby_OnJoinFailed( object sender , System.EventArgs e )
    {
        ShowMessage( "Fail to join lobby" );
    }

    private void KitchenGameLobby_OnQuickJoinFailed( object sender , System.EventArgs e )
    {
        ShowMessage( "Cound not find a Lobby to quick join" );
    }

    private void KitchenGameLobby_OnJoinStarted( object sender , System.EventArgs e )
    {
        ShowMessage( "Joinning Lobby" );
    }

    private void KitchenGameLobby_OnCreateLobbyFailed( object sender , System.EventArgs e )
    {
        ShowMessage( "Fail to create lobby" );
    }

    private void KitchenGameLobby_OnCreateLobbyStarted( object sender , System.EventArgs e )
    {
        ShowMessage( "Creating Lobby..." );
    }

    private void KitchenGameMultiplayer_OnFailedToJoinGame( object sender , System.EventArgs e )
    {
        if ( NetworkManager.Singleton.DisconnectReason == "" )
            ShowMessage( "Failed to connect" );
        else
            ShowMessage( NetworkManager.Singleton.DisconnectReason );
    }


    private void ShowMessage( string message )
    {
        Show();
        messageText.text = message;
    }


    private void Show()
    {
        gameObject.SetActive( true );
    }

    private void Hide()
    {
        gameObject.SetActive( false );
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoinGame;
    }

}
