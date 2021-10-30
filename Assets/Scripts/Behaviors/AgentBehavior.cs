using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Valve.VR;

public class AgentBehavior : MonoBehaviour
{
    private string id;
    public float baseVelocity = 0.075f;
    private float velocity;
    private bool moving = false;
    public float rotationSpeed = 1.0f;
    public ExperimentRunner experimentRunner;
    public bool debug_static = false;
    public Transform trController;
    private Transform trControllerIndicator;
    public float turnAngleMin = 20.0f;
    public float turnAngleMax = 40.0f;
    public float turnRate = 0.0f;
    private Renderer _renderer;
    private Color turningColor = Color.white;
    private Color notTurningColor = Color.red;

    void Start()
    {
        this.trControllerIndicator = gameObject.transform.Find("ControllerIndicator").GetComponent<Transform>();
        this._renderer = trControllerIndicator.gameObject.GetComponentInChildren<Renderer>();
        this.velocity = this.baseVelocity;
    }

    // Update is called once per frame
    void Update()
    {   
        if (this.experimentRunner.isNavigating()) {
            this.turnAndTurnIndicator();
            bool forward = SteamVR_Actions._default.GoForward[SteamVR_Input_Sources.RightHand].state;
            if (forward) {
                this.moving = true;
            } else {
                this.moving = false;
            }

            if (moving) {
                gameObject.transform.Translate(new Vector3(0, 0, velocity));
            }

        }
    }

    public void turnAndTurnIndicator() {
        // Get yaw from center in (-180, 180)
        float controllerYaw = this.trController.localEulerAngles.y;
        if (controllerYaw > 180.0f) controllerYaw -= 360.0f;

        // Update turn indicator
        float clampedYaw = Mathf.Clamp(controllerYaw, -this.turnAngleMax, this.turnAngleMax);
        this.trControllerIndicator.localRotation = Quaternion.Euler(0, clampedYaw, 0);

        // Perform turns
        if (controllerYaw > this.turnAngleMax) {
            this.turn(1);
        } else if (controllerYaw > this.turnAngleMin) {
            this.turn(0.5f);
        } else if (controllerYaw < -this.turnAngleMax) {
            this.turn(-1);
        } else if (controllerYaw < -this.turnAngleMin) {
            this.turn(-0.5f);
        } else {
            this.turn(0.0f);
        }
    }

    public void turn(float tr) {
        bool rateChange = tr != this.turnRate;
        this.turnRate = tr;
        if (tr != 0.0f) gameObject.transform.Rotate(Vector3.up, tr * rotationSpeed);
        if (rateChange) {
            // Maybe change mat
            if (tr != 0.0f) this._renderer.material.SetColor("_Color", this.turningColor);
            else this._renderer.material.SetColor("_Color", this.notTurningColor);
        }
    }

    public Transform getTransform() {
        return gameObject.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        this.handleCollision(other.gameObject);
    }

    public void handleCollision(GameObject collideGameObject) {
        if (collideGameObject.name.Contains("Tile")) {
            // Parent is TileX which may caontain obstacle tag
            collideGameObject = collideGameObject.transform.parent.gameObject;
        }
        // Debug.Log("Collided with " + collideGameObject.name + " with tag: " + collideGameObject.tag);
        if (collideGameObject.CompareTag("obstacle")) {
            Debug.Log("Hit obstacle");
            experimentRunner.EndTrial();
        } else if (collideGameObject.CompareTag("tile")) {
            TileBehavior tb = collideGameObject.GetComponent<TileBehavior>();
            float velocity_mult = tb.tileVelocityMult();
            this.velocity = this.baseVelocity * velocity_mult;
            // Debug.Log("Moved into tile, new velocity " + velocity_mult.ToString());
        } else if (collideGameObject.CompareTag("reward")) {
            RewardBehavior rb = collideGameObject.GetComponentInParent<RewardBehavior>();
            rb.consume();
            SessionTrial trial = this.experimentRunner.getCurrentTrial();
            if (trial.allRewardsCollected()) {
                if (trial.navigationSecondsRemaining() > 5.0f) {
                    // If enough time remaining, schedule premature end
                    StartCoroutine(WaitThenEnd(3));
                }
            }
        }
    }

    IEnumerator WaitThenEnd(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        experimentRunner.EndTrial();
    }

    public float getHeading() {
        return gameObject.transform.eulerAngles.y;
    }

}
