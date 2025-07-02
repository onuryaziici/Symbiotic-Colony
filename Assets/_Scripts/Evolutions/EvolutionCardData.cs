using UnityEngine;

// Bu enum, kartın ne tür bir etki yaptığını tanımlamamıza yardımcı olacak.
public enum EvolutionEffectType
{
    PlayerStatChange,       // Oyuncunun (BioCore) bir istatistiğini değiştirir
    ModuleStatChange,       // Belirli bir modül türünün istatistiğini değiştirir
    UnitStatChange,         // Belirli bir birim türünün istatistiğini değiştirir
    ResourceGain            // Anında kaynak kazandırır
}

[CreateAssetMenu(fileName = "New Evolution Card", menuName = "Symbiotic Colony/Evolution Card")]
public class EvolutionCardData : ScriptableObject
{
    [Header("Card Info")]
    public string cardName;
    [TextArea(3, 5)] // Inspector'da daha geniş bir metin alanı sağlar
    public string description;
    public Sprite icon; // Kartın görseli (opsiyonel)

    [Header("Effect Details")]
    public EvolutionEffectType effectType;
    public float value; // Etkinin değeri (örn: +%20 için 0.2, +10 can için 10)
    
    // Bu alanlar sadece belirli etki türleri için anlamlı olacak.
    // Örneğin, ModuleStatChange ise, hangi modülü etkileyeceğini belirtmek için kullanılabilir.
    public string targetId; // Örn: "SpikeSpitter", "ClawCell"
}