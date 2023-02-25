using System;
using Project.Managers;
using UnityEngine;

namespace Project.EventChannels
{
    [CreateAssetMenu(fileName = "newGameStateEventChannel", menuName = "Event Channels/Game State")]
    public class GameStateEventChannel : ScriptableObject
    {
        public event EventHandler<GameStateEventArgs> OnBeforeStateChange;
        public event EventHandler<GameStateEventArgs> OnAfterStateChange;
        public event EventHandler<GameStateEventArgs> OnSetChangeGameState;

        public void RaiseBeforeStateChangeEvent(object sender, GameStateEventArgs context)
        {
            OnBeforeStateChange?.Invoke(sender, context);
        }

        public void RaiseAfterStateChangeEvent(object sender, GameStateEventArgs context)
        {
            OnAfterStateChange?.Invoke(sender, context);
        }

        public void RaiseSetChangeGameStateEvent(object sender, GameStateEventArgs context)
        {
            OnSetChangeGameState?.Invoke(sender, context);
        }
    }

    public class GameStateEventArgs : EventArgs
    {
        public GameState State;

        public GameStateEventArgs(GameState state)
        {
            State = state;
        }
    }
}