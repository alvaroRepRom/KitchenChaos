using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForPlayerUI : MonoBehaviour
{
    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        KitchenGameManager.Instance.OnLocalPlayerReadyChanged += KitchenGameManager_OnLocalPlayerReadyChanged;
        Hide();
    }

    private void KitchenGameManager_OnStateChanged( object sender , System.EventArgs e )
    {
        if ( KitchenGameManager.Instance.IsCountdownToStartActive() )
        {
            Hide();
        }
    }

    private void KitchenGameManager_OnLocalPlayerReadyChanged( object sender , System.EventArgs e )
    {
        if ( KitchenGameManager.Instance.IsPlayerLocalReady() )
        {
            Show();
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
