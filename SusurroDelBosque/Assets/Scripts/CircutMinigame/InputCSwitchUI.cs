using UnityEngine;
using UnityEngine.UI;

public class InputSwitch : MonoBehaviour
{
    private Image switchImage;

    // Sprites opcionales para el estado ON/OFF
    public Sprite spriteON;
    public Sprite spriteOFF;

    void Start()
    {
        switchImage = GetComponent<Image>();
        
        // Inicializa apagada
        SetState(false);
    }

    // 🔹 Método público para encender/apagar la lámpara
    public void SetState(bool state)
    {
        if (switchImage != null)
        {
            if (state && spriteON != null)
            {
                switchImage.sprite = spriteON;
                switchImage.color = Color.white;
            }
            else if (!state && spriteOFF != null)
            {
                switchImage.sprite = spriteOFF;
                switchImage.color = Color.white;
            }
            else
            {
                // Si no usas sprites, solo cambia color
                switchImage.color = state ? Color.white : Color.white;
            }
        }
    }
}