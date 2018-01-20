using UnityEngine;

public class AttackInTheArea : Task
{
    public float StartAttackRange;
    private AttackTask attackTask;

    void OnEnable()
    {
        attackTask = GetComponent<AttackTask>();
        RunAllTheTime = true;
    }

    void Update()
    {
        if (!attackTask.IsAttacking)
        {
            if (gameObject.layer == 8)//player layer
            {
                for (int i = 0; i < Unit.EnemyUnits.Count; i++)
                {
                    if (StartAttackRange > Vector3.Distance(Unit.EnemyUnits[i].transform.position, transform.position))
                    {
                        attackTask.StartTask(Unit.EnemyUnits[i].transform);
                        break;
                    }
                }
            }
            else if (gameObject.layer == 9)//enemy layer
            {
                for (int i = 0; i < Unit.PlayerUnits.Count; i++)
                {
                    if (StartAttackRange > Vector3.Distance(Unit.PlayerUnits[i].transform.position, transform.position))
                    {
                        attackTask.StartTask(Unit.PlayerUnits[i].transform);
                        break;
                    }
                }
            }
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }



}
