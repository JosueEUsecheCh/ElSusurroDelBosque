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
        // Esto previene errores de referencia null
        slots = new GameObject[num_slots_max];
        itemNames = new string[num_slots_max];
        itemIcons = new Sprite[num_slots_max];
        itemDescriptions = new string[num_slots_max];
        itemPrefabNames = new string[num_slots_max];

        // Se asegura de que el inventario esté oculto al iniciar el juego.
        inventoryUI.SetActive(false);
        // Oculta el panel de descripción al inicio.
        UpdateDescriptionDisplay();

        // Busca el objeto del jugador por su etiqueta una sola vez al inicio para un mejor rendimiento.
        player = GameObject.FindGameObjectWithTag("Player");

    }

    // Muestra la interfaz de usuario del inventario.
    public void ShowInventory()
    {
        inventoryUI.SetActive(true);
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
        inventoryUI.SetActive(false);
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
                if (inventoryUI.activeInHierarchy)
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

    // Actualiza el panel de descripción con los datos del ítem seleccionado.
    private void UpdateDescriptionDisplay()
    {
        if (selectedSlotIndex >= 0 && selectedSlotIndex < num_slots_max && itemNames[selectedSlotIndex] != null)
        {
            // Muestra la imagen, nombre y descripción del ítem seleccionado.
            descriptionImage.sprite = itemIcons[selectedSlotIndex];
            descriptionNameText.text = itemNames[selectedSlotIndex];
            descriptionText.text = itemDescriptions[selectedSlotIndex];

            // Activa todos los elementos del panel de descripción.
            descriptionImage.enabled = true;
            descriptionNameText.enabled = true;
            descriptionText.enabled = true;
            descriptionPanel.SetActive(true);
        }
        else
        {
            // Oculta el panel de descripción si no hay un ítem seleccionado.
            descriptionImage.enabled = false;
            descriptionNameText.enabled = false;
            descriptionText.enabled = false;
            descriptionPanel.SetActive(false);
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

            //Cierra el inventario.
            HideInventory();

            //Reactiva el movimiento del jugador.
            if (player != null)
            {
                FindFirstObjectByType<PlayerMovement>().EnableMovement();
                player.GetComponent<PlayerMovement>().HideInventoryAndEnableMovement();
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
        if (player != null)
        {
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
}