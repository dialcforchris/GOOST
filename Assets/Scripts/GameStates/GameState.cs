using UnityEngine;

namespace GOOST
{
    public abstract class GameState
    {
        public abstract void OnStateActivate();
        public abstract void OnStateDeactivate();
        public abstract void Update();
    }
}