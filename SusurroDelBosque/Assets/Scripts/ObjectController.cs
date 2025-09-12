using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;


//significa que este script se puede asignar a un GameObject en Unity.
public class PickupItem : MonoBehaviour
{
    // Sirve para saber si el jugador está dentro del rango del objeto.
    public GameObject obj;
    public int cant = 1;
    private bool playerInRange = false;
    

    void Update()
    {
        // Si el jugador está en rango y presiona "B" nyeyeyey
        if (playerInRange && Input.GetKeyDown(KeyCode.B))
        {
            PickUp();
        }
    }

private void PickUp()
{
    GameObject[] inventario = GameObject.FindGameObjectWithTag("general-events").GetComponent<InventoryController>().getSlots();
    for (int i = 0; i < inventario.Length; i++)
    {
        if (!inventario[i])
        {
            GameObject.FindGameObjectWithTag("general-events").GetComponent<InventoryController>().setSlots(obj, i);
            Destroy(gameObject);
            break;
        }
    }
}


    private void OnTriggerEnter2D(Collider2D other)
    {
        // el if compara el collider del objeto si se acerca un gameobject con el tag Player
        if (other.CompareTag("Player"))
        {
            // Compara los collider de mitch y el objeto si esta en rango de recoger cambia la variable a true para poder recogerlo
            playerInRange = true;

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Compara los collider de mitch y el objeto si no esta en rango de recoger cambia la variable a false y no podra recoger el objeto
            playerInRange = false;
        }
    }
}
