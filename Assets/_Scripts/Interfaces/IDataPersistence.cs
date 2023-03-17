using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Interfaces
{
    public interface IDataPersistence
    {
        void LoadData(GameData data);

        void SaveData(GameData data);
    }
}
