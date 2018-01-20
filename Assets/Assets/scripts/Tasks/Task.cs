using UnityEngine;
using System.Collections;

public class Task:MonoBehaviour {

    public bool RunAllTheTime=false;

    protected virtual void OnDisable()
    {

    }

    protected  virtual void StartTask()
    {
        enabled = true;
    }
}
