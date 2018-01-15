using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour {

    public int m_Health = 1;

	// Use this for initialization
	void Start () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (!IsDead())
        {
            ApplyDamage();
        }

        Destroy(other.gameObject);
    }

    void ApplyDamage()
    {
        --m_Health;

        if (m_Health <= 0)
        {
            Dead();
        }
    }

    bool IsDead()
    {
        return m_Health <= 0;
    }

    void Dead()
    {
        // GameOver!!
    }

    // Update is called once per frame
    void Update () {
		
	}
}
