using UnityEngine;
using UnityEngine.UI; // Necesario para la UI
using UnityEngine.EventSystems; // Necesario para detectar clics

public class CableSocket : MonoBehaviour, IPointerClickHandler
{
    // Define el tipo de conexión que es este punto.
    public enum SocketType { InputA, InputB, InputC, InputD, GateInput1, GateInput2 }
    public SocketType type;

    // Referencia al Manager de cables (se asignará en el Inspector)
    public CableManager cableManager; 

    // Referencia al LineRenderer que usará este socket (si es el inicio de un cable)
    [HideInInspector] public LineRenderer lineRenderer;
    
    // El índice de la compuerta a la que pertenece (si es GateInput). 0 si es una entrada A, B, C o D.
    public int gateSlotIndex = 0; 
    
    // Referencia a la compuerta padre (opcional, para GateInput)
    public GateSlot parentGate; 
    
    // Al hacer clic, notificamos al Manager.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (cableManager != null)
        {
            cableManager.SocketClicked(this);
        }
    }
}