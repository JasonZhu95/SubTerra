using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeExplicitNavigationUI : MonoBehaviour
{
    private Button button;
    [SerializeField]
    private Button buttonToSelect;

    private void Awake()
    {
        button = gameObject.GetComponent<Button>();
    }

    private void Start()
    {
        Navigation nav = button.navigation;

        if(!button.navigation.selectOnDown.gameObject.activeSelf)
        {
            nav.selectOnDown = buttonToSelect;
        }

        button.navigation = nav;
    }
}
