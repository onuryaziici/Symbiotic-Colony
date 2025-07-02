// PlayerHealth.cs
using UnityEngine;
using UnityEngine.UI; // UI elemanlarını kullanmak için bu satır gerekli

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 500;
    public int currentHealth;

    [Header("UI")]
    public Slider healthBar; // Inspector'dan HealthBar'ı buraya sürükleyeceğiz

    void Start()
    {
        currentHealth = maxHealth;
        // Başlangıçta can barını ayarla
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        // Can azaldığında barı güncelle
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            currentHealth = 0; // Canın eksiye düşmesini engelle
            Die();
        }
    }

    // Bu metodu, Evrim kartı gibi canı artıran şeyler için de kullanabiliriz
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        // Can arttığında barı güncelle
        UpdateHealthBar();
    }
    
    // Bu metot, maksimum can arttığında çağrılmalı
    public void UpdateMaxHealth()
    {
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            // Slider'ın değerini 0 ile 1 arasında bir orana çeviriyoruz.
            // Örn: 250 / 500 = 0.5
            healthBar.value = (float)currentHealth / maxHealth;
        }
    }

    void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameState.GameOver);
        }
        gameObject.SetActive(false);
    }
}