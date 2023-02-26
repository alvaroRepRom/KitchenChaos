using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    private const string NUMBER_POPUP = "NumberPopUp";

    [SerializeField] private TextMeshProUGUI coutdownText;

    private Animator animator;
    private int previousCoutdownNumber;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameMAnager_OnStateChanged;
        Hide();
    }

    private void Update()
    {
        int countdownNumber = Mathf.CeilToInt(KitchenGameManager.Instance.CountdownToStartTimer());
        coutdownText.text = countdownNumber.ToString();

        if ( previousCoutdownNumber != countdownNumber )
        {
            previousCoutdownNumber = countdownNumber;
            animator.SetTrigger( NUMBER_POPUP );
            SoundManager.Instance.PlayCoutdownSound();
        }
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
