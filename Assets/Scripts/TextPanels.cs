using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPanels : MonoBehaviour
{
    public GameObject[] m_Panels = new GameObject[4];

    private bool m_GameStarted = false;
    private int m_currentPanelIndex = 0;

    void Update()
    {
        if (m_GameStarted)
            return;

        // 1Panel to 2Panel
        if (Input.GetKeyDown(KeyCode.Return))
        {
            HandleNextPanel();
            
        }
    }

    private void HandleNextPanel()
    {
        if (m_currentPanelIndex == 3)
        {
            m_GameStarted = true;
            m_Panels[m_currentPanelIndex].SetActive(false);
            GameManager.instance.StartGame();
            GameManager.instance.IsInstructionPannelShown = false;
            return;
        }

        m_Panels[m_currentPanelIndex].SetActive(false);
        m_Panels[++m_currentPanelIndex].SetActive(true);
    }
}
