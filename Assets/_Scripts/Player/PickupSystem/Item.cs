using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Inventory.Data;

public class Item : MonoBehaviour, IDataPersistence
{
    [field: SerializeField]
    public ItemSO InventoryItem { get; private set; }

    [field: SerializeField]
    public int Quantity { get; set; } = 1;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private float duration = 0.3f;

    private bool collected = false;

    [SerializeField] private string id;

    [ContextMenu("Generate Guid for Item ID")]

    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = InventoryItem.ItemImage;
        if (collected)
        {
            gameObject.SetActive(false);
        }
    }

    internal void DestroyItem()
    {
        if (!collected)
        {
            GetComponent<Collider2D>().enabled = false;
            collected = true;
            StartCoroutine(AnimateItemPickup());
        }
    }

    private IEnumerator AnimateItemPickup()
    {
        audioSource.Play();
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        float currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, currentTime / duration);
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public void LoadData(GameData data)
    {
        data.itemsCollected.TryGetValue(id, out collected);
        if (collected)
        {
            gameObject.SetActive(false);
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.itemsCollected.ContainsKey(id))
        {
            data.itemsCollected.Remove(id);
        }
        data.itemsCollected.Add(id, collected);
    }
}
