using UnityEngine;
using UnityEngine.AI;

public class MerdoController : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;

    private float repathTimer = 0f;
    public float repathRate = 1f;

    public Transform[] pathPoints;
    public int pathIndex;
    public float pointReachThreshold = 0.2f;

    public float detectionRange = 5f;
    private bool chasingPlayer = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    void Start()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        //transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        if (pathPoints !=null && pathPoints.Length>0)
        {
            agent.SetDestination(pathPoints[pathIndex].position);
        }
    }

    void Update()
    {
        if (target != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);

            // si Mitch est� dentro del rango, empezar a perseguir
            if (distanceToPlayer <= detectionRange)
            {
                chasingPlayer = true;
            }
            else
            {
                chasingPlayer = false;
            }
        }

        if (chasingPlayer && target != null)
        {
            // Persecucion
            repathTimer -= Time.deltaTime;
            if (repathTimer <= 0f)
            {
                agent.SetDestination(target.position);
                repathTimer = repathRate;
            }
        }
        else if (pathPoints != null && pathPoints.Length > 0)
        {
            // Patrullaje
            if (Vector3.Distance(transform.position, pathPoints[pathIndex].position) < pointReachThreshold)
            {
                if (pathIndex < pathPoints.Length - 1)
                {
                    pathIndex++;
                }
                else
                {
                    pathIndex = 0; // reinicia el recorrido
                }
            }

            agent.SetDestination(pathPoints[pathIndex].position);
        }
    }
}