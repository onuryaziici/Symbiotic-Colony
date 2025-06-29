using UnityEngine;

// ModuleSlot artık hem MonoBehaviour hem de IHighlightable
public class ModuleSlot : MonoBehaviour, IHighlightable 
{
    public GameObject currentModule = null;
    public Color highlightColor = Color.green;

    private Renderer visualRenderer;
    private Color originalColor;

    void Start()
    {
        visualRenderer = GetComponentInChildren<Renderer>();
        if (visualRenderer != null)
        {
            originalColor = visualRenderer.material.color;
        }
    }

    public bool IsOccupied()
    {
        return currentModule != null;
    }

    // --- IHighlightable Arayüzünden Gelen Metotlar ---

    public void Highlight()
    {
        if (!IsOccupied() && visualRenderer != null)
        {
            visualRenderer.material.color = highlightColor;
        }
    }

    public void Unhighlight()
    {
        if (visualRenderer != null)
        {
            visualRenderer.material.color = originalColor;
        }
    }
}