using UnityEngine;

public class PersistenObjects : MonoBehaviour
{
    public static PersistenObjects Instance;

    private void Awake()
    {
        if (PersistenObjects.Instance == null)
        {
            PersistenObjects.Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
