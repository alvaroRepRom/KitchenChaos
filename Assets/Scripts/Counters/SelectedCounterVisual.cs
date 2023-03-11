using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;

    private void Start()
    {
        //Player.Instance.OnSelectedCounterChanged += SelectedCounterChanged;
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
