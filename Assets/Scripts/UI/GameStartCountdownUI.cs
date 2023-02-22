using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coutdownText;

    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameMAnager_OnStateChanged;
        Hide();
    }

    private void Update()
    {
        coutdownText.text = Mathf.Ceil(KitchenGameManager.Instance.CountdownToStartTimer()).ToString();
    }

    private void KitchenGameMAnager_OnStateChanged( object sender , System.EventArgs e )
    {
        if ( KitchenGameManager.Instance.IsCountdownToStartActive() )
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
    
    private void Show()
    {
        coutdownText.gameObject.SetActive( true );
    }

    private void Hide()
    {
        coutdownText.gameObject.SetActive( false );
    }
}
