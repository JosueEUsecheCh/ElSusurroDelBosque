using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Diccionario con estados de las lámparas por ID
    public Dictionary<string, bool> lampStates = new Dictionary<string, bool>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Guardar el estado de una lámpara
    public void SetLampState(string id, bool state)
    {
        lampStates[id] = state;
    }

    // Leer el estado de una lámpara
    public bool GetLampState(string id)
    {
        if (lampStates.ContainsKey(id))
            return lampStates[id];
        return false; // Por defecto apagada
    }
}