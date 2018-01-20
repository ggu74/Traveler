using UnityEngine;
using System.Collections.Generic;

public class GoToTask : Task
{

    static Dictionary<Vector3, int> unitsPerDestination = new Dictionary<Vector3, int>();
    public float StoppingDistance;
    protected UnityEngine.AI.NavMeshAgent Agent;
    protected const float parkingDistance = 3;
    private float minStoppingDistance;
    private Vector3 goal;

    public Vector3 Goal
    {
        get
        {
            return goal;
        }
    }

    void OnEnable()
    {
        Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - Agent.destination).magnitude < StoppingDistance + minStoppingDistance)
        {
            enabled = false;
        }
    }

    public void StartTask(Vector3 destination,float minStoppingDistance=0)
    {
        base.StartTask();
        this.minStoppingDistance = minStoppingDistance;
        SetDestination(destination);
        Resume();
    }

    public void SetDestination(Vector3 destination)
    {
        if (unitsPerDestination.ContainsKey(goal))
        {
            unitsPerDestination[goal] -= 1;
            if (unitsPerDestination[goal] == 0)
            {
                unitsPerDestination.Remove(goal);
            }
        }
        if (!unitsPerDestination.ContainsKey(destination))
        {
            unitsPerDestination.Add(destination, 0);
        }
        unitsPerDestination[destination] = unitsPerDestination[destination] + 1;
        StoppingDistance = Mathf.Sqrt((Agent.radius * Agent.radius + parkingDistance) * unitsPerDestination[destination]);
        goal = destination;
        Agent.destination = goal;
        Agent.avoidancePriority = Mathf.Min(99, Mathf.RoundToInt(Mathf.Sqrt(unitsPerDestination[destination])));
    }

    public void Stop()
    {
        if (Agent.isActiveAndEnabled)
        {
            Agent.Stop();
            Agent.ResetPath();
        }
    }
    public virtual void Resume()
    {
        Agent.Resume();
        if(!isActiveAndEnabled)
        {
            enabled = true;
        }
    }

    protected override void OnDisable()
    {
        Stop();
    }

}
