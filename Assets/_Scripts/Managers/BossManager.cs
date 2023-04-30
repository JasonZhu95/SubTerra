using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Interfaces;

public class BossManager : MonoBehaviour, IDataPersistence
{
    public bool RangerBossDefeated { get; set; } = false;
    public bool TempleBossDefeated { get; set; } = false;
    public bool DemonBossDefeated { get; set; } = false;

    public void LoadData(GameData data)
    {
        RangerBossDefeated = data.rangerBossDefeated;
        TempleBossDefeated = data.templeBossDefeated;
        DemonBossDefeated = data.demonBossDefeated;
    }

    public void SaveData(GameData data)
    {
        data.rangerBossDefeated = RangerBossDefeated;
        data.templeBossDefeated = TempleBossDefeated;
        data.demonBossDefeated = DemonBossDefeated;
    }
}
