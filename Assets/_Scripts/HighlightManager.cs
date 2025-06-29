using UnityEngine;

public class HighlightManager : MonoBehaviour
{
    public LayerMask highlightLayerMask; // Sadece bu katmandaki objeleri kontrol et
    
    private IHighlightable lastHighlighted = null; // Bir önceki frame'de vurgulanan obje

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Farenin altındaki objeyi bul
        if (Physics.Raycast(ray, out hit, 100f, highlightLayerMask))
        {
            IHighlightable currentHighlighted = hit.collider.GetComponentInParent<IHighlightable>();
            
            // 1. Yeni bir objenin üzerine mi geldik?
            if (currentHighlighted != null && currentHighlighted != lastHighlighted)
            {
                // Öncekini unhighlight et (eğer varsa)
                if (lastHighlighted != null)
                {
                    lastHighlighted.Unhighlight();
                }
                // Yeniyi highlight et
                currentHighlighted.Highlight();
                lastHighlighted = currentHighlighted;
            }
            // 2. Halen aynı objenin üzerinde miyiz?
            else if (currentHighlighted != null && currentHighlighted == lastHighlighted)
            {
                // Hiçbir şey yapma
            }
            // 3. Vurgulanabilir olmayan bir objenin üzerine mi geldik?
            else
            {
                if (lastHighlighted != null)
                {
                    lastHighlighted.Unhighlight();
                    lastHighlighted = null;
                }
            }
        }
        // Fare boşluktaysa, son vurgulananı da temizle
        else
        {
            if (lastHighlighted != null)
            {
                lastHighlighted.Unhighlight();
                lastHighlighted = null;
            }
        }
    }
}