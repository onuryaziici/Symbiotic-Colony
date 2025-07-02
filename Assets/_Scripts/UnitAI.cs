using UnityEngine;
using UnityEngine.AI;

public class UnitAI : MonoBehaviour
{
    [Header("Base Stats")]
    public int baseDamage = 5;
    public float baseAttackRate = 1f;
    public float baseAttackRange = 2f;
    
    [Header("Setup")]
    public string enemyTag = "Enemy";

    // Bonuslarla hesaplanmış güncel statlar
    private int currentDamage;
    private float currentAttackRate;
    private float currentAttackRange;

    // Özel değişkenler
    private Transform target;
    private NavMeshAgent agent;
    private float attackCountdown = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ApplyBonuses();

        // NavMeshAgent'ın durma mesafesini güncel stat'a göre ayarla
        if (agent != null)
        {
            agent.stoppingDistance = currentAttackRange;
        }

        // Basit hedefleme: İlk bulduğun düşmana kilitlen
        GameObject initialTarget = GameObject.FindGameObjectWithTag(enemyTag);
        if (initialTarget != null)
        {
            target = initialTarget.transform;
        }
    }

    void ApplyBonuses()
    {
        // Başlangıçta güncel statları temel statlara eşitle
        currentDamage = baseDamage;
        currentAttackRate = baseAttackRate;
        currentAttackRange = baseAttackRange;
        
        EvolutionManager evoManager = FindObjectOfType<EvolutionManager>();
        if (evoManager != null)
        {
            // Hasar bonusunu al ve uygula
            float damageBonus = evoManager.GetStatBonus("ClawCell_Damage");
            currentDamage = (int)(baseDamage * (1f + damageBonus));

            // Saldırı hızı bonusunu al ve uygula
            float attackRateBonus = evoManager.GetStatBonus("ClawCell_AttackRate");
            currentAttackRate *= (1f + attackRateBonus);

            // Menzil bonusunu al ve uygula
            float attackRangeBonus = evoManager.GetStatBonus("ClawCell_AttackRange");
            currentAttackRange *= (1f + attackRangeBonus);
        }
    }

    void Update()
    {
        if (target == null)
        {
            if(agent.hasPath) agent.ResetPath();
            return;
        }

        agent.SetDestination(target.position);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (attackCountdown <= 0f)
            {
                FaceTarget();
                Attack();
                attackCountdown = 1f / currentAttackRate; // Zamanlayıcıyı güncel stat ile sıfırla
            }
        }

        attackCountdown -= Time.deltaTime;
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void Attack()
    {
        EnemyHealth enemy = target.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(currentDamage); // Hasarı güncel stat ile ver
        }
    }
}