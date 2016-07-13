using UnityEngine;
using System.Collections;

public class GameOverState : GameState
{
    public override void OnStateActivate()
    {
        SoundManager.instance.music.volume = 0;
    }

    public override void OnStateDeactivate()
    {

    }

    public override void Update()
    {
    }
}

public class SplashScreenState : GameState
{
    public override void OnStateActivate()
    {
    }

    public override void OnStateDeactivate()
    {

    }

    public override void Update()
    {
    }
}
