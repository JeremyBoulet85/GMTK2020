using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField]
    private int totalKeys = 4;

    enum GameState
    {
        MainMenu,
        Instruction,
        Start,
        End
    }

    public int StrikeCount { get; set; }
    public bool Finished { get; private set; } = false;
    public int CurrentKeys { get; private set; } = 0;
    public int TotalKeys { get => totalKeys; }

    public GameObject keyPrefab;

    private GameObject m_Player;
    private Transform m_PlayerSpawnTransform;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        Debug.Log("Awake");
    }

    private void Start()
    {
        Debug.Log("Start");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStrikeCount();
    }

    public void CheckGameWin()
    {
        if (CurrentKeys == TotalKeys)
        {
            var pc = m_Player.GetComponent<PlayerController>();
            pc.StopWalking();
            pc.enabled = false;
            m_Player.GetComponent<SneezeSystem>().enabled = false;
            m_Player.GetComponent<FartSystem>().enabled = false;
            m_Player.GetComponent<HungerSystem>().enabled = false;

            var endPanel = GameObject.Find("HudCanvas").transform.Find("EndPanel").gameObject;
            endPanel.SetActive(true);
        }
    }

    public void StartGame()
    {
        CurrentKeys = 0;
        StrikeCount = 0;

        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_PlayerSpawnTransform = GameObject.FindGameObjectWithTag("PlayerSpawn").transform;
        m_Player.transform.position = m_PlayerSpawnTransform.position;

        var spawns = GameObject.FindGameObjectsWithTag("KeySpawn");
        foreach (var spawn in spawns)
        {
            Instantiate(keyPrefab, spawn.transform);
        }

        m_Player.GetComponent<PlayerController>().enabled = true;
        m_Player.GetComponent<SneezeSystem>().enabled = true;
        m_Player.GetComponent<FartSystem>().enabled = true;
        m_Player.GetComponent<HungerSystem>().enabled = true;
    }

    void UpdateStrikeCount()
    {
        if (StrikeCount >= 3)
        {
            m_Player.transform.position = m_PlayerSpawnTransform.position;
            StrikeCount = 0;
        }
    }

    public void PickUpKey()
    {
        ++CurrentKeys;
        Finished = CurrentKeys == totalKeys;
    }
}
