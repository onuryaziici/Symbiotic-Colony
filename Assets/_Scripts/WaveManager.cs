// WaveManager.cs
using System.Collections;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    // Dalga durumlarını tanımlıyoruz
    private enum SpawnState { COUNTING, SPAWNING, WAITING_FOR_ENEMIES }
    private SpawnState currentState;

    [Header("Setup")]
    public WaveData[] waves;
    public Transform[] spawnPoints;
    public TextMeshProUGUI waveCountdownText;
    public TextMeshProUGUI waveCountText;

    [Header("Timings")]
    public float timeBetweenWaves = 10f;

    private int currentWaveIndex = 0;
    private float waveCountdown;
    private float searchCountdown = 1f; // Düşmanları ne sıklıkla kontrol edeceğimiz
    private EvolutionManager evolutionManager;

    void Start()
    {
        evolutionManager = GetComponent<EvolutionManager>();
        waveCountdown = timeBetweenWaves;
        currentState = SpawnState.COUNTING; // Geri sayımla başla
        UpdateWaveCountUI();
    }

    void Update()
    {
        // Sadece oyun "Playing" durumundayken çalış
        if (GameManager.Instance.currentState != GameState.Playing) return;

        // Eğer bir önceki dalga bittiyse ve tüm düşmanlar öldüyse, yeni dalganın geri sayımına başla.
        if (currentState == SpawnState.WAITING_FOR_ENEMIES)
        {
            if (!EnemiesAreAlive())
            {
                // Dalga tamamlandı!
                StartNewWaveCountdown();
            }
            else
            {
                // Düşmanlar hala hayatta, beklemeye devam et.
                return;
            }
        }

        // Geri sayım yapılıyorsa
        if (currentState == SpawnState.COUNTING)
        {
            if (waveCountdown <= 0f)
            {
                // Geri sayım bitti, dalgayı başlat.
                // Artık bir Coroutine başlatmaya gerek yok, çünkü durumu değiştiriyoruz.
                currentState = SpawnState.SPAWNING;
                waveCountdownText.gameObject.SetActive(false);
                StartCoroutine(SpawnWave(waves[currentWaveIndex]));
            }
            else
            {
                waveCountdown -= Time.deltaTime;
                waveCountdownText.text = "Next Wave In: " + Mathf.Ceil(waveCountdown).ToString();
                waveCountdownText.gameObject.SetActive(true);
            }
        }
    }

    // Haritada hala "Enemy" tag'li düşman olup olmadığını kontrol eder.
    bool EnemiesAreAlive()
    {
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0f)
        {
            searchCountdown = 1f; // Her saniyede bir kontrol et (performans için)
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false; // Düşman kalmadı
            }
        }
        return true; // Düşman var
    }

    void StartNewWaveCountdown()
    {
        Debug.Log("Wave " + (currentWaveIndex + 1) + " Completed!");

        // 1. KAZANMA DURUMUNU KONTROL ET
        // Eğer az önce biten dalga, son dalga ise, oyunu bitir ve kazanma ekranını göster.
        if (currentWaveIndex + 1 >= waves.Length)
        {
            Debug.Log("ALL WAVES COMPLETED! You Win!");
            
            // UI'ı "Kazanma" durumuna getir
            waveCountText.text = "YOU WIN!";
            if (waveCountdownText != null)
            {
                waveCountdownText.gameObject.SetActive(false);
            }
            
            // WaveManager'ın daha fazla çalışmasını engelle
            this.enabled = false;
            return; // Metodun geri kalanını çalıştırmadan buradan çık
        }

        // 2. EVRİM ZAMANINI KONTROL ET
        // Eğer az önce biten dalganın numarası (1'den başlayarak) 3'ün katıysa, evrim ekranını tetikle.
        // (Örn: 3. dalga bittiğinde currentWaveIndex=2 olur, 2+1=3. 3%3=0 olduğu için koşul sağlanır)
        if ((currentWaveIndex + 1) % 3 == 0)
        {
            if (evolutionManager != null)
            {
                evolutionManager.TriggerEvolutionChoice();
                // Evrim ekranı açılacağı ve oyun duracağı için, bu metot görevini tamamlamıştır.
                // Yeni dalganın geri sayımını başlatma sorumluluğu artık EvolutionManager'dadır.
                return; 
            }
        }

        // 3. NORMAL DALGA GEÇİŞİ
        // Eğer kazanma veya evrim durumu yoksa, direkt olarak bir sonraki dalgaya geç.
        StartNextWave();
    }

    // Artık parametre alıyor
    IEnumerator SpawnWave(WaveData wave)
    {
        Debug.Log("Spawning Wave: " + (currentWaveIndex + 1));

        foreach (EnemyGroup group in wave.enemyGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                SpawnEnemy(group.enemyPrefab);
                yield return new WaitForSeconds(wave.timeBetweenSpawns);
            }
        }

        // Tüm düşmanlar doğduktan sonra, onların ölmesini beklemeye başla.
        currentState = SpawnState.WAITING_FOR_ENEMIES;
        yield break;
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
    }

    void UpdateWaveCountUI()
    {
        waveCountText.text = "Wave: " + (currentWaveIndex + 1).ToString();
    }

    public int GetCurrentWaveNumber()
    {
        // GameOver ekranı için
        return currentWaveIndex + 1;
    }
    public void StartNextWave()
    {
        // Bu metot, EvolutionManager tarafından çağrılacak.
        currentWaveIndex++;
        currentState = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;
        UpdateWaveCountUI();
    }
}