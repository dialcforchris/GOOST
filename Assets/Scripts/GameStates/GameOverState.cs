using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverState : GameState
{
    public override void OnStateActivate()
    {
       // SoundManager.instance.music.volume = 0;
        Debug.Log("end");
        LeaderBoard.instance.EndGame();
       // SceneManager.LoadScene(0);
    }

    public override void OnStateDeactivate()
    {

    }

    public override void Update()
    {
    }
}