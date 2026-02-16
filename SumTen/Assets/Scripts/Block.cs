using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
    public int Value { get; private set; }
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void SetValue(int value)
    {
        Value = value;
        if (textMesh != null)
        {
            textMesh.text = value.ToString();
        }
        
        // Change color based on value for variety
        if (spriteRenderer != null)
        {
            spriteRenderer.color = GetColorForValue(value);
        }
    }

    private Color GetColorForValue(int value)
    {
        switch (value)
        {
            case 1: return Color.red;
            case 2: return new Color(1f, 0.5f, 0f); // Orange
            case 3: return Color.yellow;
            case 4: return Color.green;
            case 5: return Color.cyan;
            case 6: return Color.blue;
            case 7: return new Color(0.5f, 0f, 1f); // Purple
            case 8: return Color.magenta;
            case 9: return Color.gray;
            default: return Color.white;
        }
    }
}
