using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoceBurnFlashingBarUI : MonoBehaviour
{
    private const string IS_FLASHING = "IsFlashing";

    [SerializeField] private StoveCounter stoveCounter;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;

        animator.SetBool( IS_FLASHING , false );
    }

    private void StoveCounter_OnProgressChanged( object sender , IHasProgress.OnProgressChangedEventsArgs e )
    {
        float showBurnProgressAmount = 0.5f;
        bool show =  stoveCounter.IsFryied() && e.progressNormalized >= showBurnProgressAmount;

        animator.SetBool( IS_FLASHING , show );
    }
}
