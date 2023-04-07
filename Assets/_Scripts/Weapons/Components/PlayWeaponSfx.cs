using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project. Weapons
{
    public class PlayWeaponSfx : WeaponComponent<PlayWeaponSfxData>
    {
        private SfxData sfxData;

        protected override void SetCurrentAttackData()
        {
            base.SetCurrentAttackData();
            sfxData = data.GetAttackData(counter);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            eventHandler.OnSfxEnable += PlayAudio;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            eventHandler.OnSfxEnable -= PlayAudio;
        }

        private void PlayAudio()
        {
            FindObjectOfType<SoundManager>().Play(sfxData.AudioFileToPlay);
        }
    }

    [System.Serializable]
    public class PlayWeaponSfxData : WeaponComponentData<SfxData>
    {
        public PlayWeaponSfxData()
        {
            ComponentDependencies.Add(typeof(PlayWeaponSfx));
        }
    }

    [System.Serializable]
    public struct SfxData : INameable
    {
        [HideInInspector] public string AttackName;

        [field: SerializeField] public string AudioFileToPlay { get; private set; }

        public void SetName(string value) => AttackName = value;
    }
}
