using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace GOOST
{
    public class GameOverState : GameState
    {
        public override void OnStateActivate()
        {
            //   SoundManager.instance.music.volume = 0;
            //GameStats.instance.ShowStats();
        }

        public override void OnStateDeactivate()
        {

        }

        public override void Update()
        {
        }
    }
}