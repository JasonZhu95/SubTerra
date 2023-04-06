using UnityEngine;
using UnityEngine.UI;
using System;

namespace Project.Inventory.UI
{
    public class ItemActionPanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject buttonPrefab;

        [HideInInspector]
        public GameObject buttonAction;

        public int CurrentButtonIndex { get; set; } = 0;

        public void AddButton(string name, Action onClickAction, bool firstAction = false)
        {
            buttonAction = Instantiate(buttonPrefab, transform);
            buttonAction.GetComponent<Button>().onClick.AddListener(() => onClickAction());
            buttonAction.GetComponentInChildren<TMPro.TMP_Text>().text = name;
            if (firstAction)
            {
                buttonAction.transform.GetChild(1).gameObject.SetActive(true);
            }
        }

        public void DeselectSelectedBorder(int index)
        {
            transform.GetChild(index).GetChild(1).gameObject.SetActive(false);
        }

        public void SetSelectedBorder(int index)
        {
            transform.GetChild(index).GetChild(1).gameObject.SetActive(true);
            FindObjectOfType<SoundManager>().Play("UIHover");
        }

        public void Toggle(bool val)
        {
            if (val == true)
                RemoveOldButtons();
            gameObject.SetActive(val);
        }

        public void RemoveOldButtons()
        {
            foreach (Transform transformChildObjects in transform)
            {
                Destroy(transformChildObjects.gameObject);
            }
        }
    }
}
