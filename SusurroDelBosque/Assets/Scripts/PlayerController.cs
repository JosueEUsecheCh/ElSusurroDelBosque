using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    private Animator animator;
    private Vector2 movement;
    private Rigidbody2D rb;

    [Header("Inventario")]
    public GameObject inventario_com;   
    private bool inventoryVisible = false;

    private string currentAxis = ""; // "Horizontal", "Vertical" o "" significa que está quieto. Variable de prioridad del movimiento

    private InventoryController inventoryController; // Valariable para controlar la preseleccion 

    private bool canMove = true; // Variable para controlar el movimiento


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Ocultamos el inventario al inicio
        if (inventario_com != null)
        {
            inventario_com.SetActive(false);
        }
        else
        {
            inventario_com = null;
        }
        inventoryController = GameObject.FindGameObjectWithTag("general-events").GetComponent<InventoryController>(); 
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Detectar qué eje se presionó primero
        if (canMove)
        {
            if (currentAxis == "")
            {
                if (h != 0) currentAxis = "Horizontal";
                else if (v != 0) currentAxis = "Vertical";
            }

            // Si ya hay una dirección prioritaria, utiliza solo esa
            if (currentAxis == "Horizontal")
            {
                movement.x = h;
                movement.y = 0;

                if (h == 0) currentAxis = ""; // Resetear si ya no se está moviendo
            }
            else if (currentAxis == "Vertical")
            {
                movement.x = 0;
                movement.y = v;

                if (v == 0) currentAxis = ""; // Reset si ya no se está moviendo
            }
            else
            {
                movement = Vector2.zero;
            }
            UpdateAnimations();
        }
        else
        {
            movement = Vector2.zero;
            animator.SetBool("isWalking", false);
        }

        // Abrir / cerrar inventario con H
        if (Input.GetKeyUp(KeyCode.H) && inventario_com != null)
        {
            inventoryVisible = !inventoryVisible;
            if (inventoryVisible)
            {
                inventoryController.ShowInventory();
                canMove = false; // Desactivamos el movimiento del personaje al abrir el inventario
            }
            else
            {
                inventoryController.HideInventory();
                canMove = true; // Reactivamos el movimiento al cerrar
            }
        }

        // Navegación en el inventario con las flechas
        if (inventoryVisible)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                int nextSlot = (inventoryController.selectedSlotIndex + 1 + 4) % 4;
                inventoryController.SelectSlot(nextSlot);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                int prevSlot = (inventoryController.selectedSlotIndex - 1 + 4) % 4;
                inventoryController.SelectSlot(prevSlot);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                int prevRow = inventoryController.selectedSlotIndex - 2;
                if (prevRow >= 0)
                {
                    inventoryController.SelectSlot(prevRow);
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                int nextRow = inventoryController.selectedSlotIndex + 2;
                if (nextRow < 4)
                {
                    inventoryController.SelectSlot(nextRow);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    void UpdateAnimations()
    {
        if (movement != Vector2.zero)
        {
            animator.SetBool("isWalking", true);

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
            animator.SetBool("isWalking", false);
        }
    }
    public void DisableMovement()
    {
        canMove = false;
        animator.SetBool("isWalking", false);
    }

    public void EnableMovement()
    {
        canMove = true;
        animator.SetBool("isPickingUp", false);
    }
}