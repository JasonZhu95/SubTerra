using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Inventory.Data
{
    [CreateAssetMenu(fileName = "newItemParameterData", menuName = "Data/Item Data/Parameter")]
    public class ItemParameterSO : ScriptableObject
    {
        [field: SerializeField]
        public string ParameterName { get; private set; }
    }
}
