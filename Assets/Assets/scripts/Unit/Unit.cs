using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

    [System.NonSerialized]
    public Task[] AvailableTasks;
    public static List<Unit> PlayerUnits = new List<Unit>();
    public static List<Unit> EnemyUnits = new List<Unit>();

    void Start()
    {
        AvailableTasks = GetComponents<Task>();
        if (gameObject.layer == 8)//player layer
        {
            PlayerUnits.Add(this);
        }
        else if(gameObject.layer == 9)//enemy layer
        {
            EnemyUnits.Add(this);
        }
    }

    public T ChangeTask<T>() where T :Task
    {
        CencelAllTasks();
        for (int i = 0; i < AvailableTasks.Length; i++)
        {
            Task task = AvailableTasks[i];
            if (AvailableTasks[i] is T && !task.GetType().IsSubclassOf(typeof(T)))
            {
                task.enabled = true;
                return task as T;
            }
        }
        new System.Exception("Command not available for this unit");
        return null;
    }

    public void CencelAllTasks()
    {
        for (int i = 0; i < AvailableTasks.Length; i++)
        {
            if (AvailableTasks[i].enabled && !AvailableTasks[i].RunAllTheTime)
            {
                AvailableTasks[i].enabled = false;
            }
        }
    }

    public bool IsTaskAvailable<T>() where T : Task
    {
        for (int i = 0; i < AvailableTasks.Length; i++)
        {
            Task task = AvailableTasks[i];
            if (AvailableTasks[i] is T && !task.GetType().IsSubclassOf(typeof(T)))
            {
                return true;
            }
        }
        return false;
    }

}
