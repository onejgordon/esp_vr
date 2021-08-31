using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AgentBehavior : MonoBehaviour
{
    private string id;
    public float baseVelocity = 0.01f;
    private float velocity;
    public float rotationSpeed = 1.0f;
    public ExperimentRunner experimentRunner;
    public bool debug_static = false;

    void Start()
    {
        this.velocity = this.baseVelocity;
    }

    // Update is called once per frame
    void Update()
    {   
        if (!this.debug_static) gameObject.transform.Translate(new Vector3(0, 0, velocity));

        if(Keyboard.current.leftArrowKey.isPressed) {
            this.turn(-1);
        } else if (Keyboard.current.rightArrowKey.isPressed) {
            this.turn(1);
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
        Debug.Log("Collided with " + collideGameObject.name);
        if (collideGameObject.CompareTag("obstacle")) {
            experimentRunner.EndTrial();
        } else if (collideGameObject.CompareTag("tile")) {
            TileBehavior tb = collideGameObject.GetComponent<TileBehavior>();
            float velocity_mult = tb.tileVelocityMult();
            this.velocity = this.baseVelocity * velocity_mult;
            Debug.Log("Moved into tile, new velocity " + velocity_mult.ToString());
        } else if (collideGameObject.CompareTag("reward")) {
            RewardBehavior rb = collideGameObject.GetComponent<RewardBehavior>();
            rb.consume();
            this.experimentRunner.getCurrentTrial().reward += 1;
        }
    }

    public void turn(int direction) {
        gameObject.transform.Rotate(Vector3.up, direction * rotationSpeed);
    }

    public float getHeading() {
        return gameObject.transform.eulerAngles.y;
    }

}
