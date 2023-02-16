using UnityEngine;

public class ConteinerCounterVisual : MonoBehaviour
{
    [SerializeField] private ContainerCounter containerCounter;


    private string OPEN_CLOSE = "OpenClose";
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        containerCounter.OnPlayerGrabObject += ContainerCounter_OnPlayerGrabObject;
    }

    private void ContainerCounter_OnPlayerGrabObject( object sender , System.EventArgs e )
    {
        animator.SetTrigger( OPEN_CLOSE );
    }
}
