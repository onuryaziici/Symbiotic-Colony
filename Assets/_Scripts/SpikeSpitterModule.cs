// SpikeSpitterModule.cs
using UnityEngine;

public class SpikeSpitterModule : MonoBehaviour
{
    public float fireRate = 1f; // Saniyede kaç atış
    public float range = 15f; // Ateş menzili
    public GameObject projectilePrefab; // Mermi prefabı
    public Transform firePoint; // Merminin çıkacağı nokta

    private float fireCountdown = 0f;
    private Transform target;

    void Start()
    {
        // Periyodik olarak hedef ara
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
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

        if (nearestEnemy != null && shortestDistance <= range)
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

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        // TODO: Mermi fırlatma kodu
        Debug.Log(target.name + " hedefine ateş ediliyor!");
    }

    // Menzili Scene'de görmek için
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}