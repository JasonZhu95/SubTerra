using Project.EventChannels;
using System.Collections;
using UnityEngine;

// Script responsible for handling Time Scale Changes between UI.
namespace Project.Managers
{
    public class GameManager : MonoBehaviour
    {
        public GameState CurrentGameState { get; private set; }

        [SerializeField] private GameStateEventChannel channel;

        private void Awake()
        {
            channel.OnSetChangeGameState += ChangeGameState;
            ChangeGameState(this, new GameStateEventArgs(GameState.Gameplay));
        }

        private void ChangeGameState(object sender, GameStateEventArgs context)
        {
            if (context.State == CurrentGameState) return;
            
            channel.RaiseBeforeStateChangeEvent(this, new GameStateEventArgs(CurrentGameState));

            switch (context.State)
            {
                case GameState.UI:
                    HandleUIState();
                    break;
                case GameState.Gameplay:
                    HandleGameplayState();
                    break;
            }

            CurrentGameState = context.State;
            
            channel.RaiseAfterStateChangeEvent(this, new GameStateEventArgs(CurrentGameState));
        }

        private void HandleGameplayState()
        {
            Time.timeScale = 1f;
        }

        private void HandleUIState()
        {
            Time.timeScale = 0f;
        }

        private void OnDestroy()
        {
            channel.OnSetChangeGameState -= ChangeGameState;
        }
    }

    public enum GameState
    {
         UI,
         Gameplay,
    }
}