﻿using UnityEngine;
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