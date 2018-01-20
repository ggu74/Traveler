using UnityEngine;

public class GoToUnitTask : Task
{
    public Transform TargetToFollow;
    private GoToTask goToTask;

    void OnEnable()
    {
        goToTask = GetComponent<GoToTask>();
    }

    void Update()
    {
        if (hasTargetPosChanged())
        {
            goToTask.SetDestination(TargetToFollow.position);
            goToTask.Resume();
        }
    }

    private bool hasTargetPosChanged()
    {
        return !(goToTask.Goal.x == TargetToFollow.position.x && goToTask.Goal.z == TargetToFollow.position.z);
    }

    public void StartTask(Transform unitToFollow)
    {
        base.StartTask();
            TargetToFollow = unitToFollow;
        goToTask.StartTask(unitToFollow.position, transform.localScale.magnitude);
    }

    protected override void OnDisable()
    {
        goToTask.Stop();
        base.OnDisable();
    }



}
