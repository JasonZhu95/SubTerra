using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButtonController : MonoBehaviour
{
    public int index;
    public int maxIndex;
    [SerializeField] bool keyDown;
    [SerializeField] RectTransform rectTransform;
    public bool isPressConfirm;
    public int VerticalMovement;

    private int yInput;

    private PlayerInputHandler inputHandler;

    void Start()
    {
        inputHandler = GameObject.FindWithTag("Player").GetComponent<PlayerInputHandler>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        yInput = inputHandler.NormMenuInputY;

        if (yInput == 1)
            VerticalMovement = 1;
        if (yInput == -1)
            VerticalMovement = -1;
        if (yInput != 1 && yInput != -1)
            VerticalMovement = 0;

        if (yInput != 0 || VerticalMovement != 0)
        {
            if (!keyDown)
            {
                if (yInput < 0 || VerticalMovement < 0)
                {
                    if (index < maxIndex)
                    {
                        index++;
                        if (index > 1 && index < maxIndex)
                        {
                            rectTransform.offsetMax -= new Vector2(0, -70);
                        }
                    }
                    else
                    {
                        index = 0;
                        rectTransform.offsetMax = Vector2.zero;
                    }
                }
                else if (yInput > 0 || VerticalMovement > 0)
                {
                    if (index > 0)
                    {
                        index--;
                        if (index < maxIndex - 1 && index > 0)
                        {
                            rectTransform.offsetMax -= new Vector2(0, 70);
                        }
                    }
                    else
                    {
                        index = maxIndex;
                        rectTransform.offsetMax = new Vector2(0, (maxIndex - 2) * 70);
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

    public void onPressConfirm()
    {
        isPressConfirm = true;
    }

    public void onReleaseConfirm()
    {
        isPressConfirm = false;
    }
}
