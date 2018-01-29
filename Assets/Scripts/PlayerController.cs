using UnityEngine;
using Rewired;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

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

	[HideInInspector]
	public int playerId = 0;

    // Reference variables
    private Player player;
    private Rigidbody rb;
    private Renderer primaryRenderer;
    private Renderer secondaryRenderer;

    // Input variables
    private float movementInput;
    private bool isJumping;
    private bool isShooting;
    private bool isUsingAbility;

    private float timeSinceLastAbility = 0;
    private float timeSinceLastJump = 0;
    private int jumpStreak = 0;
    private bool canJump = true;

    // Use this for initialization
    void Start()
    {
		player = ReInput.players.GetPlayer(playerId);
        player.AddInputEventDelegate(OnChangePrimaryColor, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Next Primary Color");
        player.AddInputEventDelegate(OnChangeSecondaryColor, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Next Secondary Color");

        rb = GetComponent<Rigidbody>();

        // We can assume that the primary renderer will be found first as it is located higher up in the hierachy
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        primaryRenderer = renderers[0];
        secondaryRenderer = renderers[1];

        extraGravity *= -1;

        // Register for callbacks when the game state is changed
        GameManager.OnGameStateChange += OnGameStateChange;
        // TODO: Removed this once proper player joining/spawning has been implemented
        OnGameStateChange(GameManager.Instance.GameState);

        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        if (extraGravity < 0)
            rb.AddForce(new Vector3(0, extraGravity, 0));

        ApplyInput();
    }

    // Get player input from Rewired, called in Update
    private void GetInput()
    {
        movementInput = player.GetAxis("Movement");

        if (player.GetButton("Jump"))
            isJumping = true;

        if (player.GetButtonDown("Fire"))
            isShooting = true;

        if (player.GetButtonDown("Ability"))
            isUsingAbility = true;
    }

    // Applies player input, called from FixedUpdate due to physics calculations
    private void ApplyInput()
    {
        Vector3 movementForce = CalculateMovementForce(movementInput);
        rb.AddForce(movementForce);

        if (player.controllers.hasMouse)
            HandleMouseControlledGunRotation();
        else
            HandleJoystickControlledGunRotation();

        if (isJumping)
        {
            isJumping = false;
            Jump();
        }

        if (isShooting)
        {
            isShooting = false;
            Shoot();
        }

        if (isUsingAbility)
        {
            isUsingAbility = false;
            UseAbility();
        }
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
        if (!canJump || Time.time - timeSinceLastJump < jumpDelay)
            return;

        rb.AddForce(0, jumpPower, 0);
        timeSinceLastJump = Time.time;
        jumpStreak++;

        if (jumpStreak >= jumpChainLimit)
            canJump = false;
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
        if (Time.time - timeSinceLastAbility < abilityCooldown)
            return;

		int playerLayer = 1 << LayerMask.NameToLayer(TagManager.PlayerLayer);
		int projectileLayer = 1 << LayerMask.NameToLayer(TagManager.ProjectileLayer);
		Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, playerLayer | projectileLayer);
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
        if (collision.gameObject.tag == TagManager.GroundIdentifierTag)
        {
            jumpStreak = 0;
            this.canJump = true;
        }
    }

    // Enable corresponding control maps on game state change, disable all others
    public void OnGameStateChange(GameState gameState)
    {
        player.controllers.maps.SetAllMapsEnabled(false);
        player.controllers.maps.SetMapsEnabled(true, gameState.ToString());
    }

    private void OnChangePrimaryColor(InputActionEventData data) 
    {
        primaryRenderer.material.color = Random.ColorHSV();
    }

    private void OnChangeSecondaryColor(InputActionEventData data)
    {
        secondaryRenderer.material.color = Random.ColorHSV();
    }

    private void OnDestroy()
    {
        // Deregister event handler to avoid memory leak
        GameManager.OnGameStateChange -= OnGameStateChange;

        player.RemoveInputEventDelegate(OnChangePrimaryColor);
        player.RemoveInputEventDelegate(OnChangeSecondaryColor);
    }
}
