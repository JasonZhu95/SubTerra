using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.EventChannels;
using Project.Managers;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private InventoryPage inventoryUI;

    [SerializeField] private GameStateEventChannel GameStateEventChannel;

    public PlayerInputHandler InputHandler { get; private set; }

    public int inventorySize = 10;

    private void Start()
    {
        inventoryUI.InitializeInventoryUI(inventorySize);
    }

    private void Awake()
    {
        InputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        if (InputHandler.EscapePressed)
        {
            inventoryUI.Show();
            GameStateEventChannel.RaiseSetChangeGameStateEvent(this, new GameStateEventArgs(GameState.UI));
        }
        else
        {
            inventoryUI.Hide();
            GameStateEventChannel.RaiseSetChangeGameStateEvent(this, new GameStateEventArgs(GameState.Gameplay));
        }
    }

}
