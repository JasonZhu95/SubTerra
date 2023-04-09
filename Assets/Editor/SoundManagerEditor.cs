using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor
{
    private Sound[] _sounds;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SoundManager soundManager = (SoundManager)target;

        _sounds = soundManager.sounds.ToArray();

        if (GUILayout.Button("Sort Array"))
        {
            foreach (var obj in targets)
            {
                SoundManager soundComponent = (SoundManager)obj;

                List<Sound> tempList = soundComponent.sounds.ToList();

                tempList = tempList.OrderBy(x => x.name).ToList();

                soundComponent.sounds = tempList.ToArray();

                EditorUtility.SetDirty(soundComponent);

            }

        }

        if (!soundManager.sounds.SequenceEqual(_sounds))
        {
            foreach (var obj in targets)
            {
                SoundManager soundComponent = (SoundManager)obj;
                EditorUtility.SetDirty(soundComponent);
            }
        }
    }
}