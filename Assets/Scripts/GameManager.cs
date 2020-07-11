using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField]
    private int totalKeys = 4;
    public int StrikeCount { get; set; }
    public bool Finished { get; private set; } = false;
    public int CurrentKeys { get; private set; } = 0;
    public int TotalKeys { get => totalKeys; }

    private Transform player;

    public Transform startingPoint;

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

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    //Initializes the game for each level.
    void InitGame()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStrikeCount();
    }

    void UpdateStrikeCount()
    {
        if (StrikeCount >= 3)
        {
            player.transform.position = startingPoint.position;
            StrikeCount = 0;
        }
    }

    public void PickUpKey()
    {
        ++CurrentKeys;
        Finished = CurrentKeys == totalKeys;
    }
}
