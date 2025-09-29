using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine; // <- Asegúrate de que esta línea esté presente

public class SpawnPoint : MonoBehaviour
{
    
    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 1. Lee el mensaje (la etiqueta de destino: "Spawn_DesdeMain" o "spawn").
            string targetSpawnTag = PersistenObjects.Instance?.NextSpawnTag ?? "spawn";
            
            // 2. Busca el objeto con la ETIQUETA específica.
            GameObject spawn = GameObject.FindWithTag(targetSpawnTag); 
            GameObject player = GameObject.FindWithTag("Player");

            if (spawn == null || player == null) 
            {
                // Si no encuentra el spawn, el jugador se queda donde está, pero la puerta ya fue desactivada.
                return; 
            }

            Vector3 previousPos = player.transform.position;

            // 3. Mueve al jugador al Tag correcto.
            player.transform.SetPositionAndRotation(spawn.transform.position, spawn.transform.rotation);
            ForceCameraCut(player.transform, previousPos);
            
            // 4. Resetea el tag para futuras transiciones (vuelve a "spawn").
            if (PersistenObjects.Instance != null) PersistenObjects.Instance.NextSpawnTag = "spawn";
        }
        // ... (Tu función ForceCameraCut permanece sin cambios) ...


    private void ForceCameraCut(Transform target, Vector3 previousPos)
    {
        // 1. Encuentra la cámara virtual (usamos FindFirstObjectByType para evitar la advertencia).
        // NOTA: Si usas la clase antigua, usa 'CinemachineVirtualCamera'. Si no, 'CinemachineCamera'.
        CinemachineCamera vCam = FindFirstObjectByType<CinemachineCamera>();

        if (vCam != null)
        {
            // Calcula el vector de desplazamiento (delta).
            Vector3 delta = target.position - previousPos;

            // Llama al método directamente en la cámara virtual.
            vCam.OnTargetObjectWarped(target, delta);

            // PASO AGRESIVO: Aunque OnTargetObjectWarped debería funcionar,
            // forzamos un reinicio temporal del Brain para garantizar el corte.
            var brain = Camera.main?.GetComponent<CinemachineBrain>();
            if (brain != null)
            {
                brain.enabled = false;
                brain.enabled = true;
            }
        }
    }
}