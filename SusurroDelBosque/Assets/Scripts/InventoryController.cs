using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;

public class InventoryController : MonoBehaviour
{
    // Arrays para las referencias a los slots de UI en la jerarquía.
    public GameObject[] slots;
    // Define el número máximo de slots que tendrá el inventario.
    private int num_slots_max = 4;

    [Header("UI Prefab")]
    // Prefab del objeto que representa un ítem en la UI del inventario.
    public GameObject itemUIPrefab;
    // El transform del contenedor de los slots de la UI del inventario.
    public Transform inventorySlotsUI;
    // El índice del slot que está actualmente seleccionado por el jugador. -1 significa ninguno.
    public int selectedSlotIndex = -1;
    
    // <--- REINCORPORADO: ESTO ES LO QUE FALTABA O ELIMINASTE --->
    [HideInInspector] public string selectedGateToPlace = ""; 
    // <----------------------------------------------------------->

    // Arrays para almacenar los datos de los ítems recogidos. La información de cada ítem se guarda en el mismo índice en todos los arrays.
    private string[] itemNames;
    private Sprite[] itemIcons;
    private string[] itemDescriptions;

    // Referencias a los componentes de la UI para mostrar la descripción de los ítems.
    public Image descriptionImage;
    public Text descriptionNameText;
    public Text descriptionText;
    public GameObject descriptionPanel;

    // Referencia al GameObject principal de la UI del inventario para activarlo/desactivarlo.
    public GameObject inventoryUI;
    // Este array guarda el nombre del prefab físico asociado a cada ítem en el inventario.
    private string[] itemPrefabNames;

    // Referencia al jugador, se usa para saber dónde instanciar los objetos físicos.
    private GameObject player;


    void Start()
    {
        // Inicializa todos los arrays con el tamaño máximo de slots.
        slots = new GameObject[num_slots_max];
        itemNames = new string[num_slots_max];
        itemIcons = new Sprite[num_slots_max];
        itemDescriptions = new string[num_slots_max];
        itemPrefabNames = new string[num_slots_max];

        // Se asegura de que el inventario esté oculto al iniciar el juego.
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
        }
        
        // Oculta el panel de descripción al inicio.
        UpdateDescriptionDisplay();

        // Busca el objeto del jugador por su etiqueta una sola vez al inicio para un mejor rendimiento.
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Muestra la interfaz de usuario del inventario.
    public void ShowInventory()
    {
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(true);
        }
        
        // Busca el primer slot que contenga un ítem para seleccionarlo por defecto.
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
    }

    // Oculta la interfaz de usuario del inventario.
    public void HideInventory()
    {
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
        }
        
        // Deselecciona el slot actual.
        SelectSlot(-1);
    }

    // Añade un ítem al inventario. Se llama desde el script que detecta la recogida de objetos.
    public void AddItem(string name, Sprite icon, string description, string prefabName)
    {
        for (int i = 0; i < num_slots_max; i++)
        {
            // Busca el primer slot vacío.
            if (itemNames[i] == null)
            {
                // Guarda la información del nuevo ítem en el slot encontrado.
                itemNames[i] = name;
                itemIcons[i] = icon;
                itemDescriptions[i] = description;
                itemPrefabNames[i] = prefabName;

                // Si el inventario está abierto, actualiza la UI para mostrar el ítem recién añadido.
                if (inventoryUI != null && inventoryUI.activeInHierarchy)
                {
                    SelectSlot(i);
                }

                break;
            }
        }
    }

    // Actualiza la visualización de los slots del inventario.
    public void UpdateInventoryUI()
    {
        for (int i = 0; i < num_slots_max; i++)
        {
            Transform slotTransform = inventorySlotsUI.GetChild(i);
            Image slotImage = slotTransform.GetComponent<Image>();

            // Si ya hay un ítem de UI en el slot, lo destruye para evitar duplicados.
            if (slotTransform.childCount > 0)
            {
                Destroy(slotTransform.GetChild(0).gameObject);
            }

            // Si el slot tiene un ítem, instancia un nuevo ítem de UI y lo configura.
            if (itemNames[i] != null)
            {
                GameObject uiItem = Instantiate(itemUIPrefab, slotTransform);
                uiItem.transform.localPosition = Vector3.zero;
                slots[i] = uiItem;

                InventoryItemUI uiScript = uiItem.GetComponent<InventoryItemUI>();
                if (uiScript != null)
                {
                    uiScript.Setup(itemIcons[i], itemNames[i], itemDescriptions[i]);
                }
            }
            else
            {
                // Si el slot está vacío, asegura que la referencia al slot esté nula.
                slots[i] = null;
            }

            // Cambia el color del fondo del slot para indicar si está seleccionado o no.
            if (slotImage != null)
            {
                if (i == selectedSlotIndex)
                {
                    slotImage.color = new Color(1f, 1f, 1f, 1f); // Color blanco para el seleccionado
                }
                else
                {
                    slotImage.color = new Color(0.7f, 0.7f, 0.7f, 1f); // Color gris para los demás
                }
            }
        }
    }

    // Selecciona un slot específico por su índice y actualiza la UI.
    public void SelectSlot(int index)
    {
        // Se asegura de que el índice sea válido (entre 0 y el número máximo de slots).
        if (index < 0 || index >= num_slots_max)
        {
            selectedSlotIndex = -1; // Deselecciona si el índice no es válido.
        }
        else
        {
            selectedSlotIndex = index;
        }

        // Actualiza el panel de descripción y la visualización de los slots.
        UpdateDescriptionDisplay();
        UpdateInventoryUI();
    }
    
    // <--- REINCORPORADO --->
    public string GetSelectedItemName()
    {
        if (selectedSlotIndex >= 0 && selectedSlotIndex < itemNames.Length)
        {
            return itemNames[selectedSlotIndex];
        }
        return null;
    }
        // Método que consume el ítem seleccionado SIN soltarlo físicamente (usado por GateSlot.cs al colocar).
    public void RemoveSelectedItem()
    {
        if (selectedSlotIndex != -1 && itemNames[selectedSlotIndex] != null)
        {
            // 1. Limpia los datos del slot
            itemNames[selectedSlotIndex] = null;
            itemIcons[selectedSlotIndex] = null;
            itemDescriptions[selectedSlotIndex] = null;
            itemPrefabNames[selectedSlotIndex] = null;
            slots[selectedSlotIndex] = null;
            
            // 2. Busca el siguiente ítem para seleccionar o deselecciona.
            int newSelectedIndex = -1;
            for (int i = 0; i < num_slots_max; i++)
            {
                if (itemNames[i] != null)
                {
                    newSelectedIndex = i;
                    break;
                }
            }
            
            // 3. Deselecciona y actualiza la UI
            SelectSlot(newSelectedIndex); 
        }
    }
    
    // Método para devolver un ítem al inventario (usado por GateSlot.cs al remover).
    public void ReturnItem(string name, Sprite icon, string description, string prefabName)
    {
        AddItem(name, icon, description, prefabName);
    }

    // <--- REINCORPORADO --->
    public void UpdateGateSelection()
    {
        string selectedItem = GetSelectedItemName();

        // Verifica si el ítem es una de las compuertas lógicas
        if (selectedItem != null && (selectedItem.Contains("Gate_AND") || selectedItem.Contains("Gate_OR") || selectedItem.Contains("Gate_NOT")))
        {
            selectedGateToPlace = selectedItem;
        }
        else
        {
            // Si no es una compuerta, se borra el estado de colocación
            selectedGateToPlace = "";
        }
    }


    // Actualiza el panel de descripción con los datos del ítem seleccionado.
    private void UpdateDescriptionDisplay()
    {
        if (selectedSlotIndex >= 0 && selectedSlotIndex < num_slots_max && itemNames[selectedSlotIndex] != null)
        {
            // Muestra la imagen, nombre y descripción del ítem seleccionado.
            // Asegúrate de que descriptionImage, descriptionNameText y descriptionText no son null
            if (descriptionImage != null) descriptionImage.sprite = itemIcons[selectedSlotIndex];
            if (descriptionNameText != null) descriptionNameText.text = itemNames[selectedSlotIndex];
            if (descriptionText != null) descriptionText.text = itemDescriptions[selectedSlotIndex];

            // Activa todos los elementos del panel de descripción.
            if (descriptionImage != null) descriptionImage.enabled = true;
            if (descriptionNameText != null) descriptionNameText.enabled = true;
            if (descriptionText != null) descriptionText.enabled = true;
            if (descriptionPanel != null) descriptionPanel.SetActive(true);
        }
        else
        {
            // Oculta el panel de descripción si no hay un ítem seleccionado.
            if (descriptionImage != null) descriptionImage.enabled = false;
            if (descriptionNameText != null) descriptionNameText.enabled = false;
            if (descriptionText != null) descriptionText.enabled = false;
            if (descriptionPanel != null) descriptionPanel.SetActive(false);
        }
    }

    // Suelta el ítem del slot seleccionado y lo instancia en el mundo.
    public void DiscardSelectedItem()
    {
        // Verifica que haya un slot seleccionado y que no esté vacío.
        if (selectedSlotIndex != -1 && itemNames[selectedSlotIndex] != null)
        {
            // Obtiene el nombre del prefab físico a instanciar.
            string discardedItemName = itemPrefabNames[selectedSlotIndex];

            // Instancia el objeto físico.
            InstantiatePhysicalItem(discardedItemName);

            //Limpia los datos del slot en los arrays.
            itemNames[selectedSlotIndex] = null;
            itemIcons[selectedSlotIndex] = null;
            itemDescriptions[selectedSlotIndex] = null;
            itemPrefabNames[selectedSlotIndex] = null; // Limpiar también el nombre del prefab
            slots[selectedSlotIndex] = null;

            //Busca el siguiente ítem para seleccionar o deselecciona.
            int newSelectedIndex = -1;
            for (int i = 0; i < num_slots_max; i++)
            {
                if (itemNames[i] != null)
                {
                    newSelectedIndex = i;
                    break;
                }
            }
            SelectSlot(newSelectedIndex);

            // <--- REINCORPORADO: Limpiar la selección de compuertas después de soltar un ítem. --->
            selectedGateToPlace = "";

            //Cierra el inventario.
            HideInventory();

            //Reactiva el movimiento del jugador.
            if (player != null)
            {
                // Usar el método que maneja el cierre de inventario y movimiento
                PlayerMovement pm = player.GetComponent<PlayerMovement>();
                if (pm != null)
                {
                     pm.HideInventoryAndEnableMovement();
                }
            }
        }
    }

    // Instancia el objeto físico en el mundo cargándolo desde la carpeta Resources.
    private void InstantiatePhysicalItem(string prefabName)
    {
        // Verifica si el nombre del prefab es válido antes de intentar cargarlo. Se realizo para pruebas 
        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError("Error: El nombre del prefab está vacío.");
            return;
        }

        // Carga el prefab por su nombre desde cualquier carpeta 'Resources' en el proyecto. Esto es uan funcion de UNITY
        GameObject prefabToInstantiate = Resources.Load<GameObject>(prefabName);

        // Si el jugador existe, instancia el objeto físico cerca de él.
        if (player != null && prefabToInstantiate != null)
        {
            // Instancia ligeramente delante del jugador
            Vector3 spawnPosition = player.transform.position + new Vector3(0.5f, 0.5f, 0f); 
            Instantiate(prefabToInstantiate, spawnPosition, Quaternion.identity);
        }
    }

    // Limpia el inventario eliminando todos los ítems.
    public void ClearInventory()
    {
        for (int i = 0; i < num_slots_max; i++)
        {
            itemNames[i] = null;
            itemIcons[i] = null;
            itemDescriptions[i] = null;
            itemPrefabNames[i] = null;
        }
        UpdateInventoryUI();
    }
    
    public bool HasSpace()
    {
        // Recorre los slots y devuelve true tan pronto como encuentre uno vacío.
        for (int i = 0; i < num_slots_max; i++)
        {
            if (itemNames[i] == null)
            {
                return true; // Se encontró un slot vacío, hay espacio.
            }
        }
        return false; // No se encontraron slots vacíos, el inventario está lleno.
    }
    
    // Método para verificar la existencia de un ítem
    public bool ContainsItem(string itemName)
    {
        for (int i = 0; i < itemNames.Length; i++)
        {
            // Ajuste defensivo: verifica que el slot no sea nulo antes de comparar
            if (itemNames[i] != null && itemNames[i] == itemName)
            {
                return true;
            }
        }
        return false;
    }
}