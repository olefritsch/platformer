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

    [Header("Firing")]
    [SerializeField] Transform gun;
    [SerializeField] GameObject projectilePrefab;

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

        if (Input.GetButtonDown(playerId + " Shoot"))
            Shoot();
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

        // HandleJoystickControlledGunRotation();
        HandleMouseControlledGunRotation();
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

    private void HandleJoystickControlledGunRotation()
    {
        float inputHorizontal = Input.GetAxis(playerId + " Gun Horizontal");
        float inputVertical = Input.GetAxis(playerId + " Gun Vertical");

        if (inputHorizontal != 0 || inputVertical != 0)
        {
            float angle = Mathf.Atan2(-inputHorizontal, inputVertical) * Mathf.Rad2Deg;
            float rotateSpeed = 0;
            float smoothAngle = Mathf.SmoothDampAngle(gun.eulerAngles.z, angle, ref rotateSpeed, 0.08f);
            gun.rotation = Quaternion.Euler(new Vector3(0, 0, smoothAngle));
        }
    }

    private void HandleMouseControlledGunRotation()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.z;
        Vector3 objectPos = Camera.main.WorldToScreenPoint(gun.position);

        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg - 90;
        float rotateSpeed = 0;
        float smoothAngle = Mathf.SmoothDampAngle(gun.eulerAngles.z, angle, ref rotateSpeed, 0.05f);
        gun.rotation = Quaternion.Euler(new Vector3(0, 0, smoothAngle));
    }

    private void Shoot()
    {
        if (!projectilePrefab)
            return;

        Vector3 instantiatePos = gun.transform.position + (1.1f * gun.transform.up);
        Instantiate(projectilePrefab, instantiatePos, gun.transform.rotation);
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
