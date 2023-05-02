using UnityEngine;
using Project.Interfaces;
using Project.Inventory;
using Project.UI;

public class ShopTrigger : MonoBehaviour, IDataPersistence
{
    private bool playerInRange;

    private GameObject playerObject;
    private CollisionSenses playerCollision;
    private PlayerInputHandler inputHandler;
    private IBuyItem customer;
    [SerializeField] private GameObject shopConfirmMenu;
    [SerializeField] private UI_Shop shopUI;
    [SerializeField] private DialogueTrigger dialogueTrigger;
    [SerializeField] private TextAsset JSONToChange;

    private DialogueManager dialogueManagerReference;

    private bool shopDialogueHasBeenPlayed = false;
    private bool dialogueSetFirstTime = false;

    private void Awake()
    {
        playerObject = GameObject.FindWithTag("Player");
        playerCollision = playerObject.GetComponent<Player>().Core.transform.GetChild(1).GetComponent<CollisionSenses>();
        inputHandler = playerObject.GetComponent<PlayerInputHandler>();
        customer = playerObject.GetComponent<InventoryController>().GetComponent<IBuyItem>();
        dialogueManagerReference = GameObject.FindWithTag("DialogueContainer").GetComponent<DialogueManager>();
    }

    private void Start()
    {
        if (dialogueSetFirstTime)
        {
            dialogueTrigger.ChangeDialogueFile(JSONToChange);
        }
    }

    private void OnEnable()
    {
        dialogueManagerReference.OnDialogueFinish += SetShopDialoguePlayedTrue;
    }

    private void OnDisable()
    {
        dialogueManagerReference.OnDialogueFinish -= SetShopDialoguePlayedTrue;
    }

    private void Update()
    {
        if (shopDialogueHasBeenPlayed && !shopUI.isActiveAndEnabled && dialogueTrigger.playerInRange)
        {
            inputHandler.BlockActionInput = true;
            shopUI.Show(customer);
            inputHandler.SwitchToActionMap("UINoPause");
            SetShopDialoguePlayedFalse();
        }
        else if(inputHandler.BackActionUIInput && shopUI.isActiveAndEnabled && !shopConfirmMenu.activeSelf)
        {
            inputHandler.BlockActionInput = false;
            inputHandler.BackActionUIInput = false;
            shopUI.Hide();
            inputHandler.SwitchToActionMap("Gameplay");
        }
    }

    private void SetShopDialoguePlayedTrue()
    {
        shopDialogueHasBeenPlayed = true;
        if (!dialogueSetFirstTime)
        {
            dialogueSetFirstTime = true;
            dialogueTrigger.ChangeDialogueFile(JSONToChange);
        }
    }
    private void SetShopDialoguePlayedFalse() => shopDialogueHasBeenPlayed = false;

    public void LoadData(GameData data)
    {
        dialogueSetFirstTime = data.shopNPCTalkedTo;
    }

    public void SaveData(GameData data)
    {
        data.shopNPCTalkedTo = dialogueSetFirstTime;
    }
}
