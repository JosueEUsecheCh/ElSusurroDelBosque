using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; 

public class LogicCircuit : MonoBehaviour
{
    // --- Entradas Fijas (Inputs) ---
    private const bool INPUT_A = true; 
    private const bool INPUT_B = true; 
    private const bool INPUT_D = false; // D siempre es False (0)

    // C es variable y se maneja por el interruptor de UI.
    public bool inputC = false; 

    // --- Slots para las Compuertas (Piezas Coleccionables) ---
    // GateType: Ninguna, AND, OR, NOT
    public enum GateType { None, AND, OR, NOT }

    [Header("Slots para Compuertas Insertables")]
    public GateType slot1 = GateType.None; // Requerido: AND (Salida Cocina)
    public GateType slot2 = GateType.None; // Requerido: OR (Salida Sala/Estudio)
    public GateType slot3 = GateType.None; // Dilema NOT: Usado por 2Piso si tiene el NOT
    public GateType slot4 = GateType.None; // Dilema NOT: Usado por Exterior si tiene el NOT

    [Header("Visuales de Compuertas")]
    public Sprite andSprite;
    public Sprite orSprite;
    public Sprite notSprite;
    
    // Asignar en Inspector los visuales de luz
    [Header("Salidas de Luz")]
    public Image lightCocina; // Correspondiente a lightC
    public Image lightSalaEstudio; // Correspondiente a lightS
    public Image light2Piso;
    public Image lightExterior;
    
    // [NUEVO] Referencia a la UI para actualización
    public GameObject circuitPanelUI; // Para saber si el panel está activo

    // Método auxiliar para obtener el sprite
    public Sprite GetGateSprite(GateType type)
    {
        switch (type)
        {
            case GateType.AND: return andSprite;
            case GateType.OR: return orSprite;
            case GateType.NOT: return notSprite;
            default: return null;
        }
    }

    // Método para insertar una pieza en un slot
    public void InsertGate(int slotIndex, GateType gate)
    {
        // 1. Lógica XOR (Dilema del NOT)
        if (gate == GateType.NOT)
        {
            if (slotIndex == 3) // Si pongo NOT en Slot 3
            {
                slot4 = GateType.None; // Vacío Slot 4
            }
            else if (slotIndex == 4) // Si pongo NOT en Slot 4
            {
                slot3 = GateType.None; // Vacío Slot 3
            }
        }
        
        // 2. Insertar la compuerta
        switch (slotIndex)
        {
            case 1: slot1 = gate; break;
            case 2: slot2 = gate; break;
            case 3: slot3 = gate; break;
            case 4: slot4 = gate; break;
            default: Debug.LogError("Slot Index fuera de rango: " + slotIndex); break;
        }
        
        CalculateCircuit(); 
    }

    // Método para remover una pieza (simplemente asigna None)
    public void RemoveGate(int slotIndex)
    {
        InsertGate(slotIndex, GateType.None);
    }
    
    // El método principal para calcular el circuito
    public void CalculateCircuit()
    {
        // --- 1. CÁLCULO DE SALIDAS INTERMEDIAS (SLOTS VARIABLES) ---
        
        bool output_Cocina = (slot1 == GateType.AND) ? (INPUT_A && INPUT_B) : false;
        bool output_SalaEstudio = (slot2 == GateType.OR) ? (INPUT_B || inputC) : false;
        
        // El resultado del NOT se calcula solo una vez si está presente.
        bool notResult = (slot3 == GateType.NOT || slot4 == GateType.NOT) ? !INPUT_D : false;
        
        // --- 2. CÁLCULO DE CABLEADO DEL DILEMA (SLOTS 3 Y 4) ---
        
        // Lógica para 2Piso: Si NOT está en Slot 3, usa NOT D. Si no, usa D sin invertir (INPUT_D).
        bool input_to_2Piso_from_D = (slot3 == GateType.NOT) ? notResult : INPUT_D;

        // Lógica para Exterior: Si NOT está en Slot 4, usa NOT D. Si no, usa D sin invertir (INPUT_D).
        bool input_to_Exterior_from_D = (slot4 == GateType.NOT) ? notResult : INPUT_D;

        // --- 3. CÁLCULO DE SALIDAS FIJAS (LÓGICA INTERNA) ---
        
        // OR FIJO (Cableado interno del 2Piso): A OR C
        bool output_OR_Fijo_2Piso = INPUT_A || inputC; 

        // AND FIJO de 2Piso: (OR Fijo) AND (NOT D o D)
        bool output_2Piso_Final = output_OR_Fijo_2Piso && input_to_2Piso_from_D; 

        // AND FIJO de Exterior: B AND (NOT D o D)
        bool output_Exterior_Final = INPUT_B && input_to_Exterior_from_D; 
        
        // --- 4. ACTUALIZACIÓN DE VISUALES ---
        
        // Solo actualiza si el panel está abierto (para evitar errores al inicio)
        if (circuitPanelUI.activeSelf) 
        {
            SetLightState(lightCocina, output_Cocina); 
            SetLightState(lightSalaEstudio, output_SalaEstudio);
            SetLightState(light2Piso, output_2Piso_Final); 
            SetLightState(lightExterior, output_Exterior_Final); 
        }
    }

    public void SetLightState(UnityEngine.UI.Image lightImage, bool state) 
    {
        // AÑADIR: Solo procede si la referencia a la Image NO es nula.
        if (lightImage != null)
        {
            if (state)
            {
                lightImage.color = Color.yellow; // Luz encendida
                // lightImage.sprite = LightOnSprite; // (Si usas sprites)
            }
            else
            {
                lightImage.color = Color.gray; // Luz apagada
                // lightImage.sprite = LightOffSprite; // (Si usas sprites)
            }
            // lightImage.enabled = true; // Asegura que la imagen esté visible
        }
        // Si lightImage es null, el script simplemente ignora la llamada y NO falla.
    }
}