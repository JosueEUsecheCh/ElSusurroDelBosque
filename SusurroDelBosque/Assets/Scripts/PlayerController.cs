using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    // Variables de movimiento
    public float moveSpeed = 1f;
    private Animator animator;
    private Vector2 movement;
    private Rigidbody2D rb;

    // Inventario
    [Header("Inventario")]
    public GameObject inventario_com;
    private bool inventoryVisible = false;
    private string currentAxis = "";
    private InventoryController inventoryController;
    private bool canMove = true;

    // Cooldown
    [Header("Item Cooldown")]
    public float discardCooldown = 0.5f;
    private float lastDiscardTime = -1f;
    private bool circuitPanelOpen = false;

    // Audio
    [Header("Audio")]
    public AudioClip sonidoCorrer; // Clip de pasos
    private AudioSource audioSource; // Controlador de audio en el Player

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Configurar AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = sonidoCorrer;
        audioSource.loop = true; // se repetirá mientras camina
        audioSource.playOnAwake = false;

        if (inventario_com != null)
        {
            inventario_com.SetActive(false);
        }

        GameObject eventsObject = GameObject.FindGameObjectWithTag("general-events");
        if (eventsObject != null)
        {
            inventoryController = eventsObject.GetComponent<InventoryController>();
        }
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (canMove)
        {
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
                movement = Vector2.zero;
            }

            UpdateAnimations();
        }
        else
        {
            movement = Vector2.zero;
            animator.SetBool("isWalking", false);
            StopWalkingSound();
        }

        // Inventario
        if (Input.GetKeyUp(KeyCode.H) && inventario_com)
        {
            inventoryVisible = !inventoryVisible;
            if (inventoryVisible)
            {
                inventoryController.ShowInventory();
                canMove = false;
                StopWalkingSound();
            }
            else
            {
                HideInventoryAndEnableMovement();
            }
        }

        // Soltar item
        if (inventoryVisible && Input.GetKeyDown(KeyCode.M) && Time.time > lastDiscardTime + discardCooldown)
        {
            if (inventoryController != null)
            {
                inventoryController.DiscardSelectedItem();
                lastDiscardTime = Time.time;
            }
        }


// Navegación inventario
        if (inventoryVisible && inventoryController != null)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                int nextSlot = (inventoryController.selectedSlotIndex + 1 + 4) % 4;
                inventoryController.SelectSlot(nextSlot);
                inventoryController.UpdateGateSelection();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                int prevSlot = (inventoryController.selectedSlotIndex - 1 + 4) % 4;
                inventoryController.SelectSlot(prevSlot);
                inventoryController.UpdateGateSelection();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                int prevRow = inventoryController.selectedSlotIndex - 2;
                if (prevRow >= 0)
                {
                    inventoryController.SelectSlot(prevRow);
                    inventoryController.UpdateGateSelection();
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                int nextRow = inventoryController.selectedSlotIndex + 2;
                if (nextRow < 4)
                {
                    inventoryController.SelectSlot(nextRow);
                    inventoryController.UpdateGateSelection();
                }
            }
        }
    }

    public void SetCircuitPanelState(bool isOpen)
    {
        circuitPanelOpen = isOpen;
    }

    public void HideInventoryAndEnableMovement()
    {
        inventoryVisible = false;
        if (inventoryController != null)
        {
            inventoryController.HideInventory();
        }
        if (!circuitPanelOpen)
        {
            canMove = true;
        }
        else
        {
            Debug.Log("Movimiento Bloqueado por el circuito");
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

            // Reproducir sonido solo si no está ya sonando
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
            {
                animator.SetInteger("direction", movement.x > 0 ? 2 : 1);
            }
            else
            {
                animator.SetInteger("direction", movement.y > 0 ? 3 : 0);
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
            StopWalkingSound();
        }
    }

    private void StopWalkingSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void DisableMovement()
    {
        canMove = false;
        animator.SetBool("isWalking", false);
        StopWalkingSound();
    }

    public void EnableMovement()
    {
        if (!circuitPanelOpen && !inventoryVisible)
        {
            canMove = true;
        }
        animator.SetBool("isPickingUp", false);
    }
}