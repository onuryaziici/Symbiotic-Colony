using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    [Header("Setup")]
    public GameObject evolutionPanel; // Evrim panelini buraya sürükleyeceğiz
    public Transform cardContainer;   // Kartların oluşturulacağı yer
    public GameObject cardPrefab;     // Kart UI prefabı

    [Header("Card Pool")]
    public List<EvolutionCardData> availableCards; // Tüm olası kartların listesi

    private WaveManager waveManager;

    // Gerekli yöneticilere referanslar
    private PlayerHealth playerHealth;
    // Gelecekte eklenecekler: PlayerStats, ModuleManager, UnitManager vb.
    // Sadece test amaçlı bir Update metodu ekliyoruz.
    void Update()
    {
        // Eğer 'K' tuşuna basılırsa...
        if (Input.GetKeyDown(KeyCode.K))
        {
            // ... ve Evrim paneli zaten açık değilse (üst üste açılmasını engellemek için) ...
            if (!evolutionPanel.activeInHierarchy)
            {
                Debug.Log("DEBUG: Triggering evolution choice via key press.");
                TriggerEvolutionChoice();
            }
        }
    }
    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        evolutionPanel.SetActive(false);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    // Bu metot, WaveManager tarafından çağrılacak.
    public void TriggerEvolutionChoice()
    {
        Time.timeScale = 0f; // Oyunu durdur
        evolutionPanel.SetActive(true);

        // Mevcut kartları temizle (varsa)
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        // Havuzdan rastgele 3 farklı kart seç
        List<EvolutionCardData> chosenCards = GetRandomCards(3);

        // Seçilen kartlar için UI oluştur
        foreach (EvolutionCardData cardData in chosenCards)
        {
            // Kart prefab'ını oluştur
            GameObject cardGO = Instantiate(cardPrefab, cardContainer);

            // Oluşturulan kartın üzerindeki CardUI script'ini bul
            CardUI cardUI = cardGO.GetComponent<CardUI>();

            // Eğer script bulunduysa, Setup metodunu çağırarak verileri gönder
            if (cardUI != null)
            {
                cardUI.Setup(cardData, this); // 'this' diyerek kendisini (EvolutionManager'ı) de gönderir
            }
            else
            {
                Debug.LogError("Card Prefab'ında CardUI script'i bulunamadı!");
            }
        }
    }

    List<EvolutionCardData> GetRandomCards(int count)
    {
        List<EvolutionCardData> poolCopy = new List<EvolutionCardData>(availableCards);
        List<EvolutionCardData> chosen = new List<EvolutionCardData>();

        for (int i = 0; i < count; i++)
        {
            if (poolCopy.Count == 0) break;
            int randomIndex = Random.Range(0, poolCopy.Count);
            chosen.Add(poolCopy[randomIndex]);
            poolCopy.RemoveAt(randomIndex);
        }

        return chosen;
    }

    // Bu metot, kartın butonuna tıklandığında çağrılacak.
    public void ApplyEvolution(EvolutionCardData chosenCard)
    {
        Debug.Log(chosenCard.cardName + " seçildi!");
        // --- YENİ EKLENEN EFEKT UYGULAMA MANTIĞI ---
        switch (chosenCard.effectType)
        {
            case EvolutionEffectType.PlayerStatChange:
                ApplyPlayerStatChange(chosenCard);
                break;

            case EvolutionEffectType.ModuleStatChange:
                // TODO: Modül statlarını değiştiren metodu çağır
                Debug.LogWarning("ModuleStatChange efekti henüz uygulanmadı.");
                break;

            case EvolutionEffectType.UnitStatChange:
                // TODO: Birim statlarını değiştiren metodu çağır
                Debug.LogWarning("UnitStatChange efekti henüz uygulanmadı.");
                break;

            case EvolutionEffectType.ResourceGain:
                // TODO: Kaynak kazandıran metodu çağır
                Debug.LogWarning("ResourceGain efekti henüz uygulanmadı.");
                break;
        }
        // Seçim yapıldıktan sonra paneli kapat ve oyunu devam ettir
        evolutionPanel.SetActive(false);
        Time.timeScale = 1f;
        if (waveManager != null)
        {
            waveManager.StartNextWave();
        }
    }
    
    // Player statlarını değiştiren yardımcı metot
    private void ApplyPlayerStatChange(EvolutionCardData card)
    {
        if (playerHealth == null) return;

        // targetId'ye göre hangi statın değişeceğine karar ver
        if (card.targetId == "MaxHealth")
        {
            // Kartın value'su kadar maksimum canı artır
            playerHealth.maxHealth += (int)card.value;
            // Mevcut canı da aynı miktarda artırarak oyuncuyu ödüllendir
            playerHealth.currentHealth += (int)card.value;
            // TODO: Can barı UI'ını güncelle
            Debug.Log("BioCore maksimum canı " + card.value + " arttı!");
        }
        // else if (card.targetId == "Speed") { ... }
        // ... diğer statlar ...
    }
}