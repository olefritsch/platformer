using UnityEngine;
using Rewired;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    const string GroundIdentifierTag = "Ground";

    [HideInInspector]
    public int playerId = 0;

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

    [Header("Ability")]
    [SerializeField] float abilityCooldown = 1f;
    [SerializeField] float explosionForce = 500f;
    [SerializeField] float explosionRadius = 5f;

    private Player player;
    private Rigidbody rb;

    private float movementInput;
    private float timeSinceLastJump = 0;
    private int jumpStreak = 0;
    private bool canJump = true;
    private bool isJumping;

    private float timeSinceLastAbility;

    // Use this for initialization
    void Start()
    {
        player = ReInput.players.GetPlayer(playerId);
        rb = GetComponent<Rigidbody>();

        extraGravity *= -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (disabled)
            return;

        CheckInput();
    }

    private void FixedUpdate()
    {
        if (disabled)
            return;

        Vector3 movementForce = CalculateMovementForce(movementInput);
        rb.AddForce(movementForce);

        if (isJumping)
            Jump();

        if (extraGravity < 0)
            rb.AddForce(new Vector3(0, extraGravity, 0));

        if (player.controllers.hasMouse)
			HandleMouseControlledGunRotation();
        else 
            HandleJoystickControlledGunRotation();
    }

    private void CheckInput()
    {
        movementInput = player.GetAxis("Movement");
        isJumping = canJump && player.GetButton("Jump");

        if (player.GetButtonDown("Fire"))
            Shoot();

        if (player.GetButtonDown("Ability") && (Time.time - timeSinceLastAbility > abilityCooldown))
            UseAbility();
    }

    private Vector3 CalculateMovementForce(float inputMovementSpeed)
    {
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
        float inputHorizontal = player.GetAxis("Gun Horizontal");
        float inputVertical = player.GetAxis("Gun Vertical");

        if (inputHorizontal != 0 || inputVertical != 0)
        {
            float angle = Mathf.Atan2(-inputHorizontal, inputVertical) * Mathf.Rad2Deg;
            float rotateSpeed = 0;
            float smoothAngle = Mathf.SmoothDampAngle(gun.eulerAngles.z, angle, ref rotateSpeed, 0.06f);
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

    private void Shoot()
    {
        if (!projectilePrefab)
            return;

        Vector3 instantiatePos = gun.transform.position + (1.1f * gun.transform.up);
        Instantiate(projectilePrefab, instantiatePos, gun.transform.rotation);
    }

    private void UseAbility()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, 1 << LayerMask.NameToLayer("Players"));
        foreach (Collider collider in colliders)
        {
            // Don't add force to self
            if (collider.transform.root == transform)
                continue;

            Rigidbody rb = collider.GetComponentInParent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(explosionForce, this.transform.position, explosionRadius, 0.2f);
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
