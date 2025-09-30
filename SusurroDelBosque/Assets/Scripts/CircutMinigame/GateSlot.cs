using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GateSlot : MonoBehaviour, IPointerClickHandler
{
    public int slotIndex; // Asignar 1, 2, 3 o 4 en el Inspector
    
    // Asignar en el Inspector (ElectricBox)
    public LogicCircuit circuitManager; 
    
    // Ya no es pública para ser asignada en el Inspector, sino buscada en código.
    private InventoryController inventoryController; 
    
    private Image gateImage; 

    // --- Variables para almacenar los datos del ítem colocado (NECESARIO para devolverlo) ---
    private string placedGateName = null;
    private Sprite placedGateIcon = null;
    private string placedGateDescription = null;
    private string placedGatePrefabName = null;

    void Start()
    {
        // Busca el componente Image en sí mismo.
        gateImage = GetComponent<Image>(); 

        // 1. INTENTO DE ENCONTRAR EL INVENTORY CONTROLLER
        if (inventoryController == null)
        {
            // Busca por nombre del objeto (asumiendo que se llama "GeneralEvents"
            // Nota: Si usas una etiqueta "general-events", el código sería:
            GameObject invObj = GameObject.FindGameObjectWithTag("general-events");
            
            //GameObject invObj = GameObject.Find("GeneralEvents"); 
            if (invObj != null)
            {
                inventoryController = invObj.GetComponent<InventoryController>();
            }
        } 
        
        // 2. CHEQUEOS DE ERRORES
        if (circuitManager == null) Debug.LogError("GateSlot: Missing LogicCircuit. Asigna ElectricBox.");
        if (inventoryController == null) Debug.LogError("GateSlot: Missing InventoryController. No se encontró 'GeneralEvents'.");
        
        // Inicializa el visual del slot como vacío
        UpdateVisual(LogicCircuit.GateType.None);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (circuitManager == null || inventoryController == null) return;
        
        // --- A. Intento de Remover (Clic Derecho) ---
        if (eventData.button == PointerEventData.InputButton.Right) 
        {
            RemoveGate();
            return;
        }
        
        // --- B. Intento de Colocar (Clic Izquierdo) ---
        
        // 1. Obtener la compuerta seleccionada
        string selectedItemName = inventoryController.GetSelectedItemName();
        if (string.IsNullOrEmpty(selectedItemName) || !IsGateItem(selectedItemName)) return; 

        // 2. Convertir el nombre a tipo de compuerta
        LogicCircuit.GateType gateToPlace = ParseGateType(selectedItemName);
        
        // 3. Chequear pre-condiciones de colocación (si el slot no está vacío)
        if (GetGateInSlot() != LogicCircuit.GateType.None)
        {
            Debug.Log("Slot " + slotIndex + " ya está ocupado.");
            return; 
        }
        
        // 4. Colocar la compuerta
        
        // Antes de consumir, guardar los datos de la pieza para poder devolverla después.
        SavePlacedGateData(selectedItemName, gateToPlace);
        
        // Remover del inventario el ítem
        inventoryController.RemoveSelectedItem(); 
        
        // Insertar en el circuito lógico (esto llama a CalculateCircuit)
        circuitManager.InsertGate(slotIndex, gateToPlace);
        
        // Actualizar el visual del slot
        UpdateVisual(gateToPlace);
    }
    
    private void RemoveGate()
    {
        LogicCircuit.GateType currentGate = GetGateInSlot();
        
        if (currentGate != LogicCircuit.GateType.None)
        {
            // 1. Devolver la pieza al inventario usando los datos guardados
            if (placedGateName != null && inventoryController != null)
            {
                // NOTA: Asumimos que AddItem en InventoryController puede manejar nulls 
                // para la descripción y prefabName si solo son necesarios el nombre e ícono.
                inventoryController.ReturnItem(
                    placedGateName, 
                    placedGateIcon, 
                    placedGateDescription, 
                    placedGatePrefabName
                );
            }
            
            // 2. Notificar al circuito de la remoción
            circuitManager.RemoveGate(slotIndex);
            
            // 3. Limpiar los datos guardados de la pieza
            placedGateName = null;
            placedGateIcon = null;
            placedGateDescription = null;
            placedGatePrefabName = null;
            
            // 4. Actualizar el visual del slot
            UpdateVisual(LogicCircuit.GateType.None);
        }
    }
    
    // Nuevo método para guardar los detalles de la pieza antes de que sea consumida
    private void SavePlacedGateData(string itemName, LogicCircuit.GateType gateType)
    {
        // Esto es una simplificación. Un sistema robusto usaría un script de ítem o una base de datos.
        
        // Nombre del ítem (ej: "Gate_AND")
        placedGateName = itemName;
        
        // Ícono (obtenido del LogicCircuit)
        placedGateIcon = circuitManager.GetGateSprite(gateType);
        
        // La descripción y el nombre del prefab físico son más difíciles de obtener aquí, 
        // pero para evitar errores de null, los inicializamos. 
        // Idealmente, InventoryController nos daría estos datos antes de consumir el ítem.
        // Por ahora, usamos el nombre como valor de marcador de posición.
        placedGateDescription = "Compuerta lógica: " + gateType.ToString();
        placedGatePrefabName = itemName; // Asumimos que el nombre del prefab es el mismo que el nombre del ítem.
    }
    
    // Obtiene el tipo de compuerta actualmente en este slot
    private LogicCircuit.GateType GetGateInSlot()
    {
        switch (slotIndex)
        {
            case 1: return circuitManager.slot1;
            case 2: return circuitManager.slot2;
            case 3: return circuitManager.slot3;
            case 4: return circuitManager.slot4;
            default: return LogicCircuit.GateType.None;
        }
    }

    // Actualiza el sprite visible
    private void UpdateVisual(LogicCircuit.GateType gate)
    {
        if (gateImage != null)
        {
            Sprite sprite = circuitManager.GetGateSprite(gate);
            gateImage.sprite = sprite;
            //gateImage.enabled = (sprite != null); 
            gateImage.enabled = true;
        }
    }

    // Métodos auxiliares
    private bool IsGateItem(string itemName)
    {
        return itemName.Contains("AND") || itemName.Contains("OR") || itemName.Contains("NOT");
    }

    private LogicCircuit.GateType ParseGateType(string itemName)
    {
        if (itemName.Contains("AND")) return LogicCircuit.GateType.AND;
        if (itemName.Contains("OR")) return LogicCircuit.GateType.OR;
        if (itemName.Contains("NOT")) return LogicCircuit.GateType.NOT;
        return LogicCircuit.GateType.None;
    }
}