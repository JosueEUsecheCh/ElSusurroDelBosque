using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour  
{
    public Image iconImage;
    public Text nameText;
    
    // AÑADISTE ESTO:
    public string itemName;
    public string itemDescription; // ¡Genial!
    public Sprite itemIcon;

    // Configura el UI con los datos del objeto pickeable
    public void Setup(Sprite icon, string itemName, string itemDescription) // <--- ¡Modifica la firma del método!
    {
        if (iconImage != null)
            iconImage.sprite = icon;

        if (nameText != null)
            nameText.text = itemName;
        
        // Ahora sí, guarda los datos en las variables
        this.itemName = itemName;
        this.itemIcon = icon;
        this.itemDescription = itemDescription; // <--- ¡Añade esta línea!
    }
}