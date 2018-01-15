using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Creep : MonoBehaviour {

    public float m_Speed = 4.0f;
    public int m_Health = 1;
    public int m_NumCoins = 1;

    NavMeshAgent m_Agent;
    Transform m_PlayerBase;
    bool m_IsDying = false;
    bool m_Targeted = false;

    public delegate void CreepDied(Creep c);
    public static event CreepDied OnCreepDied;


    // Use this for initialization
    void Start ()
    {
        m_PlayerBase = GameObject.FindObjectOfType<PlayerBase>().transform;
        m_Agent = GetComponent<NavMeshAgent>();
        AttackBase();
	}

    void ApplyDamage(int damage)
    {
        m_Health -= damage;

        if (m_Health <= 0 && !m_IsDying)
        {
            Die();
        }
    }

    public int GetCoins()
    {
        return m_NumCoins;
    }

    void Die()
    {
        if (OnCreepDied != null)
        {
            OnCreepDied(this);
        }

        GetComponent<Renderer>().enabled = false;
        m_IsDying = true;
    }

    public void SetTargeted(bool targeted)
    {
        m_Targeted = targeted;
    }

    public bool IsTargeted()
    {
        return m_Targeted;
    }

    void AttackBase()
    {
        m_Agent.SetDestination(m_PlayerBase.position);
        m_Agent.speed = m_Speed;
    }

    public bool IsDead()
    {
        return m_Health <= 0 || m_IsDying;
    }
	
	// Update is called once per frame
	void Update () {
		
        if (m_IsDying)
        {
            Destroy(gameObject, 1.0f);
        }
	}
}
