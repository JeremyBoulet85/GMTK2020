using System;
using System.Collections;
using System.Collections.Generic;
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

    public void Respawn()
    {
        ShouldRespawn = false;
        StrikeCount = 0;
        player.transform.position = startingPoint.position;
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
