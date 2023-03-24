using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterTime : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(DisableAfterTimeOnAwake());
    }

    private IEnumerator DisableAfterTimeOnAwake()
    {
        yield return new WaitForSeconds(0.05f);

        gameObject.SetActive(false);
    }
}
