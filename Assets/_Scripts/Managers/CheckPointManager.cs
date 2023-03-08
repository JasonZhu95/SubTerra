using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Managers
{
    public class CheckPointManager : MonoBehaviour
    {
        private List<GameObject> checkPointList;

        public int LastSetCheckPointIndex { get; private set; }

        private void Awake()
        {
            InitializeList();
            SetCheckPoint(gameObject.transform.GetChild(0).gameObject);
        }

        private void InitializeList()
        {
            checkPointList = new List<GameObject>();
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                checkPointList.Add(gameObject.transform.GetChild(i).gameObject);
            }
        }

        public Vector3 GoToLastCheckPoint()
        {
            Vector3 checkPointPosition = new Vector3(checkPointList[LastSetCheckPointIndex].transform.position.x,
                checkPointList[LastSetCheckPointIndex].transform.position.y, checkPointList[LastSetCheckPointIndex].transform.position.z);
            return checkPointPosition;
        }

        public void SetCheckPoint(GameObject source)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                if (source.name == checkPointList[i].name)
                {
                    LastSetCheckPointIndex = i;
                    Debug.Log(source.name + " SET " + LastSetCheckPointIndex);
                }
            }
        }
    }
}
