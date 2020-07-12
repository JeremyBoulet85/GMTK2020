using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField]
    private int totalKeys = 4;

    [SerializeField]
    private int totalStrikes = 3;

    public bool Finished { get; private set; } = false;
    public bool ShouldRespawn { get; private set; } = false;
    public int CurrentKeys { get; private set; } = 0;
    public int TotalKeys { get => totalKeys; }
    public int StrikeCount { get; private set; } = 0;
    public int TotalStrikes { get => totalStrikes; }

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
    }

    public void Respawn()
    {
        ShouldRespawn = false;
        StrikeCount = 0;
        m_Player.transform.position = m_PlayerSpawnTransform.position;
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
        m_PlayerSpawnTransform = GameObject.Find("PlayerSpawn").transform;
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

    public void GetStriked()
    {
        ++StrikeCount;
        ShouldRespawn = StrikeCount >= TotalStrikes;
    }

    public void PickUpKey()
    {
        ++CurrentKeys;
        Finished = CurrentKeys == totalKeys;
    }
}
