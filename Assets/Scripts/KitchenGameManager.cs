using System;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour
{
    public static KitchenGameManager Instance { get; private set; }


    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnPaused;


    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    private State state;
    private float countdownToStartTimer = 3f;
    private float gamePlayingToStartTimer;
    private float gamePlayingToStartTimerMax = 30f;
    private bool isGamePaused = false;

    private void Awake()
    {
        Instance = this;
        state = State.WaitingToStart;
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction( object sender , EventArgs e )
    {
        if ( state == State.WaitingToStart )
        {
            state = State.CountdownToStart;
            OnStateChanged?.Invoke( this , EventArgs.Empty );
        }
    }

    private void GameInput_OnPauseAction( object sender , EventArgs e )
    {
        TogglePauseGame();
    }

    private void Update()
    {
        switch ( state )
        {
            case State.WaitingToStart:
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


    public void TogglePauseGame()
    {
        isGamePaused = !isGamePaused;
        if ( isGamePaused )
        {
            Time.timeScale = 0;
            OnGamePaused?.Invoke( this , EventArgs.Empty );
        }
        else
        {
            Time.timeScale = 1;
            OnGameUnPaused?.Invoke( this , EventArgs.Empty );
        }
    }
}
