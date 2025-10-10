using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public class CircuitPanelTrigger : MonoBehaviour
{
    public GameObject circuitPanelUI;
    public string requiredItemName = "Destornillador";

    private InventoryController inventory;
    private PlayerMovement playerMovement;
    private bool isInRange = false; // Nueva variable para saber si el jugador está en rango

    [Header("UI Button Reference")]
// Variable para el nombre del botón que buscas
    public string buttonNName = "Button_N"; 
    
// Variable para almacenar la referencia al componente Button
    private Button buttonN; 

    void Start()
    {
        // ... (Tu lógica de Start() para obtener referencias) ...
        GameObject eventsObject = GameObject.FindGameObjectWithTag("general-events");
        if (eventsObject != null)
        {
            inventory = eventsObject.GetComponent<InventoryController>();
        }
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
        }

        if (inventory == null) Debug.LogError("Error: No se pudo encontrar el InventoryController.");
        if (playerMovement == null) Debug.LogError("ERROR CRÍTICO: No se pudo encontrar el componente PlayerMovement.");
        if (circuitPanelUI != null) circuitPanelUI.SetActive(false);
        
        
        GameObject buttonNObject = GameObject.Find(buttonNName); 
        if (buttonNObject != null)
        {
            // 2. Obtener el componente Button
            buttonN = buttonNObject.GetComponent<Button>();

            if (buttonN != null)
            {
                // 3. ASIGNACIÓN DINÁMICA: Añadir el listener
                buttonN.onClick.AddListener(OnOpenPanelButtonPressed);
                Debug.Log($"Botón '{buttonNName}' conectado dinámicamente al CircuitPanelTrigger.");
            }
            else
            {
                Debug.LogError($"El GameObject '{buttonNName}' no tiene un componente Button.");
            }
        }
        else
        {
            Debug.LogError($"No se encontró el botón '{buttonNName}' en la escena.");
        }
    
        

    }

    void Update()
    {
        // 1. Detección de CIERRE del panel (funciona siempre, incluso sin movimiento)
        if (circuitPanelUI != null && circuitPanelUI.activeSelf && Input.GetKeyDown(KeyCode.N))
        {
            // Solo se permite cerrar si el panel está abierto.
            CloseCircuitPanel();
            return;
        }

        // 2. Detección de APERTURA (solo si está en rango y no hay otro panel/inventario abierto)
        if (isInRange && !circuitPanelUI.activeSelf && Input.GetKeyDown(KeyCode.N))
        {
            AttemptToOpenPanel();
        }
    }

    private void AttemptToOpenPanel()
    {
        // 1. Comprobamos si el inventario está abierto (bloquea la apertura del panel)
        if (inventory != null && inventory.inventoryUI != null && inventory.inventoryUI.activeSelf)
        {
            Debug.Log("Caja: No se puede abrir, el inventario está abierto.");
            return;
        }

        // 2. Comprobamos si tiene el ítem requerido
        if (inventory != null && inventory.ContainsItem(requiredItemName))
        {
            // Lógica de apertura exitosa
            circuitPanelUI.SetActive(true);
            if (playerMovement != null)
            {
                playerMovement.DisableMovement();
                playerMovement.SetCircuitPanelState(true);
            }
        }
        else
        {
            // Lógica de ítem requerido no encontrado
            Debug.Log("Necesitas un " + requiredItemName + " para abrir este panel.");
        }
    }
    public void OnOpenPanelButtonPressed()
    {
        if (circuitPanelUI != null && circuitPanelUI.activeSelf)
        {
            CloseCircuitPanel();
        } 
        // Solo comprueba si está en rango ANTES de intentar abrir.
        else if (isInRange)
        {
            AttemptToOpenPanel();
        }
        else
        {
            Debug.Log("Caja: El jugador no está en rango para interactuar.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            Debug.Log("Jugador en rango de la caja.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            if(circuitPanelUI != null && circuitPanelUI.activeSelf)
            Debug.Log("Jugador fuera de rango de la caja.");
        }
    }

    // Método que se llama para cerrar el panel (incluye el llamado a un botón 'X' en la UI).
    public void CloseCircuitPanel()
    {
        if (circuitPanelUI != null)
        {
            circuitPanelUI.SetActive(false);
        }
        
        // Habilitar el movimiento del jugador, PERO solo si no hay otro sistema (como el inventario) abierto.
        if (playerMovement != null)
        {
            playerMovement.SetCircuitPanelState(false); // <--- NUEVO: Desbloquea el estado
            playerMovement.EnableMovement(); // Se llamará solo si ningún otro panel lo impide.
        }
    }
}