using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button sfxButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAlternateButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI sfxText;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAlternateText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private Transform pressToRebingTrans;

    private void Awake()
    {
        Instance = this;

        sfxButton.onClick.AddListener( () =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisuals();

        });
        musicButton.onClick.AddListener( () => 
        {
            MusicManager.Instance.ChangeVolume();
            UpdateVisuals();
        } );

        closeButton.onClick.AddListener( () =>
        {
            Hide();
        } );
        
        moveUpButton.onClick.AddListener( () => { RebindKey( GameInput.Binding.Move_Up ); } );
        moveDownButton.onClick.AddListener( () => { RebindKey( GameInput.Binding.Move_Down ); } );
        moveLeftButton.onClick.AddListener( () => { RebindKey( GameInput.Binding.Move_Left ); } );
        moveRightButton.onClick.AddListener( () => { RebindKey( GameInput.Binding.Move_Right ); } );
        interactButton.onClick.AddListener( () => { RebindKey( GameInput.Binding.Interact ); } );
        interactAlternateButton.onClick.AddListener( () => { RebindKey( GameInput.Binding.Interact_Alternate ); } );
        pauseButton.onClick.AddListener( () => { RebindKey( GameInput.Binding.Pause ); } );
    }

    private void Start()
    {
        KitchenGameManager.Instance.OnGameUnPaused += KitchenGameManager_OnGameUnPaused;

        UpdateVisuals();
        HidePressToRibingKey();
        Hide();
    }

    private void KitchenGameManager_OnGameUnPaused( object sender , System.EventArgs e )
    {
        Hide();
    }

    private void UpdateVisuals()
    {
        sfxText.text = "Sound Effects: " + Mathf.Round( SoundManager.Instance.GetVolume() * 10f );
        musicText.text = "Music: " + Mathf.Round( MusicManager.Instance.GetVolume() * 10f );

        moveUpText.text = GameInput.Instance.GetBindingText( GameInput.Binding.Move_Up );
        moveDownText.text = GameInput.Instance.GetBindingText( GameInput.Binding.Move_Down );
        moveLeftText.text = GameInput.Instance.GetBindingText( GameInput.Binding.Move_Left );
        moveRightText.text = GameInput.Instance.GetBindingText( GameInput.Binding.Move_Right );
        interactText.text = GameInput.Instance.GetBindingText( GameInput.Binding.Interact );
        interactAlternateText.text = GameInput.Instance.GetBindingText( GameInput.Binding.Interact_Alternate );
        pauseText.text = GameInput.Instance.GetBindingText( GameInput.Binding.Pause );
    }

    public void Show()
    {
        gameObject.SetActive( true );
    }

    private void Hide()
    {
        gameObject.SetActive( false );
    }

    private void ShowPressToRibingKey()
    {
        pressToRebingTrans.gameObject.SetActive( true );
    }
    
    private void HidePressToRibingKey()
    {
        pressToRebingTrans.gameObject.SetActive( false );
    }

    private void RebindKey( GameInput.Binding binding )
    {
        ShowPressToRibingKey();
        GameInput.Instance.RebindBinding( binding, () =>
        {
            HidePressToRibingKey();
            UpdateVisuals();
        } );
    }
}
