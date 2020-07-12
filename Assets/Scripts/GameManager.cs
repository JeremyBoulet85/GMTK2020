﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // 60f -> 60 seconds
    private const float TOTAL_GAME_TIME = 120f;
    private const string MENU_SCENE = "MainMenu";

    public static GameManager instance = null;

    [SerializeField]
    private int totalKeys = 4;

    [SerializeField]
    private int totalStrikes = 3;

    public bool Finished { get; private set; } = false;
    public bool IsGameOver { get; private set; } = false;
    public bool IsGamePaused { get; private set; } = false;
    public int CurrentKeys { get; private set; } = 0;
    public int TotalKeys { get => totalKeys; }
    public int StrikeCount { get; private set; } = 0;
    public int TotalStrikes { get => totalStrikes; }
    public bool IsInstructionPannelShown { get; set; } = true;

    public bool IsHardMode { get; set; } = false;

    public float HardModeTimer { get => hardModeTimer; }

    public GameObject keyPrefab;

    private GameObject pauseMenu;
    private GameObject m_Player;
    private Transform m_PlayerSpawnTransform;
    private Transform m_GameOverSpawnTransform;
    private float gameOverTimer = 0f;
    private float principalTimer = 0f;
    private bool showGameOverPanel = false;
    private float hardModeTimer = TOTAL_GAME_TIME;
    private bool startHardModeTimer = false;


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
    }

    public void Respawn()
    {
        m_Player.transform.position = m_PlayerSpawnTransform.position;
    }

    private bool soundInit = false;
    private void InitSound()
    {
        soundInit = true;
        FindObjectOfType<AudioManager>().PlayMusic();
        FindObjectOfType<AudioManager>().PlayAmbiance();
    }

    void Update()
    {
        if (!soundInit)
            InitSound();

        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) && !IsInstructionPannelShown)
        {
            if (IsGamePaused)
                Resume();
            else
                Pause();
        }

        if (showGameOverPanel)
        {
            if (gameOverTimer == 0)
            {
                FindObjectOfType<AudioManager>().Play("PrincipalAngry");
            }
            gameOverTimer += Time.deltaTime;
            principalTimer += Time.deltaTime;
            
            if (gameOverTimer > 3.5f)
            {
                gameOverTimer = 0f;
                ShowEndPanel("You lost!", "Your noisy impulses were out of control.\nTime to get lectured by the principal.");
                showGameOverPanel = false;
            }
            if (principalTimer > 0.3f)
            {
                principalTimer = 0f;
                var angryExclamations = GameObject.Find("Map").transform.Find("Principal").transform.Find("AngryPrincipal").gameObject;
                angryExclamations.SetActive(!angryExclamations.activeSelf);
            }
        }

        if (startHardModeTimer)
        {
            if (hardModeTimer < 0)
            {
                GameOver();
            }
            hardModeTimer -= Time.deltaTime;
            print(hardModeTimer);
        }
    }

    public void GameOver()
    {
        IsGameOver = false;
        StrikeCount = 0;
        m_Player.transform.position = m_GameOverSpawnTransform.position;
        showGameOverPanel = true;
        ResetHardModeTimer();
        FreezeGame();
    }

    public void CheckGameWin()
    {
        if (CurrentKeys == TotalKeys)
        {
            FreezeGame();
            ShowEndPanel("You won!", "You kept your noisy impulses under control\nand sneaked out of school.");
        }
    }

    public void StartGame()
    {
        CurrentKeys = 0;
        StrikeCount = 0;

        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_PlayerSpawnTransform = GameObject.Find("PlayerSpawn").transform;
        m_Player.transform.position = m_PlayerSpawnTransform.position;

        m_GameOverSpawnTransform = GameObject.Find("GameOverSpawn").transform;

        var spawns = GameObject.FindGameObjectsWithTag("KeySpawn");

        List<int> usedIndexes = new List<int>();
        for (int i = 0; i < 4; i++) 
        {
            int rndIndex = GenerateNumber(spawns.Length);
            Instantiate(keyPrefab, spawns[rndIndex].transform);
        }

        m_Player.GetComponent<PlayerController>().enabled = true;
        m_Player.GetComponent<SneezeSystem>().enabled = true;
        m_Player.GetComponent<FartSystem>().enabled = true;
        m_Player.GetComponent<HungerSystem>().enabled = true;

        if (IsHardMode) 
        {
            startHardModeTimer = true;
        }
    }

    public void GetStriked()
    {
        ++StrikeCount;
        IsGameOver = StrikeCount >= TotalStrikes;
    }

    public void PickUpKey()
    {
        ++CurrentKeys;
        Finished = CurrentKeys == totalKeys;
    }

    public void Resume()
    {
        pauseMenu = GameObject.Find("HudCanvas").transform.Find("PauseMenu").gameObject;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        IsGamePaused = false;
    }

    public void LoadMenu()
    {
        IsInstructionPannelShown = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(MENU_SCENE);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Pause()
    {
        pauseMenu = GameObject.Find("HudCanvas").transform.Find("PauseMenu").gameObject;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        IsGamePaused = true;
    }

    private void ShowEndPanel(string titleText, string contentText)
    {
        var endPanel = GameObject.Find("HudCanvas").transform.Find("TextPanelContainer").transform.Find("EndPanel").gameObject;
        var title = endPanel.transform.Find("Title").gameObject.GetComponent<Text>();
        title.text = titleText;
        var content = endPanel.transform.Find("ContentText").gameObject.GetComponent<Text>();
        content.text = contentText;

        endPanel.SetActive(true);
    }

    private void FreezeGame()
    {
        var pc = m_Player.GetComponent<PlayerController>();
        pc.StopWalking();
        pc.enabled = false;
        m_Player.GetComponent<SneezeSystem>().enabled = false;
        m_Player.GetComponent<FartSystem>().enabled = false;
        m_Player.GetComponent<HungerSystem>().enabled = false;

    }

    private void ResetHardModeTimer() 
    {
        startHardModeTimer = false;
        hardModeTimer = TOTAL_GAME_TIME;
    }

    private int GenerateNumber(int size) 
    {
        return Random.Range(0, size - 1);
    }
}
