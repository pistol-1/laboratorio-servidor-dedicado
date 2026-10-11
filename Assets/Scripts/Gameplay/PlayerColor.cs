using UnityEngine;

public class PlayerColor : MonoBehaviour
{
    public Color baseColor = Color.white;

    private Renderer playerRenderer;

    private void Awake()
    {
        playerRenderer = GetComponent<Renderer>();
        Restore();
    }

    public void Show(Color color)
    {
        playerRenderer.material.color = color;
    }

    public void Restore()
    {
        Show(baseColor);
    }
}
