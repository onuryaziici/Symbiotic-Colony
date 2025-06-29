using UnityEngine;
using UnityEngine.AI;
using TMPro; // TextMeshPro için gerekli

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public int currentBiomass = 0;

    [Header("UI")]
    public TextMeshProUGUI biomassText; // UI metnini buraya sürükleyeceğiz

    private Camera cam;
    private NavMeshAgent agent;

    void Start()
    {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        BiomassResource resource = other.GetComponent<BiomassResource>();
        if (resource != null)
        {
            currentBiomass += resource.amount;
            Debug.Log("Biomass Toplandı! Mevcut: " + currentBiomass);
            Destroy(other.gameObject);
            UpdateUI(); // UI'ı güncelle
        }
    }

    public void UpdateUI()
    {
        biomassText.text = "Biomass: " + currentBiomass;
    }
}