using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;

public class InventoryController : MonoBehaviour
{
    public GameObject[] slots;
    private int num_slots_max = 4;

    [Header("UI Prefab")]
    public GameObject itemUIPrefab;
    public Transform inventorySlotsUI;
    public int selectedSlotIndex = -1;

    // Datos de los items recogidos
    private string[] itemNames;
    private Sprite[] itemIcons;
    private string[] itemDescriptions;

    // Descripcion de objetos inventario
    public Image descriptionImage;
    public Text descriptionNameText;
    public Text descriptionText;
    public GameObject descriptionPanel; // Nueva referencia al panel para ocultar/mostrar

    // Referencia al contenedor del inventario para mostrar/ocultar
    public GameObject inventoryUI;

    void Start()
    {
        // Inicializamos los arreglos
        slots = new GameObject[num_slots_max];
        itemNames = new string[num_slots_max];
        itemIcons = new Sprite[num_slots_max];
        itemDescriptions = new string[num_slots_max];

        // Ocultamos la UI del inventario y la descripción al inicio
        inventoryUI.SetActive(false);
        UpdateDescriptionDisplay();
    }
    
    // Método para mostrar el inventario y preseleccionar el primer slot con un objeto
    public void ShowInventory()
    {
        inventoryUI.SetActive(true);
        // Encontramos el primer slot con un objeto y lo seleccionamos
        int firstItemIndex = -1;
        for (int i = 0; i < num_slots_max; i++)
        {
            if (itemNames[i] != null)
            {
                firstItemIndex = i;
                break;
            }
        }
        SelectSlot(firstItemIndex);
        UpdateInventoryUI();
    }

    public void HideInventory()
    {
        inventoryUI.SetActive(false);
        SelectSlot(-1); // Limpiar la selección al cerrar
    }

    // Método para añadir un item recogido
    public void AddItem(string name, Sprite icon, string description)
    {
        for (int i = 0; i < num_slots_max; i++)
        {
            if (itemNames[i] == null)
            {
                itemNames[i] = name;
                itemIcons[i] = icon;
                itemDescriptions[i] = description;
                
                // Actualiza la UI y selecciona el slot del item recién añadido
                UpdateInventoryUI();
                SelectSlot(i);
                
                break;
            }
        }
    }

    // Actualiza la UI del inventario usando el prefab genérico
    public void UpdateInventoryUI()
    {
        for (int i = 0; i < num_slots_max; i++)
        {
            Transform slotTransform = inventorySlotsUI.GetChild(i);

            // Limpiamos el slot antes de recrearlo
            if (slotTransform.childCount > 0)
            {
                Destroy(slotTransform.GetChild(0).gameObject);
            }

            // Si hay un item, instanciamos el prefab y lo configuramos
            if (itemNames[i] != null)
            {
                GameObject uiItem = Instantiate(itemUIPrefab, slotTransform);
                uiItem.transform.localPosition = Vector3.zero;

                // Almacenamos el objeto instanciado en el arreglo de slots
                slots[i] = uiItem;
                
                // Configuramos el script InventoryItemUI con los datos
                InventoryItemUI uiScript = uiItem.GetComponent<InventoryItemUI>();
                if (uiScript != null)
                {
                    uiScript.Setup(itemIcons[i], itemNames[i], itemDescriptions[i]);
                }
            }
            else
            {
                // Si el slot está vacío, aseguramos que la referencia en 'slots' sea nula
                slots[i] = null;
            }

            Image slotImage = slotTransform.GetComponent<Image>();
            if (slotImage != null)
            {
                // Cambiamos el color según si el slot está seleccionado
                if (i == selectedSlotIndex)
                {
                    slotImage.color = new Color(1f, 1f, 1f, 1f);
                }
                else
                {
                    slotImage.color = new Color(0.7f, 0.7f, 0.7f, 1f);
                }
            }
        }
    }

    public void SelectSlot(int index)
    {
        selectedSlotIndex = index;
        UpdateDescriptionDisplay();
        UpdateInventoryUI();
    }

    private void UpdateDescriptionDisplay()
    {
        // Lógica para mostrar la descripción
        if (selectedSlotIndex >= 0 && selectedSlotIndex < num_slots_max && itemNames[selectedSlotIndex] != null)
        {
            descriptionImage.sprite = itemIcons[selectedSlotIndex];
            descriptionNameText.text = itemNames[selectedSlotIndex];
            descriptionText.text = itemDescriptions[selectedSlotIndex];

            // Aseguramos que los componentes de la descripción estén visibles
            descriptionImage.enabled = true;
            descriptionNameText.enabled = true;
            descriptionText.enabled = true;
            descriptionPanel.SetActive(true);
        }
        else
        {
            // Si no hay un slot seleccionado o el slot está vacío, ocultamos la imagen y el texto
            descriptionImage.enabled = false;
            descriptionNameText.enabled = false;
            descriptionText.enabled = false;
            descriptionPanel.SetActive(false);
        }
    }
    
    // Opcional: limpiar todo el inventario
    public void ClearInventory()
    {
        for (int i = 0; i < num_slots_max; i++)
        {
            itemNames[i] = null;
            itemIcons[i] = null;
            itemDescriptions[i] = null;
        }
        UpdateInventoryUI();
    }
}