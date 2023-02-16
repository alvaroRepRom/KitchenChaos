using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{
    [SerializeField] private CuttingCounter cuttingCounter;


    private string CUT = "Cut";
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        cuttingCounter.OnCut += CuttingCounter_OnCut;
    }

    private void OnDisable()
    {
        cuttingCounter.OnCut -= CuttingCounter_OnCut;
    }

    private void CuttingCounter_OnCut( object sender , System.EventArgs e )
    {
        animator.SetTrigger( CUT );
    }
}
