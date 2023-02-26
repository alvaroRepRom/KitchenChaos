using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{
    private const string POP_UP = "PopUp";

    [SerializeField] private Image background;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Color successColor;
    [SerializeField] private Color failedColor;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failedSprite;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;

        gameObject.SetActive( false );
    }

    private void DeliveryManager_OnRecipeFailed( object sender , System.EventArgs e )
    {
        gameObject.SetActive( true );
        animator.SetTrigger( POP_UP );
        background.color = failedColor;
        icon.sprite = failedSprite;
        messageText.text = "DELIVERY\nFAILED";
    }

    private void DeliveryManager_OnRecipeSuccess( object sender , System.EventArgs e )
    {
        gameObject.SetActive( true );
        animator.SetTrigger( POP_UP );
        background.color = successColor;
        icon.sprite = successSprite;
        messageText.text = "DELIVERY\nSUCCESS";
    }
}
