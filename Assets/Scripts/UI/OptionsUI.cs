using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button sfxButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI sfxText;
    [SerializeField] private TextMeshProUGUI musicText;

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
    }

    private void Start()
    {
        KitchenGameManager.Instance.OnGameUnPaused += KitchenGameManager_OnGameUnPaused;

        UpdateVisuals();
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
    }

    public void Show()
    {
        gameObject.SetActive( true );
    }

    private void Hide()
    {
        gameObject.SetActive( false );
    }
}
