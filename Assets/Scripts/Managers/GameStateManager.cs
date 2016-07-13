﻿using UnityEngine;

public enum GameStates
{
    STATE_GAMEPLAY = 0,
    STATE_PAUSE,
    STATE_GAMEOVER,
    STATE_SPLASH,
    GAMESTATES_COUNT
}

public class GameStateManager : MonoBehaviour
{
    //public bool pausePressed;
    public GameObject PauseMenu;
    private static GameStateManager singleton = null;
    public static GameStateManager instance { get { return singleton; } }

    private GameState[] states = new GameState[(int)GameStates.GAMESTATES_COUNT];
    private GameStates currentState;
    public GameStates previousState;

    private void Awake()
    {
        if (singleton)
        {
            DestroyImmediate(this);
        }
        else
        {
            singleton = this;
            states[(int)GameStates.STATE_GAMEPLAY] = new GameplayState();
            states[(int)GameStates.STATE_PAUSE] = new PauseState();
            states[(int)GameStates.STATE_GAMEOVER] = new GameOverState();
            states[(int)GameStates.STATE_SPLASH] = new SplashScreenState();
            ChangeState(GameStates.STATE_GAMEPLAY);
        }
        
    }

    private void Update()
    {
        states[(int)currentState].Update();
    }

	public void ChangeState(GameStates _state)
    {
        previousState = currentState;
        states[(int)currentState].OnStateDeactivate();
        currentState = _state;
        states[(int)currentState].OnStateActivate();
    }

    public GameStates GetState()
    {
        return currentState;
    }


}