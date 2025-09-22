using UnityEngine;
using UnityEngine.UI;

public class AttributeController : MonoBehaviour
{
    public string itemName;         // Nombre del objeto en los slots del inventario
    
    private Text nameText;           // Texto que mostrará el nombre (Legacy Text)

    public void setName(string name)
    {
        this.itemName = name;
        if (nameText != null)
            nameText.text = name;   // Actualiza el texto en UI
    }

    public string getName()
    {
        return this.itemName;
    }
}
