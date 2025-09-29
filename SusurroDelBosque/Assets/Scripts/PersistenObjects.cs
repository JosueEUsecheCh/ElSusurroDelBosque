// EN PersistenObjects.cs (ADJUNTADO A TU OBJETO DONTDESTROY)
using UnityEngine;

public class PersistenObjects : MonoBehaviour
{
    public static PersistenObjects Instance;
    // CLAVE: Usaremos "spawn" como el Tag por defecto.
    public string NextSpawnTag { get; set; } = "spawn"; 

    private void Awake()
    {
        if (PersistenObjects.Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}