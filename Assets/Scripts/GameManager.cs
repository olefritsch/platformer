using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public static GameManager Instance = null;

    [SerializeField] GameObject playerPrefab;

    public enum GameState { Menu, GameSetup, GamePlay, GameEnd }
    private GameState state;

    private List<GameObject> players;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this.gameObject);
    }

    void Start()
    {
        players = new List<GameObject>();
        OnPlayerJoin();
        OnPlayerJoin();
    }

    public void OnPlayerJoin()
    {
        Vector3 spawnPos = new Vector3(-10f + (players.Count * 4), 0f, 0f);
        GameObject playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        
        // TODO: Allow players to choose abilities before game starts
        PlayerController player = playerObj.GetComponent<PlayerController>();

        player.playerId = players.Count;
        players.Add(playerObj);
    }
}
