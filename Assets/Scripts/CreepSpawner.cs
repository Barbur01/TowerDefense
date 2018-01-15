using UnityEngine;
using System.Collections;

public class CreepSpawner : MonoBehaviour
{
    public float m_TimeBetweenOrders;
    public int m_MaxEnemiesOrder;
    public int MAX_FIBONAZZI;
    public GameObject m_Creep;
    private Vector3 m_NextTargetPosition;

    Transform m_Transform;

    private int m_LastFibonacciIndex;
    private float m_TimeUntilNextOrder;
    private int m_NumEnemies;
    private float m_TimeUntilNextEnemy;

    private enum SpawnerState
    {
        WAITING,
        STARTING_SPAWN,
        SPAWNING,
        SPAWNING_END
    }

    private SpawnerState m_State;

    // Use this for initialization
    void Start()
    {
        m_Transform = transform;
        m_LastFibonacciIndex = 1;
        m_TimeUntilNextOrder = m_TimeBetweenOrders;
        MAX_FIBONAZZI = 7;
        m_NextTargetPosition = GetNextTargetPoint();
        m_State = SpawnerState.WAITING;
        m_NumEnemies = 0;
        m_TimeUntilNextEnemy = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        m_TimeUntilNextOrder -= Time.deltaTime;

        switch (m_State)
        {
            case SpawnerState.WAITING:
                if (m_TimeUntilNextOrder <= 0)
                {
                    m_State = SpawnerState.STARTING_SPAWN;
                }
                break;
            case SpawnerState.STARTING_SPAWN:
                m_NumEnemies = Fibonacci(m_LastFibonacciIndex);
                m_NumEnemies = Mathf.Min(m_NumEnemies, m_MaxEnemiesOrder);
                if (m_NumEnemies != -1)
                {
                    m_State = SpawnerState.SPAWNING;
                }
                break;
            case SpawnerState.SPAWNING:
                m_TimeUntilNextEnemy -= Time.deltaTime;
                if (m_TimeUntilNextEnemy <= 0.0f)
                {
                    CreateEnemy();
                    m_NumEnemies--;
                    if (m_NumEnemies <= 0)
                    {
                        m_State = SpawnerState.SPAWNING_END;
                    }
                    m_TimeUntilNextEnemy = 1.0f;
                }
                break;
            case SpawnerState.SPAWNING_END:
                if (m_LastFibonacciIndex < MAX_FIBONAZZI)
                {
                    m_LastFibonacciIndex++;
                }
                ResetTimeUntilNextOrder();
                m_NextTargetPosition = GetNextTargetPoint();
                m_State = SpawnerState.WAITING;
                break;
            default:
                break;
        }
    }

    void CreateEnemy()
    {
        GameObject.Instantiate(m_Creep, m_NextTargetPosition, Quaternion.identity, m_Transform);
        m_NextTargetPosition = GetNextTargetPoint();
    }

    Vector3 GetNextTargetPoint()
    {
        return m_Transform.position;
    }

    void ResetTimeUntilNextOrder()
    {
        m_TimeUntilNextOrder = m_TimeBetweenOrders;
    }

    int Fibonacci(int n)
    {
        if (n == 0)
        {
            return 0;
        }
        else if (n == 1)
        {
            return 1;
        }
        else if (n >= 2)
        {
            return Fibonacci(n - 1) + Fibonacci(n - 2);
        }
        else
        {
            Debug.Log("Bad fibonacci index");
            Debug.Assert(false);
            return -1;
        }
    }
}
