using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class SpawnPoint : MonoBehaviour
{
    void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject spawn = GameObject.FindWithTag("spawn"); 
        GameObject player = GameObject.FindWithTag("Player");

        if (spawn == null || player == null) return;

        Vector3 previousPos = player.transform.position;

        player.transform.SetPositionAndRotation(spawn.transform.position, spawn.transform.rotation);

        ForceCameraCut(player.transform, previousPos);
    }

    private void ForceCameraCut(Transform target, Vector3 previousPos)
    {
        CinemachineCamera vCam = FindFirstObjectByType<CinemachineCamera>();

        if (vCam != null)
        {
            Vector3 delta = target.position - previousPos;

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