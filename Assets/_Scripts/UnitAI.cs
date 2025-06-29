// UnitAI.cs
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent kullanmak için bu satır gerekli

public class UnitAI : MonoBehaviour
{
    [Header("Stats")]
    public float attackRange = 2f;    // Düşmana saldırmaya başlayacağı mesafe
    public float attackRate = 1f;     // Saniyede kaç kez saldıracağı
    public int damage = 5;            // Her saldırıda vereceği hasar

    [Header("Setup")]
    public string enemyTag = "Enemy"; // Hedef alacağı düşmanların etiketi

    private Transform target;             // Mevcut hedefi
    private NavMeshAgent agent;           // Hareketi sağlayan bileşen
    private float attackCountdown = 0f;   // Bir sonraki saldırıya kalan süre

    void Start()
    {
        // Gerekli bileşenleri başta bir kere bulup sakla
        agent = GetComponent<NavMeshAgent>();

        // NavMeshAgent'a, hedefe ne kadar mesafe kala durması gerektiğini söyle.
        // Bu, titreme ve iç içe geçme sorununu çözer.
        if (agent != null)
        {
            agent.stoppingDistance = attackRange;
        }

        // Oyunda bir düşman varsa, onu hedef olarak belirle.
        // Bu basit bir AI, daha gelişmiş sistemler periyodik olarak en yakın hedefi arar.
        GameObject initialTarget = GameObject.FindGameObjectWithTag(enemyTag);
        if (initialTarget != null)
        {
            target = initialTarget.transform;
        }
    }

    void Update()
    {
        // Eğer hedef yoksa veya hedef yok olduysa, hiçbir şey yapma.
        if (target == null)
        {
            // İsteğe bağlı: Burada birime bekleme veya üsse geri dönme komutu verilebilir.
            // Şimdilik sadece durmasını sağlıyoruz.
            if(agent.hasPath)
            {
                agent.ResetPath();
            }
            return;
        }

        // Hedefe doğru gitmesi için komutu sürekli yenile.
        agent.SetDestination(target.position);

        // Ajanın hedefe ulaşıp ulaşmadığını kontrol et.
        // `!agent.pathPending` -> Ajanın hala bir yol hesaplamaya çalışmadığından emin olur.
        // `agent.remainingDistance <= agent.stoppingDistance` -> Ajanın hedefe yeterince yaklaştığını kontrol eder.
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Saldırı süresi geldiyse saldır.
            if (attackCountdown <= 0f)
            {
                FaceTarget(); // Saldırmadan önce hedefe dön (daha şık bir görüntü için).
                Attack();
                attackCountdown = 1f / attackRate; // Zamanlayıcıyı sıfırla.
            }
        }

        // Her frame'de saldırı zamanlayıcısını azalt.
        attackCountdown -= Time.deltaTime;
    }

    // Hedefe dönme fonksiyonu
    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        // Sadece yatay eksende (Y eksenini sıfırlayarak) dönmesini sağla.
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        // Dönüşü anlık yapmak yerine yumuşak bir geçişle yap (daha doğal görünür).
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // Saldırı fonksiyonu
    void Attack()
    {
        // Hedefin sağlık bileşenini bulmaya çalış.
        EnemyHealth enemy = target.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            // Eğer bulduysan hasar ver.
            enemy.TakeDamage(damage);
        }
    }
}