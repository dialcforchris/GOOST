using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverState : GameState
{
    public override void OnStateActivate()
    {
        //   SoundManager.instance.music.volume = 0;
        //EndGameLogic.instance.EndGame();
        GameStats.instance.ShowStats();
    }

    public override void OnStateDeactivate()
    {

    }

    public override void Update()
    {
    }
}