using UnityEngine;

public class DontDestroyEventSystem : MonoBehaviour
{
    void Awake()
    {
        // La función correcta es DontDestroyOnLoad(GameObject).
        DontDestroyOnLoad(this.gameObject); 
    }
}