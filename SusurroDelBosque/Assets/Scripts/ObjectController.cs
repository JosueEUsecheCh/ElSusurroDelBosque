using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickupItem : MonoBehaviour
{
    public string itemName;
    public Sprite itemIcon;
    [TextArea]
    public string itemDescription;
    public string prefabName;

    private GameObject player;
    private bool playerIsInRange = false;
    private InventoryController inventoryController;

    void Start()
    {
        // intanciamos InventoryController
        inventoryController = FindFirstObjectByType<InventoryController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            playerIsInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            playerIsInRange = false;
        }
    }

    void Update()
    {
        if (playerIsInRange && Input.GetKeyDown(KeyCode.B))
        {
            if (IsPlayerStopped())
            {
                AttemptPickup();
            }
        }
    }

    private void AttemptPickup()
    {
        if (inventoryController != null && inventoryController.HasSpace())
        {
            PickUp();
        }
        else
        {
            Debug.Log("Inventario lleno. No se puede recoger el ítem '" + itemName + "'.");
        }
    }

    private bool IsPlayerStopped()
    {
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                return player.GetComponent<Rigidbody2D>().linearVelocity.magnitude < 0.1f;
            }
        }
        return false;
    }

    private void PickUp()
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        Animator playerAnimator = player.GetComponent<Animator>();

        if (playerMovement != null && playerAnimator != null)
        {
            playerMovement.DisableMovement();
            int direction = playerAnimator.GetInteger("direction"); 
            playerAnimator.SetBool("isPickingUp", true);
            playerAnimator.SetInteger("pickupDirection", direction);
        }

        StartCoroutine(WaitForAnimationAndDestroy());
    }

    private IEnumerator WaitForAnimationAndDestroy()
    {
        yield return new WaitForSeconds(0.5f);

        if (inventoryController != null)
        {
            inventoryController.AddItem(itemName, itemIcon, itemDescription, prefabName);
        }

        if (player != null)
        {
            player.GetComponent<PlayerMovement>().EnableMovement();
        }

        Destroy(gameObject); 
    }
}