using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public GameObject[] slots;
    private int num_slots_max = 4; // ahora dijiste que son 4

    void Start()
    {
        slots = new GameObject[num_slots_max];
    }

    public GameObject[] getSlots()
    {
        return this.slots;
    }

    // Ahora solo guarda el objeto y su nombre
    public void setSlots(GameObject slot, int pos)
    {
        bool exist = false;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                if (slots[i].tag == slot.tag)
                {
                    exist = true; // Ya está ese objeto, no lo duplicamos
                }
            }
        }

        if (!exist)
        {
            this.slots[pos] = slot;
            var attr = slot.GetComponent<AttributeController>();
            if (attr != null)
            {
                attr.setName(slot.name); // usa el nombre del prefab como nombre
            }
        }
    }

    public void showInventory()
    {
        Component[] inventario = GameObject.FindGameObjectWithTag("inventario").GetComponentsInChildren<Transform>();
        bool slotUsed = false;

        if (removeItems(inventario))
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null)
                {
                    slotUsed = false;

                    for (int j = 0; j < inventario.Length; j++)
                    {
                        GameObject child = inventario[j].gameObject;

                        if (child.tag == "slot" && child.transform.childCount <= 1 && !slotUsed)
                        {
                            GameObject item = Instantiate(slots[i], child.transform.position, Quaternion.identity);
                            item.transform.SetParent(child.transform, false);
                            item.transform.localPosition = Vector3.zero;
                            item.name = item.name.Replace("(Clone)", "");

                            // aquí ya no usamos cantidad, solo mostramos el nombre
                            var attr = item.GetComponent<AttributeController>();
                            if (attr != null)
                            {
                                attr.setName(item.name);
                            }

                            slotUsed = true;
                        }
                    }
                }
            }
        }
    }

    public bool removeItems(Component[] inventario)
    {
        for (int i = 0; i < inventario.Length; i++)
        {
            GameObject child = inventario[i].gameObject;
            if (child.tag == "slot" && child.transform.childCount > 0)
            {
                for (int j = 0; j <= 0; j++)
                {
                    Destroy(child.transform.GetChild(j).gameObject);
                }
            }
        }
        return true;
    }
}
