using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public GameObject[] slots;            // Slots lógicos
    private int num_slots_max = 4; // Numero de slots maximos en el inventario

    [Header("UI Prefab")]
    public GameObject itemUIPrefab;       // Prefab genérico (Button)
    public Transform inventorySlotsUI;    // Contenedor de los slots en la UI
    public GameObject[] uiSlots; // Añade una referencia a los gameobjects de los slots en la UI
    public int selectedSlotIndex = -1;

    // Datos de los items recogidos
    private string[] itemNames;
    private Sprite[] itemIcons;

    void Start()
    {
        slots = new GameObject[num_slots_max];
        itemNames = new string[num_slots_max];
        itemIcons = new Sprite[num_slots_max];
    }

    // Método para añadir un item recogido
    public void AddItem(string name, Sprite icon)
    {
        for (int i = 0; i < num_slots_max; i++)
        {
            if (itemNames[i] == null)
            {
                itemNames[i] = name;
                itemIcons[i] = icon;
                UpdateInventoryUI();
                break;
            }
        }
    }

    // Actualiza la UI del inventario usando el prefab genérico
    public void UpdateInventoryUI()
    {
        for (int i = 0; i < num_slots_max; i++)
        {
            Transform slot = inventorySlotsUI.GetChild(i);

            // Limpiar slot
            if (slot.childCount > 0)
                Destroy(slot.GetChild(0).gameObject);

            // Si hay item, instanciar prefab genérico y configurar
            if (itemNames[i] != null)
            {
                GameObject uiItem = Instantiate(itemUIPrefab, slot);
                uiItem.transform.localPosition = Vector3.zero;

                // Configurar el prefab genérico con nombre y sprite
                InventoryItemUI uiScript = uiItem.GetComponent<InventoryItemUI>();
                if (uiScript != null)
                {
                    uiScript.Setup(itemIcons[i], itemNames[i]);
                }
            }
            Image slotImage = slot.GetComponent<Image>();
            if (slotImage != null)
            {
                // Cambia el color del slot si está seleccionado
                if (i == selectedSlotIndex)
                {
                    slotImage.color = new Color(1f, 1f, 1f, 1f); // Un color más brillante, por ejemplo blanco
                }
                else
                {
                    slotImage.color = new Color(0.7f, 0.7f, 0.7f, 1f); // Un color más opaco
                }
            }

        }
    }

    public void SelectSlot(int index)
    {
        if (index >= 0 && index < num_slots_max)
        {
            selectedSlotIndex = index;
            UpdateInventoryUI();
        }
    }

    // Opcional: limpiar todo el inventario
    public void ClearInventory()
    {
        for (int i = 0; i < num_slots_max; i++)
        {
            itemNames[i] = null;
            itemIcons[i] = null;
        }
        UpdateInventoryUI();
    }
}
