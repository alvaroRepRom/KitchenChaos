using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;

    private void Start()
    {
        if ( Player.LocalInstance != null )
            Player.LocalInstance.OnSelectedCounterChanged += SelectedCounterChanged;
        else
            Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
    }

    private void Player_OnAnyPlayerSpawned( object sender , System.EventArgs e )
    {
        if ( Player.LocalInstance != null )
        {
            Player.LocalInstance.OnSelectedCounterChanged -= SelectedCounterChanged;
            Player.LocalInstance.OnSelectedCounterChanged += SelectedCounterChanged;

        }
    }

    private void SelectedCounterChanged( object sender , Player.OnSelectedCounterChangedEventsArgs e )
    {
        if ( e.selectedCounter == baseCounter )
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
        foreach ( GameObject item in visualGameObjectArray )
            item.SetActive( true );
    }
    
    private void Hide()
    {
        foreach ( GameObject item in visualGameObjectArray )
            item.SetActive( false );
    }
}
