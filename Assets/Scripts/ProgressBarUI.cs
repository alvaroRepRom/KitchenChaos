using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject hasProgressGameObject;
    [SerializeField] private Image barImage;

    private IHasProgress hasProgress;

    private void Start()
    {
        hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
        if ( hasProgress == null ) Debug.LogError( "The gameObject " + hasProgressGameObject.name + " has no IHasProgress" );

        hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;
        barImage.fillAmount = 0;

        Hide();
    }

    private void HasProgress_OnProgressChanged( object sender , IHasProgress.OnProgressChangedEventsArgs e )
    {
        barImage.fillAmount = e.progressNormalized;

        if ( e.progressNormalized == 0 | e.progressNormalized == 1 )
            Hide();
        else
            Show();
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
