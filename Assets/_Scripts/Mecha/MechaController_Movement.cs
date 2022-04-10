using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MechaMovementStates {
    NORMAL,
    ATTACKING
}

public class MechaController_Movement : MonoBehaviour {

    public static MechaController_Movement current;
    public MechaMovementStates mechaState = MechaMovementStates.NORMAL;

    [Header("Public variables:")]
    public Vector3 contactNormal, steepNormal;
    public float flyingFuelConsumptionRate = 1;

    [Header("Exposed privates:")]
    [SerializeField] private float maxMoveSpeed = 1f;
    [SerializeField] private float maxGroundAngle = 40f;
    [SerializeField] private float maxAcceleration = 40f;
    [SerializeField] private float maxJumpAcceleration = 20f;
    [SerializeField] private float flyingDirectionalForce = 10f;
    //[SerializeField] private float dashAcceleration = 30f;
    [Space]
    [SerializeField] private LayerMask probeMask = -1;
    [SerializeField] private GunnerCameraTarget camTarget = null;
    [SerializeField, Range(0f, 100f)]
    private float maxSnapSpeed = 50f;
    [SerializeField, Min(0f)]
    [Tooltip("Length of the downwards ray that determines if player sticks to the ground when running over a bump")]
    private float probeDistance = 1f;

    //private GameObject gunner;
    private MechaActivation activation;
    private MechaJump jump;
    private MechaFuel fuel;
    private MechaGravity gravity;
    private MechaSwordAttack attack;
    private MechaHP hp;
    private Rigidbody rb, connectedRb, previousConnectedRb;
    private Vector3 velocity, desiredVelocity;
    private Vector3 connectionWorldPos, connectionLocalPosition, connectionVelocity;
    public bool MechaGrounded => groundContactCount > 0;
    public bool MechaOnSteep => steepContactCount > 0;
    private int groundContactCount, steepContactCount, stepsSinceLastGrounded;
    private float minGroundDotProduct;
    private float moveSpeedMultiplier = 1;
    private float moveSlowReturnSpd = 1;
    private float xAxis, yAxis;


    void Start() {
        current = this;
        activation = GetComponent<MechaActivation>();
        jump = GetComponent<MechaJump>();
        fuel = GetComponent<MechaFuel>();
        /*
        grapple = GetComponent<PilotGrapple>();
        dash = GetComponent<PilotDash>();
        attack = GetComponent<PilotAttack>();
        */
        gravity = GetComponent<MechaGravity>();
        attack = GetComponent<MechaSwordAttack>();
        hp = GetComponent<MechaHP>();
        rb = GetComponent<Rigidbody>();
        OnValidate();
    }

    public void MechaUpdate_Gunner() {
        if (hp.AddHP(-Time.deltaTime * hp.hpDecayRate) < 0) {
            activation.DEACTIVATE();
        }

        Axes();
        MovementVector(xAxis);
        RotatePlayer(xAxis);
        /*
        if (pilotState == PilotStates.GRAPPLE) {
            grapple.UpdateGrapple();
            camTarget.MoveCameraTarget(xAxis);
            return;
        }*/
        attack.InitSlashAttack(new Vector2(xAxis, yAxis));
        camTarget.MoveCameraTarget_mecha(xAxis);
        //grapple.InitGrapple();
        jump.Jump();
        //dash.Dash();
        //attack.SwordAttack();
    }

    public void Mecha_Gunner_FixedUpdate() {
        UpdateState();
        AdjustVelocity();
        gravity.HandleGravity();
        Flying();
        ClearState();
    }

    void Axes() {
        xAxis = yAxis = 0;
        if (Keyboard.current.wKey.isPressed) yAxis += 1;
        if (Keyboard.current.aKey.isPressed) xAxis -= 1;
        if (Keyboard.current.sKey.isPressed) yAxis -= 1;
        if (Keyboard.current.dKey.isPressed) xAxis += 1;
    }

    void MovementVector(float xAxis) {
        desiredVelocity = Vector3.right * xAxis * maxMoveSpeed;
    }

    void RotatePlayer(float xAxis) {
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

        if (MechaGrounded) {
            acceleration = maxAcceleration;
        }
        /*else if (dash.currentDashCooldown > dash.dashCooldown * 0.5f) {
            acceleration = dashAcceleration;
        }*/
        else {
            acceleration = maxJumpAcceleration;
        }

        // Calculate and set the new velocity:
        float maxSpeedChange = acceleration * Time.deltaTime;
        Vector2 currentVelo = new Vector2(currentX, 0);
        Vector2 desiredVelo = new Vector2(desiredVelocity.x, 0);
        desiredVelo *= moveSpeedMultiplier;
        Vector2 newVelo = Vector2.MoveTowards(currentVelo, desiredVelo, maxSpeedChange);
        velocity += xAxis * (newVelo.x - currentX);
        rb.velocity = velocity;
    }

    void Flying() {
        if (!MechaGrounded) {
            if (xAxis < -0.2f || xAxis > 0.2f) {
                if (fuel.ConsumeFuel(flyingFuelConsumptionRate)) {
                    rb.AddForce(Vector3.right * xAxis * flyingDirectionalForce * Time.deltaTime, ForceMode.Impulse);
                }
            }

        }
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
        if (MechaGrounded || SnapToGround()) {
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
            Vector3 normal = col.GetContact(i).normal;
            if (normal.y >= minGroundDotProduct) {
                if (!jump.justJumped_mecha) {
                    jump.lateJumpTimer_mecha = 0;
                }

                /*if (dash.currentDashCooldown <= 0) {
                    dash.ToggleCanDash(true);
                }
                grapple.ToggleCanGrapple(true);
                */
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
        /*
        if (pilotState == PilotStates.DASH) {
            return false;
        }
        */
        if (jump.justJumped_mecha) {
            return false;
        }

        float speed = velocity.magnitude;
        if (speed > maxSnapSpeed) {
            return false;
        }

        if (!Physics.Raycast(rb.position, Vector3.down, out RaycastHit hit, probeDistance, probeMask)) {
            return false;
        }

        if (hit.normal.y < minGroundDotProduct) {   // GetMinDot(hit.collider.gameObject.layer)) {
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
        //minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
        //minSlipperyDotProd = Mathf.Cos(maxSlipperyAngle * Mathf.Deg2Rad);
        //minMediumSlipDotProd = Mathf.Cos(maxMediumSlipAngle * Mathf.Deg2Rad);
    }

    public async void SlowMovement(float targetSpeedMultiplier, float returnSpeed) {
        if (moveSpeedMultiplier > targetSpeedMultiplier) {
            moveSpeedMultiplier = targetSpeedMultiplier;
        }

        if (moveSlowReturnSpd > returnSpeed) {
            moveSlowReturnSpd = returnSpeed;
        }
        else {
            moveSlowReturnSpd = (moveSlowReturnSpd + returnSpeed) * 0.5f;
        }

        while (moveSpeedMultiplier < 1) {
            moveSpeedMultiplier += Time.deltaTime * moveSlowReturnSpd;
            await Task.Yield();
        }
    }

    public void SetGunner(GameObject gunner) {
        //this.gunner = gunner;
    }
}
