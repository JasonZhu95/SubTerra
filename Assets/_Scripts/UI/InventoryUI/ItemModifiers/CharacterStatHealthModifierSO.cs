using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newModifierData", menuName = "Data/Modifier Data/Health")]
public class CharacterStatHealthModifierSO : CharacterStatModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        Stats stats = character.transform.Find("Core").transform.Find("Stats").gameObject.GetComponent<Stats>();
        if (stats != null)
        {
            stats.Health.Increase((int)val);
        }
    }
}
