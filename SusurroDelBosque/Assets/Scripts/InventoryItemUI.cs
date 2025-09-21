using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour  
{
    // Variables de Slot
    public Image iconImage;
    public Text nameText;
    
    //Variables de Description
    public string itemName;
    public string itemDescription; 
    public Sprite itemIcon;

    // Configura el UI con los datos del objeto pickeable
    public void Setup(Sprite icon, string itemName, string itemDescription) 
    {
        if (iconImage != null)
            iconImage.sprite = icon;

        if (nameText != null)
            nameText.text = itemName;
        
        //Guardar los datos a mostrar en la descipcion en las variables
        this.itemName = itemName;
        this.itemIcon = icon;
        this.itemDescription = itemDescription; 
    }
}