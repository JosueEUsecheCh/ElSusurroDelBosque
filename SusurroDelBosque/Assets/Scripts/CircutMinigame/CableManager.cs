using UnityEngine;
using System.Collections.Generic;

public class CableManager : MonoBehaviour
{
    public GameObject lineRendererPrefab; // Prefab con un LineRenderer
    public LogicCircuit circuitManager;   // Referencia a LogicCircuit
    public RectTransform uiCanvas;        // Referencia al RectTransform del Canvas principal
    
    private CableSocket startSocket = null;
    private LineRenderer activeLine = null;

    // Diccionario para almacenar las conexiones lógicas (de entrada a slot)
    // Clave: Socket de inicio (Input A/B/C/D)
    // Valor: Socket de destino (GateInput1/2)
    private Dictionary<CableSocket, CableSocket> connections = new Dictionary<CableSocket, CableSocket>();

    void Update()
    {
        // Si hay un cable activo (flotante), actualiza su posición final al cursor
        if (activeLine != null)
        {
            Vector3 mousePos = Input.mousePosition;
            // Convertir la posición del mouse de pantalla a la posición local del Canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                uiCanvas, mousePos, null, out Vector2 localPoint);
            
            // Asigna la posición del punto final.
            activeLine.SetPosition(1, localPoint);
        }
    }

    public void SocketClicked(CableSocket clickedSocket)
    {
        // 1. Si NO hay un cable activo, INICIAMOS una nueva conexión
        if (startSocket == null)
        {
            StartNewConnection(clickedSocket);
        }
        // 2. Si ya hay un cable activo, COMPLETAMOS la conexión
        else
        {
            CompleteConnection(clickedSocket);
        }
    }

    private void StartNewConnection(CableSocket socket)
    {
        // Solo se puede empezar desde una entrada (A, B, C, D) o un GateInput que ya tenga una conexión.
        if (socket.type == CableSocket.SocketType.InputA || 
            socket.type == CableSocket.SocketType.InputB || 
            socket.type == CableSocket.SocketType.InputC || 
            socket.type == CableSocket.SocketType.InputD)
        {
            startSocket = socket;
            
            // Crear una nueva línea visual.
            GameObject lineObj = Instantiate(lineRendererPrefab, uiCanvas);
            activeLine = lineObj.GetComponent<LineRenderer>();
            
            // Asignar el punto de inicio de la línea.
            Vector3 startPos = startSocket.GetComponent<RectTransform>().localPosition;
            activeLine.SetPosition(0, startPos);
            activeLine.SetPosition(1, startPos); // Temporalmente, el final es el mismo que el inicio.
        }
        // AÑADIR lógica para desconectar cables existentes (tarea futura)
    }

    private void CompleteConnection(CableSocket endSocket)
    {
        // Solo se puede terminar en un GateInput (Entrada de compuerta)
        if (endSocket.type == CableSocket.SocketType.GateInput1 || 
            endSocket.type == CableSocket.SocketType.GateInput2)
        {
            // 1. COMPLETAR la conexión visual
            activeLine.SetPosition(1, endSocket.GetComponent<RectTransform>().localPosition);
            
            // 2. ALMACENAR la conexión LÓGICA
            connections[startSocket] = endSocket;
            
            // 3. ACTUALIZAR la lógica del circuito (Tarea futura: Crear un método de conexión en LogicCircuit)
            // circuitManager.ConnectInputToGate(startSocket.type, endSocket.gateSlotIndex, endSocket.type);
            
            // 4. Limpiar el estado
            activeLine = null;
            startSocket = null;
        }
        else // Si el jugador hace clic en otro Input o un punto no válido, cancelamos.
        {
            Destroy(activeLine.gameObject);
            activeLine = null;
            startSocket = null;
        }
    }
}