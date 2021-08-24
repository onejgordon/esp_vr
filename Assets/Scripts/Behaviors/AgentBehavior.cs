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


    void Start()
    {
        this.velocity = this.baseVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(new Vector3(0, 0, velocity));

        if(Keyboard.current.leftArrowKey.isPressed) {
            this.turn(-1);
        } else if (Keyboard.current.rightArrowKey.isPressed) {
            this.turn(1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with " + other.gameObject.name);
        if (other.gameObject.CompareTag("obstacle")) {
            experimentRunner.EndTrial();
        } else if (other.gameObject.CompareTag("tile")) {
            TileBehavior tb = other.gameObject.GetComponent<TileBehavior>();
            float velocity_mult = tb.tileVelocityMult();
            this.velocity = this.baseVelocity * velocity_mult;
            Debug.Log("Moved into tile, new velocity " + velocity_mult.ToString());
        }
    }

    public void turn(int direction) {
        gameObject.transform.Rotate(Vector3.up, direction * rotationSpeed);
    }

    public Vector3 overheadCameraPosition() {
        return new Vector3(gameObject.transform.position.x, Constants.CAM_NAV_HEIGHT, gameObject.transform.position.z - Constants.CAM_NAV_BEHIND_DIST);
    }

    public float getHeading() {
        return gameObject.transform.eulerAngles.y;
    }

}
