using UnityEngine;
using UnityEngine.EventSystems; // UI'a tıklanıp tıklanmadığını kontrol etmek için

public class SproutController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject sproutMenu;

    [Header("Module Prefabs")]
    public GameObject spikeSpitterPrefab;
    public GameObject clawNestPrefab;

    [Header("Unit Prefabs")]
    public GameObject clawCellPrefab; // Yeni birim prefabı

    [Header("Settings")]
    public LayerMask slotLayerMask; // Inspector'dan 'Slots' katmanını seçeceğiz

    private PlayerController playerController;
    private ModuleSlot selectedSlot;

    void Start()
    {
        // Gerekli bileşenleri başta bir kere bulup saklamak performansı artırır.
        playerController = GetComponent<PlayerController>();

        // Oyun başlangıcında menünün kapalı olduğundan emin ol.
        if (sproutMenu != null)
        {
            sproutMenu.SetActive(false);
        }
    }

    void Update()
    {
        // 1. Sol fare tuşuna basıldığı anı yakala.
        // 2. Farenin bir UI elemanının üzerinde olup olmadığını kontrol et. Eğer öyleyse, dünyadaki objelere tıklamayı engelle.
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 3. Sadece 'Slots' katmanındaki objelere bir ışın gönder.
            if (Physics.Raycast(ray, out hit, 100f, slotLayerMask))
            {
                // Işının çarptığı objeden veya onun üst objesinden ModuleSlot bileşenini bulmaya çalış.
                ModuleSlot slot = hit.collider.GetComponentInParent<ModuleSlot>();

                // Eğer bir slot bulduysak ve bu slot boşsa...
                if (slot != null && !slot.IsOccupied())
                {
                    OpenSproutMenu(slot);
                }
                // Eğer slot doluysa veya tıklanan şey bir slot değilse menüyü kapat.
                else
                {
                    CloseSproutMenu();
                }
            }
            // 4. Işın hiçbir şeye çarpmadıysa (boşluğa tıklandıysa) menüyü yine kapat.
            else
            {
                CloseSproutMenu();
            }
        }
    }

    void OpenSproutMenu(ModuleSlot slot)
    {
        selectedSlot = slot;
        sproutMenu.transform.position = Input.mousePosition; // Menüyü fare pozisyonunda aç
        sproutMenu.SetActive(true);
    }

    public void CloseSproutMenu()
    {
        sproutMenu.SetActive(false);
        selectedSlot = null;
    }

    // Bu metot, UI'daki "Build Spike Spitter" butonuna bağlı.
    public void BuildSpikeSpitter()
    {
        // Gerekli kontroller: Slot seçili mi? Yeterli kaynak var mı?
        if (selectedSlot != null && playerController.currentBiomass >= 125)
        {
            // Kaynağı harca ve UI'ı güncelle.
            playerController.currentBiomass -= 125;
            playerController.UpdateUI();

            // Modülü oluştur ve slotun pozisyonuna/rotasyonuna yerleştir.
            GameObject moduleGO = Instantiate(spikeSpitterPrefab, selectedSlot.transform.position, selectedSlot.transform.rotation);

            // Oluşturulan modülü slotun alt objesi (child) yap. Bu, Çekirdek hareket ettiğinde modülün de onunla gelmesini sağlar.
            moduleGO.transform.SetParent(selectedSlot.transform);

            // Slotun artık dolu olduğunu ve üzerinde hangi modülün olduğunu kaydet.
            selectedSlot.currentModule = moduleGO;

            // Slotun görselini (disk) gizle.
            Renderer slotVisual = selectedSlot.GetComponentInChildren<Renderer>();
            if (slotVisual != null)
            {
                slotVisual.enabled = false;
            }

            // Her işlemden sonra menüyü kapatmak iyi bir pratiktir.
            CloseSproutMenu();
        }
        else
        {
            // Oyuncuya neden inşa edemediğini bildiren bir geri bildirim (ses, UI mesajı vb.) eklenebilir.
            Debug.LogWarning("İnşa etmek için yeterli Biyo-Kütle yok veya slot seçili değil!");
            CloseSproutMenu();
        }
    }
    // Yeni bir metot: Bu, inşa menüsündeki "Pençe Yuvası İnşa Et" butonuna bağlanacak.
public void BuildClawNest()
{
    if (selectedSlot != null && playerController.currentBiomass >= 75) // Maliyeti 75 olsun
    {
        playerController.currentBiomass -= 75;
        playerController.UpdateUI();

        GameObject moduleGO = Instantiate(clawNestPrefab, selectedSlot.transform.position, selectedSlot.transform.rotation);
        moduleGO.transform.SetParent(selectedSlot.transform);
        selectedSlot.currentModule = moduleGO;

        Renderer slotVisual = selectedSlot.GetComponentInChildren<Renderer>();
        if (slotVisual != null) { slotVisual.enabled = false; }
        
        CloseSproutMenu();
    }
    else
    {
        Debug.LogWarning("Yeterli Biyo-Kütle yok!");
        CloseSproutMenu();
    }
}

// Yeni bir metot: Bu, ana UI'daki "Pençe Hücresi Üret" butonuna bağlanacak.
public void SpawnClawCell()
{
    if (playerController.currentBiomass >= 25) // Maliyeti 25
    {
        // Oyuncunun sahip olduğu tüm Pençe Yuvalarını bul
        ClawNestModule[] nests = FindObjectsOfType<ClawNestModule>();
        if (nests.Length > 0)
        {
            // İlk bulduğun yuvadan birim üret (şimdilik basit tutalım)
            Transform spawnPoint = nests[0].spawnPoint;
            Instantiate(clawCellPrefab, spawnPoint.position, spawnPoint.rotation);

            playerController.currentBiomass -= 25;
            playerController.UpdateUI();
        }
        else
        {
            Debug.LogWarning("Pençe Hücresi üretmek için Pençe Yuvası yok!");
        }
    }
}
    
}