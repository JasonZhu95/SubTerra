using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.LevelSetup
{
    public class LevelLoaderManager : MonoBehaviour
    {
        public Animator transition;
        public float transitionTime = 1f;

        public void LoadNextLevel()
        {
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
        }

        IEnumerator LoadLevel(int levelIndex)
        {
            transition.SetTrigger("Start");

            yield return new WaitForSeconds(transitionTime);

            SceneManager.LoadScene(levelIndex);
        }

        public void PlayTransition()
        {
            transition.SetTrigger("Start");
        }
    }
}
