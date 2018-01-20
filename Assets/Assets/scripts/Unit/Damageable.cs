using UnityEngine;

public class Damageable : MonoBehaviour
{
    public GameObject HealthBar;
    public float MaxLife;
    public float Defence;
    public delegate void ReceiveDamageEventHandler(Unit attackingUnit);
    public event ReceiveDamageEventHandler ReceivedDamage;
    [SerializeField]
    private float life;

    void Awake()
    {
        GameObject healthBar = Instantiate<GameObject>(HealthBar);
        healthBar.transform.SetParent(transform);
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        float sizeY = 0;
        for (int i = 0; i < renderers.Length; i++)
        {
            sizeY += renderers[i].bounds.extents.y;
        }
        healthBar.transform.localPosition = Vector3.up * sizeY * 2;
        Life = MaxLife;
    }

    public float Life
    {
        get
        {
            return life;
        }

        internal set
        {
            life = value;
            if (life <= 0)
            {
                Unit unit = GetComponent<Unit>();
                int index = Unit.PlayerUnits.IndexOf(unit);
                if (index == -1)
                {
                    index = Unit.EnemyUnits.IndexOf(unit);
                    if (index != -1)
                    {
                        Unit.EnemyUnits.RemoveAt(index);
                    }
                }
                else
                {
                    Unit.PlayerUnits.RemoveAt(index);
                }
                GameObject.Destroy(gameObject);
            }
        }
    }

    public void ReceiveDamage(float force, AttackStats attackStats)
    {
        if (ReceivedDamage != null)
        {
            Unit attackingUnit = attackStats.GetComponentInParent<Unit>();
            ReceivedDamage(attackingUnit);
        }
        Life -= force * attackStats.Strength - Defence;
    }

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            Life = life;
        }
    }


    // Update is called once per frame
    //void Update()
    //{

    //}
}
