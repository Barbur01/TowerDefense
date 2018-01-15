using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Tower : MonoBehaviour
{
    enum State
    {
        BUILDING = 0,
        IDLE,
        ATTACK,

        INVALID
    };

    public delegate void TowerSelected(bool canUpgrade);
    public static event TowerSelected OnTowerSelected;
    public delegate void TowerLocating();
    public static event TowerLocating OnTowerLocating;
    public delegate void TowerCreated();
    public static event TowerCreated OnTowerCreated;

    public float m_ShootTime = 1.0f;
    public float m_RotationSpeed = 2.0f;
    public float m_ShootingRadius = 5;
    public int m_Cost = 5;
    int mUpgradeLevel = 1;

    public GameObject m_Projectile;
    public Transform m_CannonHead;
    public Transform m_CannonTop;

    Transform m_Transform;
    Transform m_EnemyTarget = null;
    float m_LastShotTime = 0.0f;
    float m_BuildRadius = 0.0f;
    State m_State = State.INVALID;

    bool m_IsSelected = false;

    private void Awake()
    {
        m_Transform = transform;
    }

    void Start()
    {
        m_State = State.BUILDING;

        GetComponent<NavMeshObstacle>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;

        CalculateRadius();

        if (OnTowerLocating != null)
        {
            OnTowerLocating();
        }
    }

    public void Build()
    {
        m_State = State.IDLE;
        GetComponent<NavMeshObstacle>().enabled = true;
        GetComponent<CapsuleCollider>().enabled = true;
        SetTowerAlphaColor(1.0f);

        ++mUpgradeLevel;

        if (OnTowerCreated != null)
        {
            OnTowerCreated();
        }
    }

    void CalculateRadius()
    {
        m_BuildRadius = GetComponent<CapsuleCollider>().radius;
    }

    public void SetTowerSelected(bool selected)
    {
        if (m_IsSelected != selected)
        {
            if (OnTowerSelected != null)
            {
                OnTowerSelected(selected);
            }

            m_IsSelected = selected;
        }
    }

    public bool CanBuild()
    {
        Collider[] colliders = Physics.OverlapSphere(m_Transform.position, GetBuildRadius());

        foreach (var col in colliders)
        {
            if (col.gameObject != gameObject && 
                col.gameObject.layer != LayerMask.NameToLayer("terrain") &&
                col.gameObject.layer != LayerMask.NameToLayer("creep"))
            {
                return false;
            }
        }

        return true;
    }

    void SetTowerAlphaColor(float alpha)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Color c = renderer.material.color;
            c.a = alpha;
            renderer.material.color = c;
        }
    }

    public void CheckCanBuild()
    {
        if (CanBuild())
        {
            SetTowerAlphaColor(1.0f);
        }
        else
        {
            SetTowerAlphaColor(0.5f);
        }
    }

    public float GetBuildRadius()
    {
        return m_BuildRadius;
    }

    public int GetCost()
    {
        return m_Cost * mUpgradeLevel;
    }

    Transform GetNearestTarget()
    {
        Creep[] enemies = GameObject.FindObjectsOfType<Creep>();

        Transform nearestEnemy = null;
        float minDistance = float.MaxValue;

        for (int i = 0; i < enemies.Length; ++i)
        {
            Creep creep = enemies[i].GetComponent<Creep>();
            if (!creep.IsDead() && !creep.IsTargeted())
            {
                Vector2 pos = m_Transform.position - enemies[i].transform.position;
                float sqDistance = Vector2.SqrMagnitude(pos);

                if (IsInsideShootingRadius(enemies[i].transform) && minDistance > sqDistance)
                {
                    minDistance = sqDistance;
                    nearestEnemy = enemies[i].transform;
                }
            }
        }

        return nearestEnemy;
    }

    public void Upgrade()
    {

    }

    void Shoot(Transform enemy)
    {
        if (m_LastShotTime <= 0.0f)
        {
            GameObject projectileObj = GameObject.Instantiate(m_Projectile, m_CannonHead.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.SetTarget(enemy);
            m_LastShotTime = m_ShootTime;
        }
    }

    bool IsInsideShootingRadius(Transform target)
    {
        Vector3 dir = m_Transform.position - target.position;
        dir.y = 0;
        return dir.sqrMagnitude <= m_ShootingRadius * m_ShootingRadius;
    }

    bool IsTargetDead(Transform target)
    {
        if (target != null)
        {
            Creep creep = target.GetComponent<Creep>();
            return creep.IsDead();
        }

        return true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_LastShotTime -= Time.deltaTime;

        switch (m_State)
        {
            case State.BUILDING:
                break;
            case State.IDLE:
                UpdateIdle();
                break;
            case State.ATTACK:
                UpdateAttack();
                break;
            case State.INVALID:
                break;
            default:
                break;
        }
	}

    void UpdateIdle()
    {
        if (m_EnemyTarget == null)
        {
            m_EnemyTarget = GetNearestTarget();
        }

        if (m_EnemyTarget)
        {
            m_EnemyTarget.GetComponent<Creep>().SetTargeted(true);
            m_State = State.ATTACK;
        }
    }

    float FaceTarget()
    {
        Vector3 dir = m_EnemyTarget.position - m_Transform.position;
        dir.y = 0;
        dir = dir.normalized;

        float targetAngle = Vector3.SignedAngle(m_CannonTop.transform.forward, dir, Vector3.up);
        float sign = Mathf.Sign(targetAngle);
        float delta = Mathf.Min(Mathf.Abs(targetAngle), m_RotationSpeed * Time.deltaTime);

        m_CannonTop.Rotate(Vector3.up, sign * delta);

        return targetAngle;
    }

    void UpdateAttack()
    {
        if (IsTargetDead(m_EnemyTarget) || !IsInsideShootingRadius(m_EnemyTarget))
        {
            if (m_EnemyTarget != null)
            {
                m_EnemyTarget.GetComponent<Creep>().SetTargeted(false);
            }

            m_State = State.IDLE;
            m_EnemyTarget = null;
        }
        else
        {
            float targetAngle = FaceTarget();

            if (Mathf.Abs(targetAngle) < 5.0f)
            {
                Shoot(m_EnemyTarget);
            }
        }
    }
}
