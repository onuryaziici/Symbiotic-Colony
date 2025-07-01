using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için gerekli

// Oyunun bulunabileceği durumları tanımlıyoruz
public enum GameState { MainMenu, Playing, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton deseni ile her yerden erişim

    public GameState currentState;

    [Header("UI Panels")]
    public GameObject gameOverUI;
    public TextMeshProUGUI waveReachedText; // Inspector'dan ScoreText'i buraya ata
    private WaveManager waveManager; // WaveManager'a referans

    void Awake()
    {
        // Singleton desenini kuruyoruz
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        waveManager = GetComponent<WaveManager>(); // WaveManager'ı bul
        // Oyun "Playing" durumunda başlasın
        ChangeState(GameState.Playing);
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.MainMenu:
                // TODO: Ana menü mantığı
                break;
            case GameState.Playing:
                // Oyun başladığında veya yeniden başladığında yapılacaklar
                Time.timeScale = 1f; // Zamanı normal akışına döndür
                if(gameOverUI != null) gameOverUI.SetActive(false);
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                if (gameOverUI != null) gameOverUI.SetActive(true);
                
                // Skoru güncelle
                if (waveReachedText != null && waveManager != null)
                {
                    // WaveManager'daki wave sayısını alıp yazdır
                    waveReachedText.text = "Ulaşılan Dalga: " + waveManager.GetCurrentWaveNumber(); 
                }
                break;
        }
    }

    // Bu metotları UI butonlarına bağlayacağız
    public void RestartGame()
    {
        // Aktif sahneyi yeniden yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Oyundan çıkıldı.");
    }
}