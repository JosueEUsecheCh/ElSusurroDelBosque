using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputSwitch : MonoBehaviour, IPointerClickHandler
{
    // Asigna el objeto ElectricBox con el script LogicCircuit en el Inspector
    public LogicCircuit circuitManager;
    
    private Image switchImage;
    
    // Sprites para el estado ON (True) y OFF (False)
    public Sprite spriteON;
    public Sprite spriteOFF;

    void Start()
    {
        switchImage = GetComponent<Image>();
        
        if (circuitManager == null)
        {
            Debug.LogError("InputSwitch: LogicCircuit no asignado. Asigna el ElectricBox.");
            return;
        }
        
        // Inicializa el visual con el estado actual de inputC
        UpdateVisual(circuitManager.inputC);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("¡CLIC DETECTADO en Input C! Nuevo estado: " + !circuitManager.inputC);
        if (circuitManager == null) return;

        // 1. Invierte el estado de la entrada C
        bool newState = !circuitManager.inputC;
        circuitManager.inputC = newState;

        // 2. Actualiza el visual del interruptor
        UpdateVisual(newState);

        // 3. Recalcula el circuito con el nuevo valor de C
        circuitManager.CalculateCircuit();
    }

    private void UpdateVisual(bool state)
    {
        if (switchImage != null)
        {
            // Cambia el sprite según el estado (ON/OFF)
            if (state && spriteON != null)
            {
                switchImage.sprite = spriteON;
                switchImage.color = Color.green; // O algún color que indique ON
            }
            else if (!state && spriteOFF != null)
            {
                switchImage.sprite = spriteOFF;
                switchImage.color = Color.red; // O algún color que indique OFF
            }
            // Si no hay sprites, usa un color base
            else
            {
                switchImage.color = state ? Color.green : Color.red;
            }
        }
    }
}