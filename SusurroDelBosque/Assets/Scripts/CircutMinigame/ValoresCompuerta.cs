using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ValoresCompuerta : MonoBehaviour, IPointerClickHandler
{
    // Imagen de la bolita (el círculo blanco)
    private Image compuertaImage;

    // Texto que mostrará 1 o 0
    public string id_value; // o TextMeshProUGUI si usas TMP
    


    // Valor actual de la compuerta (0 = OFF, 1 = ON)
    public int currentValue = 0;

    void Start()
    {
        compuertaImage = GetComponent<Image>();

        // Inicializa en OFF (rojo y texto "0")
        UpdateVisual(false);
        Debug.Log($"soy: {id_value}");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Cambiar el valor (0 -> 1, 1 -> 0)
        bool newState = currentValue == 0;
        currentValue = newState ? 1 : 0;

        // Actualizar la parte visual
        UpdateVisual(newState);

        Debug.Log($"{gameObject.name} valor cambiado a: {currentValue}");
    }

    private void UpdateVisual(bool state)
    {
        Debug.Log($"soy: {id_value}");
        if (compuertaImage != null)
        {
            // Cambia color de la bolita
            compuertaImage.color = state ? Color.green : Color.red;
        }

    }
}
