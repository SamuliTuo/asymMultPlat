using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PilotStates {
    NORMAL,
    GRAPPLE,
    DASH,
    IN_MECHA,
    NOT_IN_CONTROL
}

public class PilotController : MonoBehaviour {

    [Header("Public variables:")]
    public PilotStates pilotState = PilotStates.NORMAL;
    public Vector3 contactNormal, steepNormal;
    public bool swordIsInHand = true;

    [Header("Exposed privates:")]
    [SerializeField] private float maxMoveSpeed = 1f;
    [SerializeField] private float maxGroundAngle = 40f;
    [SerializeField] private float maxAcceleration = 40f;
    [SerializeField] private float maxJumpAcceleration = 20f;
    [SerializeField] private float dashAcceleration = 30f;
    [Space]
    [SerializeField] private LayerMask probeMask = -1;
    [SerializeField] private PilotCameraTarget camTarget = null;
    [SerializeField, Range(0f, 100f)]
    private float maxSnapSpeed = 50f;
    [SerializeField, Min(0f)]
    [Tooltip("Length of the downwards ray that determines if player sticks to the ground when running over a bump")]
    private float probeDistance = 1f;

    private PilotJump jump;
    private PilotGrapple grapple;
    private PilotDash dash;
    private PilotAttack attack;
    private PilotSwordThrow swordThrow;
    private PilotItems items;
    private PilotGravity gravity;
    private PilotHP hp;
    private Rigidbody rb, connectedRb, previousConnectedRb;
    private Vector3 velocity, desiredVelocity;
    private Vector3 connectionWorldPos, connectionLocalPosition, connectionVelocity;
    public bool PilotGrounded => groundContactCount > 0;
    public bool PilotOnSteep => steepContactCount > 0;
    private int groundContactCount, steepContactCount, stepsSinceLastGrounded;
    private float minGroundDotProduct;
    private float xAxis;


    void Start() {
        jump = GetComponent<PilotJump>();
        grapple = GetComponent<PilotGrapple>();
        dash = GetComponent<PilotDash>();
        attack = GetComponent<PilotAttack>();
        swordThrow = GetComponent<PilotSwordThrow>();
        items = GetComponent<PilotItems>();
        gravity = GetComponent<PilotGravity>();
        hp = GetComponent<PilotHP>();
        rb = GetComponent<Rigidbody>();
        OnValidate();
    }

    void Update() {
        if (!swordIsInHand) {
            swordThrow.UpdateSwordThrow();
        }
        if (pilotState == PilotStates.NOT_IN_CONTROL) {
            return;
        }
        // Playing in mecha
        if (pilotState == PilotStates.IN_MECHA) {
            MechaController_CannonPilot.current.MechaUpdate_Pilot();
            return;
        }
        MechaActivation.current.ActivateMecha_Pilot();

        // Playing as pilot
        xAxis = Gamepad.current.leftStick.ReadValue().x;
        if (xAxis > -0.2f && xAxis < 0.2f) {
            xAxis = 0;
        }
        MovementVector();
        RotatePlayer();
        items.UpdateItems();
        swordThrow.InitSwordThrow();
        attack.InitSlashAttack();
        camTarget.MoveCameraTarget(xAxis);
        if (pilotState == PilotStates.GRAPPLE) {
            grapple.UpdateGrapple();
            return;
        }
        grapple.InitGrapple();
        jump.Jump();
        dash.Dash();
    }

    void FixedUpdate() {
        if (pilotState == PilotStates.IN_MECHA) {
            return;
        }
        UpdateState();
        AdjustVelocity();
        gravity.HandleGravity();
        ClearState();
    }

    /////////////////////////////////////////////////////////////////////////////// 

    void MovementVector() {
        desiredVelocity = Vector3.right * xAxis * maxMoveSpeed;
    }

    void RotatePlayer() {
        if (xAxis > 0.1f) {
            transform.forward = Vector3.back;
        }
        else if (xAxis < -0.1f) {
            transform.forward = Vector3.forward;
        }
    }

    void AdjustVelocity() {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 relativeVelocity = velocity - connectionVelocity;
        float currentX = Vector3.Dot(relativeVelocity, xAxis);

        // Set the value for acceleration:
        float acceleration;
        if (pilotState == PilotStates.NOT_IN_CONTROL) {
            acceleration = 0;
        }
        else if (pilotState == PilotStates.GRAPPLE) {
            acceleration = 100;
        }
        else if (PilotGrounded) {
            acceleration = maxAcceleration;
        }
        else if (dash.currentDashCooldown > dash.dashCooldown * 0.5f) {
            acceleration = dashAcceleration;
        }
        else {
            acceleration = maxJumpAcceleration;
        }
        
        // Calculate and set the new velocity:
        float maxSpeedChange = acceleration * Time.deltaTime;
        Vector2 currentVelo = new Vector2(currentX, 0);
        Vector2 desiredVelo = new Vector2(desiredVelocity.x, 0);
        Vector2 newVelo = Vector2.MoveTowards(currentVelo, desiredVelo, maxSpeedChange);
        velocity += xAxis * (newVelo.x - currentX);
        rb.velocity = velocity * grapple.anticipationMoveSpeedMultiplier;
    }

    void UpdateConnectionState() {
        if (connectedRb == previousConnectedRb) {
            Vector3 connectionMovement = connectedRb.transform.TransformPoint(connectionLocalPosition) - connectionWorldPos;
            connectionVelocity = connectionMovement / Time.deltaTime;
        }
        connectionWorldPos = rb.position;
        connectionLocalPosition = connectedRb.transform.InverseTransformPoint(connectionWorldPos);
    }

    Vector3 ProjectOnContactPlane(Vector3 v) {
        return v - contactNormal * Vector3.Dot(v, contactNormal);
    }

    void UpdateState() {
        stepsSinceLastGrounded += 1;
        velocity = rb.velocity;
        if (PilotGrounded || SnapToGround()) {
            stepsSinceLastGrounded = 0;
            if (groundContactCount > 1) {
                contactNormal.Normalize();
            }
        }
        if (connectedRb) {
            UpdateConnectionState();
        }
    }


    void ClearState() {
        groundContactCount = steepContactCount = 0;
        contactNormal = steepNormal = connectionVelocity = Vector3.zero;
        previousConnectedRb = connectedRb;
        connectedRb = null;
    }

    void OnCollisionEnter(Collision collision) {
        EvaluateCollision(collision);
    }

    void OnCollisionStay(Collision collision) {
        EvaluateCollision(collision);
    }

    void EvaluateCollision(Collision col) {
        //contactPoint = col.GetContact(0).point;
        for (int i = 0; i < col.contactCount; i++) {
            if (col.collider.tag == "Spike") {
                hp.Hurt(hp.damageTakeFromSpikes, col.GetContact(i).point);
            }
            Vector3 normal = col.GetContact(i).normal;
            if (normal.y >= minGroundDotProduct) {
                if (!jump.justJumped) {
                    jump.lateJumpTimer = 0;
                }
                if (dash.currentDashCooldown <= 0) {
                    dash.ToggleCanDash(true);
                }
                grapple.ToggleCanGrapple(true);
                gravity.swordJumped = false;
                groundContactCount += 1;
                contactNormal += normal;
                connectedRb = col.rigidbody;
            }
            else if (normal.y > -0.5f) {
                steepContactCount += 1;
                steepNormal += normal;
                if (groundContactCount == 0) {
                    connectedRb = col.rigidbody;
                }
            }
        }
    }

    bool SnapToGround() {
        if (stepsSinceLastGrounded > 1) {
            return false;
        }
        if (pilotState == PilotStates.DASH) {
            return false;
        }
        if (jump.justJumped) {
            return false;
        }   
        float speed = velocity.magnitude;
        if (speed > maxSnapSpeed) {
            return false;
        }
        if (!Physics.Raycast(rb.position, Vector3.down, out RaycastHit hit, probeDistance, probeMask)) {
            return false;
        }
        if (hit.normal.y < minGroundDotProduct) {
            return false;
        }
        groundContactCount = 1;
        contactNormal = hit.normal;
        float dot = Vector3.Dot(velocity, hit.normal);
        if (dot > 0f) {
            velocity = (velocity - hit.normal * dot).normalized * speed;
        }
        connectedRb = hit.rigidbody;
        return true;
    }

    void OnValidate() {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    public void ResetPilot() {
        xAxis = 0;
        desiredVelocity = Vector3.zero;
        dash.StopAllCoroutines();
        grapple.EndGrappleEarly();
    }

    public void SetupPilotForMechaMode() {
        pilotState = PilotStates.IN_MECHA;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ReturnFromMechaMode() {
        pilotState = PilotStates.NORMAL;
        transform.GetChild(0).gameObject.SetActive(true);
    }
}