using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AgentBehavior : MonoBehaviour
{
    private string id;
    public float velocity = 0.01f;
    public float rotationSpeed = 1.0f;


    void Start()
    {

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
