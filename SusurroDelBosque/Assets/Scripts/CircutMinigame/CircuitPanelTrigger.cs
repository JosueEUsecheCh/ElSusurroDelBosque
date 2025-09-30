using UnityEngine;

public class CircuitPanelTrigger : MonoBehaviour
{
    public GameObject circuitPanelUI;
    public string requiredItemName = "Destornillador";

    private InventoryController inventory;
    private PlayerMovement playerMovement;
    private bool isInRange = false; // Nueva variable para saber si el jugador está en rango

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
            // Se comprueba si el inventario está abierto ANTES de abrir el panel de circuitos
            if (inventory != null && inventory.inventoryUI != null && inventory.inventoryUI.activeSelf)
            {
                Debug.Log("Caja: No se puede abrir, el inventario está abierto.");
                return;
            }
            
            Debug.Log("Tecla N detectada para abrir.");

            if (inventory != null && inventory.ContainsItem(requiredItemName))
            {
                circuitPanelUI.SetActive(true);
                if (playerMovement != null)
                {
                    playerMovement.DisableMovement();
                    playerMovement.SetCircuitPanelState(true); // <--- NUEVO: Bloquea reactivación del movimiento por Inventario
                }
            }
            else
            {
                Debug.Log("Necesitas un " + requiredItemName + " para abrir este panel.");
            }
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