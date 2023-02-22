using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipesDeliveredText;

    private void Start()
    {
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
