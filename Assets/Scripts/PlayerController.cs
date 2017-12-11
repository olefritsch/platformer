using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    const string GroundIdentifierTag = "Ground";

    [Header("Testing Only")]        // For testing purposes only,
    public bool disabled;           // Disables player controls                 

    [Header("Movement")]
    [SerializeField] float maxMovementSpeed = 10f;
    [SerializeField] float jumpPower = 500f;
    [SerializeField] float jumpDelay = 0.4f;
    [SerializeField] int jumpChainLimit = 3;
    [SerializeField] float extraGravity = 10;

    private Rigidbody rb;
    private int playerNumber = 1;
    private string playerId;

    private float timeSinceLastJump = 0;
    private int jumpStreak = 0;
    private bool canJump = true;


    // Use this for initialization
    void Start()
    {
        playerId = "P" + playerNumber;
        rb = GetComponent<Rigidbody>();

        extraGravity *= -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (disabled)
            return;
    }

    private void FixedUpdate()
    {
        if (disabled)
            return;

        Vector3 movementForce = CalculateMovementForce();
        rb.AddForce(movementForce);

        if (extraGravity < 0)
            rb.AddForce(new Vector3(0, extraGravity, 0));

        if (canJump && Input.GetButton(playerId + " Jump"))
            Jump();
    }

    private Vector3 CalculateMovementForce()
    {
        float inputMovementSpeed = Input.GetAxis(playerId + " Movement");
        float currentMovementSpeed = rb.velocity.x;
        float speedModifier = 1;

        if (inputMovementSpeed > 0)
            speedModifier = maxMovementSpeed - currentMovementSpeed;
        else if (inputMovementSpeed < 0)
            speedModifier = Mathf.Abs(-maxMovementSpeed - currentMovementSpeed);

        float movementForce = inputMovementSpeed * speedModifier * 250 * Time.deltaTime;
        return new Vector3(movementForce, 0, 0);
    }

    private void Jump()
    {
        if (Time.time - timeSinceLastJump > jumpDelay)
        {
            rb.AddForce(0, jumpPower, 0);
            timeSinceLastJump = Time.time;
            jumpStreak++;

            if (jumpStreak >= jumpChainLimit)
                canJump = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == GroundIdentifierTag)
        {
            jumpStreak = 0;
            this.canJump = true;
        }
    }
}
