using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipesDeliveredText;
    [SerializeField] private Button playAgainButton;

    private void Start()
    {
        playAgainButton.onClick.AddListener( () => {
            NetworkManager.Singleton.Shutdown();
            Loader.Load( Loader.Scene.MainMenuScene );
        } );
        KitchenGameManager.Instance.OnStateChanged += KitchenGameMAnager_OnStateChanged;
        Hide();
    }

    private void KitchenGameMAnager_OnStateChanged( object sender , System.EventArgs e )
    {
        if ( KitchenGameManager.Instance.IsGameOver() )
        {
            Show();

            recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfullRecipesAmount().ToString();
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
