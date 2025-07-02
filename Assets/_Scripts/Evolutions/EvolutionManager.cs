using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    [Header("Setup")]
    public GameObject evolutionPanel;       // Evrim panelinin referansı
    public Transform cardContainer;         // Kartların oluşturulacağı UI konteyneri
    public GameObject cardPrefab;           // Tek bir kartın UI prefab'ı

    [Header("Card Pool")]
    public List<EvolutionCardData> availableCards; // Oyunda çıkabilecek tüm olası kartların listesi

    // Gerekli diğer yönetici script'lerine referanslar
    private WaveManager waveManager;
    private PlayerHealth playerHealth;
    private PlayerController playerController;

    // Stat bonuslarını saklamak için bir sözlük (Dictionary)
    // Key: "ClawCell_Damage" gibi benzersiz bir ID
    // Value: Toplam bonus miktarı (örn: %30 için 0.3)
    public Dictionary<string, float> statBonuses = new Dictionary<string, float>();

    void Start()
    {
        // Gerekli bileşenleri ve yöneticileri başta bir kere bulup sakla
        waveManager = FindObjectOfType<WaveManager>();
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            playerController = player.GetComponent<PlayerController>();
        }
        
        // Oyun başlangıcında Evrim panelinin kapalı olduğundan emin ol
        if (evolutionPanel != null)
        {
            evolutionPanel.SetActive(false);
        }
    }

    // Test amaçlı debug kodu
    void Update()
    {
        // Eğer 'K' tuşuna basılırsa ve panel zaten açık değilse Evrim ekranını tetikle
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (evolutionPanel != null && !evolutionPanel.activeInHierarchy)
            {
                Debug.Log("DEBUG: Evrim seçimi test tuşu ile tetiklendi.");
                TriggerEvolutionChoice();
            }
        }
    }

    // Bu metot, WaveManager tarafından veya test tuşu ile çağrılır
    public void TriggerEvolutionChoice()
    {
        // Oyunu durdur ve paneli görünür yap
        Time.timeScale = 0f;
        evolutionPanel.SetActive(true);

        // Bir önceki seçimden kalan kartları temizle
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        // Havuzdan rastgele 3 farklı kart seç
        List<EvolutionCardData> chosenCards = GetRandomCards(3);

        // Seçilen her kart için bir UI objesi oluştur ve ayarla
        foreach (EvolutionCardData cardData in chosenCards)
        {
            GameObject cardGO = Instantiate(cardPrefab, cardContainer);
            CardUI cardUI = cardGO.GetComponent<CardUI>();

            if (cardUI != null)
            {
                // CardUI'a hem kart verisini hem de bu yöneticinin referansını gönder
                cardUI.Setup(cardData, this);
            }
        }
    }

    // Bu metot, bir CardUI üzerindeki butona tıklandığında çağrılır
    public void ApplyEvolution(EvolutionCardData chosenCard)
    {
        Debug.Log("'" + chosenCard.cardName + "' kartı seçildi ve efekti uygulanıyor.");

        // Seçilen kartın türüne göre ilgili efekti uygula
        switch (chosenCard.effectType)
        {
            case EvolutionEffectType.PlayerStatChange:
                ApplyPlayerStatChange(chosenCard);
                break;

            case EvolutionEffectType.ModuleStatChange:
            case EvolutionEffectType.UnitStatChange:
                ApplyGenericStatChange(chosenCard);
                break;

            case EvolutionEffectType.ResourceGain:
                ApplyResourceGain(chosenCard);
                break;
        }

        // Seçim yapıldıktan sonra paneli kapat, oyunu devam ettir ve bir sonraki dalgayı başlat
        evolutionPanel.SetActive(false);
        Time.timeScale = 1f;
        
        if (waveManager != null)
        {
            waveManager.StartNextWave();
        }
    }

    // Modül ve Birim statlarını değiştiren genel metot
    private void ApplyGenericStatChange(EvolutionCardData card)
    {
        string id = card.targetId;
        float value = card.value;

        if (statBonuses.ContainsKey(id))
        {
            statBonuses[id] += value; // Var olan bonusun üzerine ekle
        }
        else
        {
            statBonuses.Add(id, value); // Yeni bir bonus olarak ekle
        }
        Debug.Log(id + " bonusu " + (statBonuses[id] * 100) + "% değerine yükseldi.");
    }

    // Oyuncu statlarını değiştiren metot
    private void ApplyPlayerStatChange(EvolutionCardData card)
    {
        if (card.targetId == "MaxHealth")
        {
            if (playerHealth != null)
            {
                int healAmount = (int)card.value;
                playerHealth.maxHealth += healAmount;
                playerHealth.Heal(healAmount); // Hem canı artır hem de UI'ı güncelle

                // Bu metot artık Heal içinde çağrıldığı için tekrar çağırmaya gerek yok,
                // ama yapısal olarak maxHealth değiştiğinde UI'ın güncellenmesi gerektiğini bilmek önemli.
                // playerHealth.UpdateMaxHealth(); 
            }
        }
    }

    // Anında kaynak kazandıran metot
    private void ApplyResourceGain(EvolutionCardData card)
    {
        if (playerController != null)
        {
            playerController.currentBiomass += (int)card.value;
            playerController.UpdateUI();
        }
    }

    // Diğer script'lerin bu bonusları sorgulayabilmesi için public metot
    public float GetStatBonus(string statId)
    {
        if (statBonuses.ContainsKey(statId))
        {
            return statBonuses[statId];
        }
        return 0f; // Eğer o stat için bir bonus yoksa 0 döndür
    }

    // Kart havuzundan belirtilen sayıda rastgele ve benzersiz kart seçen metot
    private List<EvolutionCardData> GetRandomCards(int count)
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
}