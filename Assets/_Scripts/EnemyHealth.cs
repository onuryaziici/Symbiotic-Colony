// EnemyHealth.cs
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(transform.name + " " + damage + " hasar aldı! Kalan can: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // TODO: Ölüm efekti, ganimet düşürme vb. eklenebilir.
        Debug.Log(transform.name + " yok oldu!");
        Destroy(gameObject);
    }
}