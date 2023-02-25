using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct OptionalAttachmentPointStruct : INameable
{
    [HideInInspector]
    public string AttackName;
    public Sprite Sprite;
    public bool UseSprite;

    public void SetName(string value)
    {
        AttackName = value;
    }
}
