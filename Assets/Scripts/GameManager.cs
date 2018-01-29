using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public static GameManager Instance = null;

	public delegate void GameStateChanged(GameState gameState);
	public static event GameStateChanged OnGameStateChange; 

	public GameState GameState 
	{ 
		get { return _gameState; }
		private set 
		{
			_gameState = value;
			if (OnGameStateChange != null)
				OnGameStateChange(value);
		}
	}
	private GameState _gameState;

	[SerializeField] GameObject playerPrefab;

    private List<PlayerController> players;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
		if (SceneManager.GetActiveScene().name == "_Game")
			GameState = GameState.Gameplay;
		else
			GameState = GameState.Setup;

		PlayerDetector.OnPlayerJoin += OnPlayerJoin;
        Shredder.PlayerDeath += OnPlayerDeath;
        players = new List<PlayerController>();

        // TODO: Removed this once proper player joining/spawning has been implemented
        if (SceneManager.GetActiveScene().name == "_Game")
        {
            OnPlayerJoin(0);
            OnPlayerJoin(1);   
        }
    }

	public void OnStartGame()
	{
		GameState = GameState.Gameplay;
		SceneManager.LoadScene("_Game");
	}

	public void OnPlayerJoin(int playerId)
    {
        Vector3 spawnPos = new Vector3(-8f + (playerId * 5), 4f, 0f);
        GameObject playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        PlayerController player = playerObj.GetComponent<PlayerController>();

		player.playerId = playerId;
        players.Add(player);
    }

	public void OnPlayerDeath(int playerId)
    {
        for (int i=0, len=players.Count;  i<len; i++)
        {
            if (players[i].playerId == playerId)
            {
                PlayerController player = players[i];
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                player.transform.position = new Vector3(0, 10, 0);
            }
        }
    }
}
