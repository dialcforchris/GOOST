using UnityEngine;

namespace GOOST
{
    public class GameplayState : GameState
    {
        public override void OnStateActivate()
        {

        }

        public override void OnStateDeactivate()
        {

        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button9) || Input.GetKeyDown(KeyCode.Joystick1Button10))
                GameStateManager.instance.ChangeState(GameStates.STATE_PAUSE);
        }
    }
}