using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI keyMoveUpText;
    [SerializeField] private TextMeshProUGUI keyMoveDownText;
    [SerializeField] private TextMeshProUGUI keyMoveLeftText;
    [SerializeField] private TextMeshProUGUI keyMoveRightText;
    [SerializeField] private TextMeshProUGUI keyInteractText;
    [SerializeField] private TextMeshProUGUI keyInteractAlternateText;
    [SerializeField] private TextMeshProUGUI keyPauseText;
    [SerializeField] private TextMeshProUGUI gamepadInteractText;
    [SerializeField] private TextMeshProUGUI gamepadInteractAlternateText;
    [SerializeField] private TextMeshProUGUI gamepadPauseText;

    private void Start()
    {
        GameInput.Instance.OnBindingRebing += GameInput_OnBindingRebing;
        KitchenGameManager.Instance.OnLocalPlayerReadyChanged += KitchenGameManager_OnLocalPlayerReadyChanged;

        UpdateVisuals();
        Show();
    }

    private void KitchenGameManager_OnLocalPlayerReadyChanged( object sender , System.EventArgs e )
    {
        if ( KitchenGameManager.Instance.IsPlayerLocalReady() )
        {
            Hide();
        }
    }


    private void GameInput_OnBindingRebing( object sender , System.EventArgs e )
    {
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        keyMoveUpText.text = GameInput.Instance.GetBindingText( GameInput.Binding.Move_Up );
        keyMoveDownText.text = GameInput.Instance.GetBindingText( GameInput.Binding.Move_Down );
        keyMoveLeftText.text = GameInput.Instance.GetBindingText( GameInput.Binding.Move_Left );
        keyMoveRightText.text = GameInput.Instance.GetBindingText( GameInput.Binding.Move_Right );

        keyInteractText.text = GameInput.Instance.GetBindingText( GameInput.Binding.Interact );
        keyInteractAlternateText.text = GameInput.Instance.GetBindingText( GameInput.Binding.Interact_Alternate );
        keyPauseText.text = GameInput.Instance.GetBindingText( GameInput.Binding.Pause );

        gamepadInteractText.text = GameInput.Instance.GetBindingText( GameInput.Binding.Gamepad_Interact );
        gamepadInteractAlternateText.text = GameInput.Instance.GetBindingText( GameInput.Binding.Gamepad_Interact_Alternate );
        gamepadPauseText.text = GameInput.Instance.GetBindingText( GameInput.Binding.Gamepad_Pause );
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
