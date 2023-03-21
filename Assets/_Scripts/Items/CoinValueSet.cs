using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinValueSet : MonoBehaviour
{
    [SerializeField]
    private RuntimeAnimatorController[] coinValueAnim;

    private Animator coinAnim;
    private Rigidbody2D coinRB;

    public bool CanCollect { get; private set; }

    public int CoinValue { get; private set; }

    private void Awake()
    {
        coinAnim = gameObject.GetComponent<Animator>();
        coinRB = gameObject.GetComponent<Rigidbody2D>();
        Vector3 impulse = new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(3.0f, 6.0f), 0f);
        coinRB.AddForce(impulse, ForceMode2D.Impulse);
        StartCoroutine(EnableCoinCollision());
        CanCollect = false;
    }

    public void SetCoinValue(int value)
    {
        CoinValue = value;
        SetCoinAnim();
    }
    
    private void SetCoinAnim()
    {
        if (CoinValue == 1)
        {
            coinAnim.runtimeAnimatorController = coinValueAnim[0];
        }
        if (CoinValue == 5)
        {
            coinAnim.runtimeAnimatorController = coinValueAnim[1];
        }
        if (CoinValue == 10)
        {
            coinAnim.runtimeAnimatorController = coinValueAnim[2];
        }
    }

    private IEnumerator EnableCoinCollision()
    {
        yield return new WaitForSeconds(.3f);
        CanCollect = true;
    }
}
