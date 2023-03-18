using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButtonController : MonoBehaviour
{
    public int index;
    public int maxIndex;
    [SerializeField] bool keyDown;
    [SerializeField] RectTransform rectTransform;
    public bool isPressUp, isPressDown, isPressConfirm;
    public int VerticalMovement;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        isPressUp = isPressDown = false;
    }

    void Update()
    {
        if (isPressUp) VerticalMovement = 1;
        if (isPressDown) VerticalMovement = -1;
        if (!isPressUp && !isPressDown) VerticalMovement = 0;

        if (Input.GetAxis("Vertical") != 0 || VerticalMovement != 0)
        {
            if (!keyDown)
            {
                if (Input.GetAxis("Vertical") < 0 || VerticalMovement < 0)
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
                else if (Input.GetAxis("Vertical") > 0 || VerticalMovement > 0)
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

    public void onPressUp()
    {
        isPressUp = true;
    }

    public void onReleaseUp()
    {
        isPressUp = false;
    }

    public void onPressDown()
    {
        isPressDown = true;
    }

    public void onReleaseDown()
    {
        isPressDown = false;
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
