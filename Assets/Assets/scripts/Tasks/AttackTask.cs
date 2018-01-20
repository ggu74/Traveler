using UnityEngine;

public class AttackTask : Task
{
    public bool IsAttacking;
    [SerializeField]
    private Transform unitToAttack;
    private Damageable unitDamageable;
    private AttackStats attackStats;
    private GoToUnitTask goToUnitTask;
    private float lastAttackTime;

    public Transform UnitToAttack
    {
        get
        {
            return unitToAttack;
        }

        set
        {
            unitToAttack = value;
            if (unitToAttack)
            {
                unitDamageable = unitToAttack.GetComponent<Damageable>();
            }
            else
            {
                unitDamageable = null;
                goToUnitTask.enabled = false;
            }
        }
    }

    void OnEnable()
    {
        attackStats = GetComponent<AttackStats>();
        goToUnitTask = GetComponent<GoToUnitTask>();
        Damageable damageable= GetComponent<Damageable>();
        damageable.ReceivedDamage += new Damageable.ReceiveDamageEventHandler(OnReceivedDamage);
    }

    void OnReceivedDamage(Unit attackingUnit)
    {
        if(!IsAttacking)
        {
            StartTask(attackingUnit.transform);
        }
    }


    //void OnEnable()
    //{
    //    GoToUnitTask goToUnitTask = GetComponent<GoToUnitTask>();
    //}

    public void StartTask(Transform unitToAttack)
    {
        base.StartTask();
        UnitToAttack = unitToAttack;
        goToUnitTask.StartTask(UnitToAttack);
    }

    void Update()
    {
        if (unitToAttack != null)
        {
            if (goToUnitTask.isActiveAndEnabled)
            {
                if ((transform.position - unitToAttack.position).magnitude < transform.localScale.magnitude + attackStats.Range)
                {
                    goToUnitTask.enabled = false;
                    lastAttackTime = Time.fixedDeltaTime;
                    IsAttacking = true;
                }
            }
            else
            {
                if ((transform.position - unitToAttack.position).magnitude > transform.localScale.magnitude + attackStats.Range)
                {
                    IsAttacking = false;
                    goToUnitTask.StartTask(unitToAttack);
                }
            }
        }
        else
        {
            OnAttackEnd();
        }
    }
    void FixedUpdate()
    {
        if (IsAttacking && (Time.fixedTime - lastAttackTime) > attackStats.AttackInterval)
        {
            lastAttackTime = Time.fixedTime;
            unitDamageable.ReceiveDamage(5f, attackStats);
        }
    }

    private void OnAttackEnd()
    {
        UnitToAttack = null;
        enabled = false;
    }
    protected override void OnDisable()
    {
        IsAttacking = false;
        base.OnDisable();
    }
}
