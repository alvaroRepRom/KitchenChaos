using System;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour
{
    public static KitchenGameManager Instance { get; private set; }


    public event EventHandler OnStateChanged;


    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    private State state;
    private float waitToStartTimer = 1f;
    private float countdownToStartTimer = 3f;
    private float gamePlayingToStartTimer;
    private float gamePlayingToStartTimerMax = 10f;

    private void Awake()
    {
        Instance = this;
        state = State.WaitingToStart;
    }

    private void Update()
    {
        switch ( state )
        {
            case State.WaitingToStart:
                waitToStartTimer -= Time.deltaTime;
                if ( waitToStartTimer < 0 )
                {
                    state = State.CountdownToStart;
                    OnStateChanged?.Invoke( this , EventArgs.Empty );
                }
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if ( countdownToStartTimer < 0 )
                {
                    state = State.GamePlaying;
                    gamePlayingToStartTimer = gamePlayingToStartTimerMax;
                    OnStateChanged?.Invoke( this , EventArgs.Empty );
                }
                break;
            case State.GamePlaying:
                gamePlayingToStartTimer -= Time.deltaTime;
                if ( gamePlayingToStartTimer < 0 )
                {
                    state = State.GameOver;
                    OnStateChanged?.Invoke( this , EventArgs.Empty );
                }
                break;
            case State.GameOver:
                break;
        }
    }


    public bool IsGamePlaying() => state == State.GamePlaying;
    public bool IsCountdownToStartActive() => state == State.CountdownToStart;
    public float CountdownToStartTimer() => countdownToStartTimer;
    public bool IsGameOver() => state == State.GameOver;
    public float GetGamePlayingTimeNormalized() => 1 - gamePlayingToStartTimer / gamePlayingToStartTimerMax;
}
