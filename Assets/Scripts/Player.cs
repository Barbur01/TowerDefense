using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public int mCoins = 5;

    public Tower m_TowerPrefab;
    Tower m_TowerToBuild = null;
    Tower m_TowerToUpgrade = null;

    public delegate void CoinsChanged(int totalCoins);
    public static event CoinsChanged OnCoinsChanged;

    public delegate void TowerUpgradeAvailable();
    public static event TowerUpgradeAvailable OnTowerUpgradeAvailable;

    public delegate void CancelBuildTowerEvent();
    public static event CancelBuildTowerEvent OnCancelBuildTowerEvent;

    void Start()
    {
    }

    private void OnEnable()
    {
        Creep.OnCreepDied += CreepDied;
    }

    private void OnDisable()
    {
        Creep.OnCreepDied -= CreepDied;
    }

    void CreepDied(Creep creep)
    {
        AddCoin(creep.GetCoins());
    }

    void AddCoin(int coins)
    {
        mCoins += coins;

        if (OnCoinsChanged != null)
        {
            OnCoinsChanged(mCoins);
        }
    }

    bool HasEnoughCoins(Tower tower)
    {
        return mCoins > 0 && tower.GetCost() <= mCoins;
    }

    void BuildTower(Vector3 pos)
    {
        m_TowerToBuild.Build();
    }

    public void CancelBuildTower()
    {
        Destroy(m_TowerToBuild.gameObject);
        m_TowerToBuild = null;

        if (OnCancelBuildTowerEvent != null)
        {
            OnCancelBuildTowerEvent();
        }
    }

    void UpgradeTower(Tower tower)
    {
        tower.Upgrade();
    }

    public void PrepareNewTower()
    {
        m_TowerToBuild = null;

        if (HasEnoughCoins(m_TowerPrefab))
        {
            m_TowerToBuild = GameObject.Instantiate(m_TowerPrefab, new Vector3(10000, 0, 0), Quaternion.identity);
        }
    }

    void CheckBuildTower()
    {
        Vector3 mpos = Input.mousePosition;
        Ray r = Camera.main.ScreenPointToRay(mpos);
        RaycastHit hit;

        if (Physics.Raycast(r, out hit, 1000, LayerMask.GetMask("terrain")))
        {
            m_TowerToBuild.transform.position = hit.point;
            m_TowerToBuild.CheckCanBuild();

            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject() && 
                    m_TowerToBuild.CanBuild())
                {
                    AddCoin(-m_TowerToBuild.GetCost());
                    m_TowerToBuild.Build();
                    m_TowerToBuild = null;
                }
            }
        }
    }

    void CheckUpgradeTower()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mpos = Input.mousePosition;
            Ray r = Camera.main.ScreenPointToRay(mpos);
            RaycastHit hit;

            if (Physics.Raycast(r, out hit, 1000, LayerMask.GetMask("tower")))
            {
                Tower towerToUpgrade = hit.collider.GetComponent<Tower>();
                if (towerToUpgrade != m_TowerToUpgrade)
                {
                    if (m_TowerToUpgrade != null)
                    {
                        m_TowerToUpgrade.SetTowerSelected(false);
                    }

                    m_TowerToUpgrade = towerToUpgrade;
                    m_TowerToUpgrade.SetTowerSelected(true);

                    if (mCoins >= m_TowerToUpgrade.GetCost())
                    {
                        if (OnTowerUpgradeAvailable != null)
                        {
                            OnTowerUpgradeAvailable();
                        }
                    }
                }
            }
        }
    }

    void Update()
    {
        if (m_TowerToBuild != null)
        {
            CheckBuildTower();
        }
        else
        {
            CheckUpgradeTower();
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            ++Time.timeScale;
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            --Time.timeScale;
        }
    }
}
