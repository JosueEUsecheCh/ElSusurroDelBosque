using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    // Variables de movimiento
    public float moveSpeed = 1f; // Velocidad de movimiento del personaje
    private Animator animator; // Referencia al componente Animator para controlar las animaciones
    private Vector2 movement; // Vector que almacena la dirección del movimiento (X, Y)
    private Rigidbody2D rb; // Referencia al componente Rigidbody2D para el movimiento físico

    // Variables del inventario
    [Header("Inventario")]
    public GameObject inventario_com; // Referencia al GameObject del inventario (normalmente un Canvas)
    private bool inventoryVisible = false; // Indica si el inventario está visible o no

    // Variables de control de movimiento
    private string currentAxis = ""; // Almacena el eje de movimiento actual para evitar movimiento diagonal
    private InventoryController inventoryController; // Referencia al script que gestiona el inventario
    private bool canMove = true; // Booleano para activar o desactivar el movimiento del jugador

    // Variables para el Cooldown
    [Header("Item Cooldown")]
    public float discardCooldown = 0.5f; // Tiempo de espera para soltar otro ítem
    private float lastDiscardTime = -1f; // Almacena el tiempo en que se soltó el último ítem
    
    void Start()
    {
        // Obtiene las referencias a los componentes necesarios en el mismo GameObject
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Desactiva el inventario al iniciar el juego si la referencia no es nula
        if (inventario_com != null)
        {
            inventario_com.SetActive(false);
        }
        
        // Busca y obtiene la referencia al script InventoryController
        inventoryController = GameObject.FindGameObjectWithTag("general-events").GetComponent<InventoryController>();
    }

    void Update()
    {
        // Obtiene la entrada del teclado para el movimiento
        float h = Input.GetAxisRaw("Horizontal"); // -1 (izquierda), 1 (derecha), 0 (sin pulsar)
        float v = Input.GetAxisRaw("Vertical"); // -1 (abajo), 1 (arriba), 0 (sin pulsar)

        // Lógica para el movimiento
        if (canMove)
        {
            // Evita el movimiento diagonal asegurando que solo un eje esté activo a la vez
            if (currentAxis == "")
            {
                if (h != 0) currentAxis = "Horizontal";
                else if (v != 0) currentAxis = "Vertical";
            }

            if (currentAxis == "Horizontal")
            {
                movement.x = h;
                movement.y = 0;
                if (h == 0) currentAxis = "";
                
            }
            else if (currentAxis == "Vertical")
            {
                movement.x = 0;
                movement.y = v;
                if (v == 0) currentAxis = "";
            }
            else
            {
                movement = Vector2.zero; // Detiene el movimiento si no se pulsa ninguna tecla
            }
            UpdateAnimations(); // Llama al método para actualizar las animaciones
        }
        else
        {
            // Si no puede moverse, detiene el movimiento y la animación
            movement = Vector2.zero;
            animator.SetBool("isWalking", false);
        }

        // Detección de entrada para el inventario
        if (Input.GetKeyUp(KeyCode.H) && inventario_com )
        {
            inventoryVisible = !inventoryVisible; // Invierte el estado de visibilidad
            if (inventoryVisible)
            {
                inventoryController.ShowInventory(); // Muestra el inventario
                canMove = false; // Desactiva el movimiento del jugador

            }
            else
            {
                HideInventoryAndEnableMovement(); // Oculta el inventario y activa el movimiento
                
            }
        }
        
        // Detección de entrada para soltar un ítem
        if (inventoryVisible && Input.GetKeyDown(KeyCode.M)&& Time.time > lastDiscardTime + discardCooldown)
        {
            if (inventoryController != null)
            {
                inventoryController.DiscardSelectedItem(); // Llama al método para descartar el ítem
                lastDiscardTime = Time.time; // Actualiza el tiempo del último descarte
                
            }
        }

        // Lógica para la navegación del inventario si está visible
        if (inventoryVisible)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                // Calcula el siguiente slot a la derecha (con un bucle que vuelve al inicio)
                int nextSlot = (inventoryController.selectedSlotIndex + 1 + 4) % 4;
                inventoryController.SelectSlot(nextSlot);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                // Calcula el slot anterior a la izquierda (con un bucle que vuelve al final)
                int prevSlot = (inventoryController.selectedSlotIndex - 1 + 4) % 4;
                inventoryController.SelectSlot(prevSlot);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                // Selecciona el slot de la fila de arriba (si existe)
                int prevRow = inventoryController.selectedSlotIndex - 2;
                if (prevRow >= 0)
                {
                    inventoryController.SelectSlot(prevRow);
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                // Selecciona el slot de la fila de abajo (si existe)
                int nextRow = inventoryController.selectedSlotIndex + 2;
                if (nextRow < 4)
                {
                    inventoryController.SelectSlot(nextRow);
                }
            }
        }
    }

    // Este método solo se llama cuando se cierra el inventario
    public void HideInventoryAndEnableMovement()
    {
        inventoryVisible = false; // Actualiza el estado de visibilidad del inventario
        inventoryController.HideInventory(); // Llama a la función del controlador para ocultar la UI
        canMove = true; // Activa el movimiento del jugador
    }

    // FixedUpdate se llama en intervalos de tiempo fijos, ideal para la física
    void FixedUpdate()
    {
        if (canMove)
        {
            // Mueve el Rigidbody2D a la nueva posición calculada
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    // Actualiza los parámetros del Animator para controlar las animaciones de movimiento
    void UpdateAnimations()
    {
        if (movement != Vector2.zero)
        {
            animator.SetBool("isWalking", true); // Activa la animación de caminar

            // Determina la dirección de la animación (arriba, abajo, izquierda, derecha)
            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
            {
                if (movement.x > 0)
                    animator.SetInteger("direction", 2); // Derecha
                else
                    animator.SetInteger("direction", 1); // Izquierda
            }
            else
            {
                if (movement.y > 0)
                    animator.SetInteger("direction", 3); // Arriba
                else
                    animator.SetInteger("direction", 0); // Abajo
            }
        }
        else
        {
            animator.SetBool("isWalking", false); // Desactiva la animación si el jugador no se mueve
        }
    }
    
    // Funciones de movimiento desde otros scripts
    public void DisableMovement()
    {
        canMove = false; // Desactiva el movimiento
        animator.SetBool("isWalking", false); // Detiene la animación de caminar
    }

    public void EnableMovement()
    {
        canMove = true; // Activa el movimiento
        animator.SetBool("isPickingUp", false); // Desactiva la animación de recoger
    }
}