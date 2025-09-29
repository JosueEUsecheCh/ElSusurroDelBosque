using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string nextSceneName;
    // 1. VARIABLE NECESARIA: Configúrala en el Inspector (Ej: "Spawn_DesdeBosque").
    public string targetSpawnTag; 
    
    private Collider2D doorCollider;

    void Awake()
    {
        // Obtiene el Collider (el Box Collider 2D de tu Square).
        doorCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 2. Guarda el mensaje (la etiqueta de destino).
            if (PersistenObjects.Instance != null)
            {
                PersistenObjects.Instance.NextSpawnTag = targetSpawnTag;
            }
            
            // 3. ¡CLAVE PARA ROMPER EL BUCLE!: Desactiva el collider.
            if(doorCollider != null) doorCollider.enabled = false;

            // 4. Carga la escena.
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 5. Se activa cuando Mitch se aleja del spawn inicial.
        if (collision.CompareTag("Player") && doorCollider != null)
        {
            doorCollider.enabled = true; // Vuelve a activar el collider.
        }
    }
}