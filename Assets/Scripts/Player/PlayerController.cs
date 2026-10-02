using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float reverseSpeed = 3.5f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 180f;

    [Header("Inertia")]
    [SerializeField] private float drag = 1.5f;
    [SerializeField] private float reverseDrag = 5f;

    [Header("Mobile")]
    [SerializeField] private MobileJoystick joystick;

    [Header("Boost")]
    [SerializeField] private float boostMultiplier = 1.6f;
    [SerializeField] private float maxBoostEnergy = 100f;
    [SerializeField] private float boostDrain = 35f;
    [SerializeField] private float boostRegen = 20f;
private float previousThrottle;
private float thrustSoundCooldown;
    public Vector2 Velocity =>
        rb != null
            ? rb.linearVelocity
            : Vector2.zero;

    public Vector2 Forward =>
        transform.right;

    public float BoostEnergy =>
        boostEnergy;

    public float MaxBoostEnergy =>
        maxBoostEnergy;

    public bool IsBoosting =>
        boostPressed &&
        boostEnergy > 0.01f;

    private Rigidbody2D rb;

    private float throttle;
    private float steering;

    private float boostEnergy;
    private bool boostPressed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;

        boostEnergy = maxBoostEnergy;
    }

    private void Update()
    {

        ReadInput();
        HandleThrustAudio();
        HandleBoostEnergy();
    }
    private void HandleThrustAudio()
{
    thrustSoundCooldown -=
        Time.deltaTime;

    bool startedThrust =
        Mathf.Abs(throttle) > 0.1f &&
        Mathf.Abs(previousThrottle) <= 0.1f;

    if (startedThrust &&
        thrustSoundCooldown <= 0f)
    {
        ProceduralAudioManager.Instance?.PlaySfx(
            AudioSfxType.EngineThrust,
            0.55f
        );

        thrustSoundCooldown = 0.12f;
    }

    previousThrottle =
        throttle;
}

    private void FixedUpdate()
    {
        HandleRotation();
        HandleThrust();
        ApplyDrag();
    }

    private void ReadInput()
    {
        throttle = 0f;
        steering = 0f;

        // =========================
        // MOBILE
        // =========================

        if (joystick != null &&
            joystick.Input.sqrMagnitude > 0.01f)
        {
            steering =
                joystick.Input.x;

            throttle =
                joystick.Input.y;

            return;
        }

        // =========================
        // KEYBOARD
        // =========================

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.wKey.isPressed ||
            Keyboard.current.upArrowKey.isPressed)
        {
            throttle += 1f;
        }

        if (Keyboard.current.sKey.isPressed ||
            Keyboard.current.downArrowKey.isPressed)
        {
            throttle -= 1f;
        }

        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
        {
            steering += 1f;
        }

        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            steering -= 1f;
        }
    }

    private void HandleRotation()
    {
        if (Mathf.Abs(steering) < 0.01f)
            return;

        float rotation =
            -steering *
            rotationSpeed *
            Time.fixedDeltaTime;

        rb.MoveRotation(
            rb.rotation + rotation
        );
    }

    private void HandleThrust()
    {
        if (Mathf.Abs(throttle) < 0.01f)
            return;

        Vector2 forward =
            transform.right;

        float currentForwardSpeed =
            Vector2.Dot(
                rb.linearVelocity,
                forward
            );

        float normalMaxSpeed =
            GetMaxSpeed();

        float currentMaxSpeed =
            IsBoosting
                ? normalMaxSpeed *
                  boostMultiplier
                : normalMaxSpeed;

        float targetSpeed =
            throttle > 0f
                ? currentMaxSpeed
                : -reverseSpeed;

        float accelerationAmount =
            acceleration *
            Time.fixedDeltaTime;

        if (IsBoosting)
        {
            accelerationAmount *= 1.4f;
        }

        float newForwardSpeed =
            Mathf.MoveTowards(
                currentForwardSpeed,
                targetSpeed,
                accelerationAmount
            );

        float speedChange =
            newForwardSpeed -
            currentForwardSpeed;

        rb.AddForce(
            forward *
            speedChange,
            ForceMode2D.Impulse
        );
    }

    private void ApplyDrag()
    {
        Vector2 velocity =
            rb.linearVelocity;

        if (velocity.sqrMagnitude <
            0.001f)
        {
            rb.linearVelocity =
                Vector2.zero;

            return;
        }

        float currentForwardSpeed =
            Vector2.Dot(
                velocity,
                transform.right
            );

        float currentSideSpeed =
            Vector2.Dot(
                velocity,
                transform.up
            );

        currentSideSpeed =
            Mathf.MoveTowards(
                currentSideSpeed,
                0f,
                drag *
                Time.fixedDeltaTime
            );

        float selectedDrag =
            currentForwardSpeed < 0f
                ? reverseDrag
                : drag;

        if (Mathf.Abs(throttle) < 0.01f)
        {
            currentForwardSpeed =
                Mathf.MoveTowards(
                    currentForwardSpeed,
                    0f,
                    selectedDrag *
                    Time.fixedDeltaTime
                );
        }

        Vector2 newVelocity =
            transform.right *
            currentForwardSpeed +
            transform.up *
            currentSideSpeed;

        rb.linearVelocity =
            newVelocity;

        float maxSpeed =
            GetMaxSpeed();

        if (IsBoosting)
        {
            maxSpeed *=
                boostMultiplier;
        }

        if (rb.linearVelocity.magnitude >
            maxSpeed)
        {
            rb.linearVelocity =
                rb.linearVelocity.normalized *
                maxSpeed;
        }
    }

    private float GetMaxSpeed()
    {
        if (PlayerStats.Instance != null)
            return PlayerStats.Instance.MoveSpeed;

        return 7f;
    }

    private void HandleBoostEnergy()
    {
        if (IsBoosting)
        {
            boostEnergy -=
                boostDrain *
                Time.deltaTime;

            boostEnergy =
                Mathf.Max(
                    0f,
                    boostEnergy
                );
        }
        else
        {
            boostEnergy +=
                boostRegen *
                Time.deltaTime;

            boostEnergy =
                Mathf.Min(
                    maxBoostEnergy,
                    boostEnergy
                );
        }
    }

    public void SetBoost(bool value)
{
    bool oldState =
        boostPressed;

    boostPressed =
        value &&
        boostEnergy > 0.01f;

    if (!oldState &&
        boostPressed)
    {
        if (ProceduralAudioManager.Instance != null)
        {
            ProceduralAudioManager.Instance.PlaySfx(
                AudioSfxType.BoostStart
            );
        }
    }
}
}