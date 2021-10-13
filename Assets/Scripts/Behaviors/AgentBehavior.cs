﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Valve.VR;

public class AgentBehavior : MonoBehaviour
{
    private string id;
    public float baseVelocity = 0.01f;
    private float velocity;
    private bool moving = false;
    public float rotationSpeed = 1.0f;
    public ExperimentRunner experimentRunner;
    public bool debug_static = false;
    public Transform trController;
    public float turnAngleMin = 20.0f;
    public float turnAngleRange = 40.0f;


    void Start()
    {
        this.velocity = this.baseVelocity;
    }

    // Update is called once per frame
    void Update()
    {   
        

        if(Keyboard.current.leftArrowKey.isPressed) {
            this.turn(-1);
        } else if (Keyboard.current.rightArrowKey.isPressed) {
            this.turn(1);
        }
        float controllerYaw = this.trController.localEulerAngles.y;
        if (controllerYaw > this.turnAngleMin && 
            controllerYaw < this.turnAngleMin + this.turnAngleRange) {
            this.turn(1);
        } else if (controllerYaw < 360.0f-this.turnAngleMin &&
            controllerYaw > 360.0f-this.turnAngleMin-this.turnAngleRange) {
            this.turn(-1);
        }

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

    public Transform getTransform() {
        return gameObject.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        this.handleCollision(other.gameObject);
    }

    public void handleCollision(GameObject collideGameObject) {
        if (collideGameObject.name.Contains("Tile")) {
            // Parent is TileX which contains obstalce tag
            collideGameObject = collideGameObject.transform.parent.gameObject;
        }
        Debug.Log("Collided with " + collideGameObject.name);
        if (collideGameObject.CompareTag("obstacle")) {
            Debug.Log("Hit obstacle");
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
