using UnityEngine;

public class SpikeSpitterModule : MonoBehaviour
{
    [Header("Base Stats")]
    public float baseFireRate = 1f;    // Temel atış hızı (saniyedeki atış sayısı)
    public float baseRange = 15f;      // Temel menzil

    [Header("Setup")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public string enemyTag = "Enemy";

    // Bonuslarla hesaplanmış güncel statlar
    private float currentFireRate;
    private float currentRange;

    // Özel değişkenler
    private float fireCountdown = 0f;
    private Transform target;

    void Start()
    {
        ApplyBonuses();
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    void ApplyBonuses()
    {
        // Başlangıçta güncel statları temel statlara eşitle
        currentFireRate = baseFireRate;
        currentRange = baseRange;

        // EvolutionManager'ı bul ve bonusları al
        EvolutionManager evoManager = FindObjectOfType<EvolutionManager>();
        if (evoManager != null)
        {
            // Atış hızı bonusunu al ve uygula
            float fireRateBonus = evoManager.GetStatBonus("SpikeSpitter_FireRate");
            currentFireRate *= (1f + fireRateBonus); // Örn: 1 * (1 + 0.15) = 1.15

            // Menzil bonusunu al ve uygula
            float rangeBonus = evoManager.GetStatBonus("SpikeSpitter_Range");
            currentRange *= (1f + rangeBonus);
        }
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        // Hedefin menzil içinde olup olmadığını `currentRange` ile kontrol et
        if (nearestEnemy != null && shortestDistance <= currentRange)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    void Update()
    {
        if (target == null)
            return;

        // Zamanlayıcıyı sıfırla ve ateş et
        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / currentFireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileGO.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.Seek(target);
        }
    }

    // Scene'de menzili görselleştirmek için
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Menzil çizimini de güncel stat'a göre yap
        Gizmos.DrawWireSphere(transform.position, currentRange > 0 ? currentRange : baseRange);
    }
}