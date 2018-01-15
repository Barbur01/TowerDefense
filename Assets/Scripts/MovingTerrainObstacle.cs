using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTerrainObstacle : MonoBehaviour
{
    enum State
    {
        IDLE = 0,
        MOVING,

        INVALID
    }

    public float m_Delta = 1.5f;
    public float m_Speed = 2.0f;
    public Vector2 m_WaitTimeRange = new Vector2(5.0f, 10.0f);

    Transform m_Transform;
    State m_State = State.INVALID;
    private Vector3 startPos;
    float m_Time;
    float m_Direction = 1.0f;

    float m_CurrentTime = 0.0f;

    void Start()
    {
        m_Transform = transform;
        startPos = m_Transform.position;
        m_State = State.MOVING;
    }

    void Update()
    {
        switch (m_State)
        {
            case State.IDLE:
                m_CurrentTime -= Time.deltaTime;
                if (m_CurrentTime <= 0.0f)
                {
                    m_State = State.MOVING;
                }
                break;
            case State.MOVING:
                {
                    m_Time += Time.deltaTime * m_Direction;

                    Vector3 v = startPos;
                    float sin = Mathf.Sin(m_Time * m_Speed);
                    v.x += m_Delta * sin;

                    m_Transform.position = v;

                    if (Mathf.Abs(sin) > 0.95f)
                    {
                        m_CurrentTime = Random.Range(m_WaitTimeRange[0], m_WaitTimeRange[1]);
                        m_State = State.IDLE;
                        m_Direction *= -1.0f;
                    }
                }
                break;
            case State.INVALID:
                break;
            default:
                break;
        }
    }
}
