// CardUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;
    public Button selectButton;

    private EvolutionCardData cardData;
    private EvolutionManager evolutionManager;

    // Bu metot, EvolutionManager tarafından çağrılacak.
    public void Setup(EvolutionCardData data, EvolutionManager manager)
    {
        cardData = data;
        evolutionManager = manager;

        // UI elemanlarını gelen veriyle doldur
        titleText.text = cardData.cardName;
        descriptionText.text = cardData.description;
        
        if (cardData.icon != null)
        {
            iconImage.sprite = cardData.icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false;
        }

        // Butonun OnClick olayını kodla ayarla
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnCardSelected);
    }

    void OnCardSelected()
    {
        // EvolutionManager'a hangi kartın seçildiğini bildir
        if (evolutionManager != null)
        {
            evolutionManager.ApplyEvolution(cardData);
        }
    }
}