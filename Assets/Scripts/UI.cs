using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text m_NumCoins;
    public Button m_CreateTowerButton;
    public Button m_CancelTowerButton;
    public Button m_UpgradeTowerButton;

    void Start ()
    {	
	}

    private void OnEnable()
    {
        Player.OnCoinsChanged += SetCoins;
        Player.OnTowerUpgradeAvailable += ShowUpgradeTowerButton;
        Tower.OnTowerCreated += ShowCreateTowerButton;
        Tower.OnTowerLocating += ShowCancelTowerButton;
    }

    private void OnDisable()
    {
        Player.OnCoinsChanged -= SetCoins;
        Player.OnTowerUpgradeAvailable -= ShowUpgradeTowerButton;
        Tower.OnTowerCreated -= ShowCreateTowerButton;
        Tower.OnTowerLocating -= ShowCancelTowerButton;
    }

    void Update ()
    {	
	}

    public void SetCoins(int numCoins)
    {
        m_NumCoins.text = numCoins.ToString();
    }

    public void ShowCancelTowerButton()
    {
        m_CreateTowerButton.gameObject.SetActive(false);
        m_CancelTowerButton.gameObject.SetActive(true);
        m_UpgradeTowerButton.gameObject.SetActive(false);
    }

    public void ShowUpgradeTowerButton()
    {
        m_CreateTowerButton.gameObject.SetActive(false);
        m_CancelTowerButton.gameObject.SetActive(false);
        m_UpgradeTowerButton.gameObject.SetActive(true);
    }

    void ShowCreateTowerButton()
    {
        m_CreateTowerButton.gameObject.SetActive(true);
        m_CancelTowerButton.gameObject.SetActive(false);
        m_UpgradeTowerButton.gameObject.SetActive(false);
    }
}
