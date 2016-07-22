using UnityEngine;

public enum GameStates
{
    STATE_GAMEPLAY = 0,
    STATE_MENU,
    STATE_PAUSE,
    STATE_GAMEOVER,
    STATE_TRANSITIONING,
    STATE_READYUP,
    GAMESTATES_COUNT
}

public class GameStateManager : MonoBehaviour
{
    //public bool pausePressed;
    public GameObject PauseMenu;
    private static GameStateManager singleton = null;
    public static GameStateManager instance { get { return singleton; } }

    private GameState[] states = new GameState[(int)GameStates.GAMESTATES_COUNT];
    [SerializeField]
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
            states[(int)GameStates.STATE_MENU] = new MenuState();
            states[(int)GameStates.STATE_PAUSE] = new PauseState();
            states[(int)GameStates.STATE_GAMEOVER] = new GameOverState();
            states[(int)GameStates.STATE_TRANSITIONING] = new TransitioningState();
            states[(int)GameStates.STATE_READYUP] = new ReadyUpState();
            ChangeState(GameStates.STATE_MENU);
        }
        
    }

    private void Update()
    {
        states[(int)currentState].Update();
    }

    public void unPause()
    {
        ChangeState(previousState);
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
