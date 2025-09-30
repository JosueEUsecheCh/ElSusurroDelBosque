using UnityEngine;

public class InputSwitchSP: MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Header("Sprites de la lámpara")]
    public Sprite spriteON;
    public Sprite spriteOFF;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("⚠️ No se encontró un SpriteRenderer en " + gameObject.name);
        }

        // Inicializa apagada
        SetState(false);
    }

    /// <summary>
    /// Cambia el estado de la lámpara
    /// </summary>
    public void SetState(bool state)
    {
        if (spriteRenderer == null) return;

        if (state && spriteON != null)
        {
            spriteRenderer.sprite = spriteON;
            spriteRenderer.color = Color.white;
        }
        else if (!state && spriteOFF != null)
        {
            spriteRenderer.sprite = spriteOFF;
            spriteRenderer.color = Color.white;
        }
        else
        {
            // Si no hay sprites configurados, solo cambia color
            spriteRenderer.color = state ? Color.white : Color.gray;
        }
    }
}