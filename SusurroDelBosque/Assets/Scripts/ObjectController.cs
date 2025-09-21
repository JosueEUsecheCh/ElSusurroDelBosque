using UnityEngine;
using System.Collections;

public class PickupItem : MonoBehaviour
{
    public string itemName;     // Nombre del objeto
    public Sprite itemIcon;     // Sprite que se mostrará en el inventario

    public string itemDescription; // Descripcion del objeto
    private bool playerInRange = false; //Variable para decir si esta en rango o no 

    void Update()
    {
        // Recoger el objeto si está en rango y presionas B
        if (playerInRange && Input.GetKeyDown(KeyCode.B))
        {
            PickUp();
        }
    }

    private void PickUp()
    {
        // Encuentra el objeto del jugador por su tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Obtén una referencia al script PlayerMovement y al Animator
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        Animator playerAnimator = player.GetComponent<Animator>();

        if (playerMovement != null && playerAnimator != null)
        {
            // Detén el movimiento del jugador
            playerMovement.DisableMovement();

            // Obtén la dirección del jugador para la animación
            int direction = playerAnimator.GetInteger("direction"); // Asume que ya tienes una variable de dirección para caminar

            // Configura los parámetros del Animator
            playerAnimator.SetBool("isPickingUp", true);
            playerAnimator.SetInteger("pickupDirection", direction);
        }

        // Llama a una Coroutine para esperar a que termine la animación antes de destruir el objeto
        StartCoroutine(WaitForAnimationAndDestroy());
    }
    private IEnumerator WaitForAnimationAndDestroy() //funcion para poner un temporizador al recoger objetos apara que de tiempo de recoger el objeto antes de destruirlo 
    {
        // Esperar un momento para que la animación de recoger se reproduzca
        yield return new WaitForSeconds(0.5f); // Ajusta este valor según la duración de tu animación

        InventoryController inventory = GameObject.FindGameObjectWithTag("general-events").GetComponent<InventoryController>();
        if (inventory != null)
        {
            inventory.AddItem(itemName, itemIcon, itemDescription);
        }
        
        // Vuelve a habilitar el movimiento del jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.GetComponent<PlayerMovement>().EnableMovement();
        }

        Destroy(gameObject); // Elimina el objeto del mundo
    }

    private void OnTriggerEnter2D(Collider2D other) // funcion de si esta en rango
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true; // Ahora el jugador puede recogerlo
        }
    }

    private void OnTriggerExit2D(Collider2D other) // funcion de si no esta en rango
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false; // Ya no está en rango
        }
    }
}
