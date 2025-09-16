using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour  // Funcion de tomar datos para ver en el inventario
{
    public Image iconImage; // La Image del prefab
    public Text nameText;   // El hijo Text del Button

    // Configura el UI con los datos del objeto pickeable
    public void Setup(Sprite icon, string itemName)
    {
        if (iconImage != null)
            iconImage.sprite = icon;

        if (nameText != null)
            nameText.text = itemName;
    }
}
