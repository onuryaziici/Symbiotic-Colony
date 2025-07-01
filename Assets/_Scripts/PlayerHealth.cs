// PlayerHealth.cs
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 500;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        // TODO: Can barı UI'ını güncelle
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        // TODO: Can barı UI'ını güncelle

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // GameManager'a oyunun bittiğini haber ver
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameState.GameOver);
        }
        // Çekirdeğin kendisini yok etmek yerine devre dışı bırakmak daha iyi olabilir
        gameObject.SetActive(false); 
    }
}