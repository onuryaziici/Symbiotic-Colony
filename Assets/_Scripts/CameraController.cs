using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Panning")]
    public float panSpeed = 20f;
    public float panBorderThickness = 10f; // Ekran kenarına gelince kaydırma (opsiyonel)
    public Vector2 panLimitX; // Kameranın X ekseninde gidebileceği sınırlar
    public Vector2 panLimitZ; // Kameranın Z ekseninde gidebileceği sınırlar

    [Header("Zooming")]
    public float scrollSpeed = 20f;
    public float minY = 10f; // En yakın zoom mesafesi
    public float maxY = 80f; // En uzak zoom mesafesi


    void Update()
    {
        Vector3 pos = transform.position;

        // WASD ile kaydırma
        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        // Fare tekerleği ile zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;

        // Sınırları uygulama
        pos.x = Mathf.Clamp(pos.x, panLimitX.x, panLimitX.y);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, panLimitZ.x, panLimitZ.y);
        
        transform.position = pos;
    }
}