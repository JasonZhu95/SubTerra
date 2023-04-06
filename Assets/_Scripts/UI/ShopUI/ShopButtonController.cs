using UnityEngine;
using UnityEngine.UI;

public class ShopButtonController : MonoBehaviour
{
    public int index;
    public int maxIndex;
    private int yInput;

    public int buttonOffsetHeight = 70;

    private PlayerInputHandler inputHandler;

    private bool keyDown;
    [SerializeField] RectTransform rectTransform;
    private Vector2 startingOffset;
    [SerializeField] private bool confirmMenu;
    [SerializeField] private GameObject confirmMenuObject;
    [SerializeField] public GameObject shopVerticalLayoutObject;

    void Start()
    {
        inputHandler = GameObject.FindWithTag("Player").GetComponent<PlayerInputHandler>();
        rectTransform = GetComponent<RectTransform>();
        startingOffset = rectTransform.offsetMax;
    }

    void Update()
    {
        yInput = inputHandler.NormMenuInputY;

        // Check menu navigation on Back
        if (confirmMenuObject.activeSelf)
        {
            if (inputHandler.BackActionUIInput)
            {
                shopVerticalLayoutObject.SetActive(true);
                confirmMenuObject.GetComponent<Animator>().SetBool("start", false);
                inputHandler.BackActionUIInput = false;
            }
        }

        // Check Menu Navigation arrow keys
        if (yInput != 0)
        {
            if (!keyDown)
            {
                FindObjectOfType<SoundManager>().Play("UIHover");
                if (yInput < 0)
                {
                    if (index < maxIndex)
                    {
                        index++;
                        if (index > 1 && index < maxIndex)
                        {
                            rectTransform.offsetMax -= new Vector2(0, -buttonOffsetHeight);
                        }
                    }
                    else
                    {
                        index = 0;
                        rectTransform.offsetMax = startingOffset;
                    }
                }
                else if (yInput > 0)
                {
                    if (index > 0)
                    {
                        index--;
                        if (index < maxIndex - 1 && index > 0)
                        {
                            rectTransform.offsetMax -= new Vector2(0, buttonOffsetHeight);
                        }
                    }
                    else
                    {
                        index = maxIndex;
                        if (confirmMenu)
                        {
                            rectTransform.offsetMax = startingOffset;
                        }
                        else
                        {
                            rectTransform.offsetMax = new Vector2(0, (maxIndex - 2) * buttonOffsetHeight);
                        }
                    }
                }
                keyDown = true;
            }
        }
        else
        {
            keyDown = false;
        }
    }
}
