using UnityEngine;

public class FollowPlayers : MonoBehaviour {

    private PlayerController[] players;

    // Use this for initialization
    void Start()
    {
        players = FindObjectsOfType<PlayerController>();
    }

    private void FixedUpdate()
    {
        Vector3 playerPositions = Vector3.zero;

        for (int i=0, len=players.Length; i<len; i++)
        {
            PlayerController player = players[i];
            playerPositions += player.transform.position;
        }

        playerPositions = playerPositions.normalized / 5;
        playerPositions.y += 5;

        float speedX = 0;
        float speedY = 0;

        float posX = Mathf.SmoothDamp(transform.position.x, playerPositions.x, ref speedX, 0.5f);
        float posY = Mathf.SmoothDamp(transform.position.y, playerPositions.y, ref speedY, 0.5f);

        transform.position = new Vector3(posX, posY, transform.position.z);
        transform.LookAt(new Vector3(posX, posY, playerPositions.z));
    }
}
