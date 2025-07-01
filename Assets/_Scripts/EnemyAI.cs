// EnemyAI.cs
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))] // Bu script'in eklendiği objede NavMeshAgent olmasını zorunlu kıl
[RequireComponent(typeof(EnemyHealth))]  // ve EnemyHealth olmasını da zorunlu kıl
public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public int damage = 10;           // Çekirdeğe vereceği hasar
    public float attackRate = 1f;     // Saniyede kaç kez saldıracağı
    public float attackRange = 2.5f;  // Çekirdeğe ne kadar yaklaşınca saldıracağı

    private Transform targetCore;         // Hedefimiz olan BioCore
    private NavMeshAgent agent;
    private PlayerHealth playerHealth;    // Hedefin sağlık bileşeni
    private float attackCountdown = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // Oyundaki BioCore'u bul ve hedef olarak belirle
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player"); 
        if (playerObject != null)
        {
            targetCore = playerObject.transform;
            playerHealth = playerObject.GetComponent<PlayerHealth>();
        }

        // NavMeshAgent'a, hedefe saldırı menzili kadar kala durmasını söyle
        agent.stoppingDistance = attackRange;
    }

    void Update()
    {
        // Eğer hedef yoksa (oyun bittiyse vb.) veya hedefin canı kalmadıysa dur
        if (targetCore == null || playerHealth == null || playerHealth.currentHealth <= 0)
        {
            agent.ResetPath();
            return;
        }

        // Sürekli olarak hedefe doğru git
        agent.SetDestination(targetCore.position);

        // Hedefe ulaştıysak ve saldırı zamanı geldiyse
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (attackCountdown <= 0f)
            {
                Attack();
                attackCountdown = 1f / attackRate;
            }
        }
        
        attackCountdown -= Time.deltaTime;
    }

    void Attack()
    {
        // Hedefin hala var olduğundan emin ol
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            // TODO: Saldırı animasyonu veya efekti tetiklenebilir
            Debug.Log(transform.name + ", BioCore'a " + damage + " hasar verdi.");
        }
    }
}