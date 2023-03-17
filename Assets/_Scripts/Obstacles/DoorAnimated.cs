using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Interfaces;

public class DoorAnimated : MonoBehaviour, IDataPersistence
{
    private Animator doorAnim;
    public bool doorOpen = false;

    [SerializeField] private string id;

    [ContextMenu("Generate Guid for Item ID")]

    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void Awake()
    {
        doorAnim = transform.parent.GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        doorOpen = true;
        doorAnim.SetBool("open", doorOpen);
    }

    public void CloseDoor()
    {
        doorOpen = false;
        doorAnim.SetBool("open", doorOpen);
    }

    public void LoadData(GameData data)
    {
        data.doorState.TryGetValue(id, out doorOpen);
        if (doorOpen){
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    public void SaveData(GameData data)
    {
        if (data.doorState.ContainsKey(id))
        {
            data.doorState.Remove(id);
        }
        data.doorState.Add(id, doorOpen);
    }
}
