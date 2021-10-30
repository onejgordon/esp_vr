using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollowBehavior : MonoBehaviour
{
    public AgentBehavior behAgent;
    public ExperimentRunner experimentRunner;

    public float distance = 3.0f;
    public float height = 1.0f;
    public float damping = 5.0f;
    public bool pitchAtHorizon = true;
    public bool smoothRotation = true;
    public bool followBehind = true;
    public float rotationDamping = 10.0f;

    void Start() {

    }
    void Update () {
        Transform target = behAgent.getTransform();
        if (experimentRunner.isNavigating()) {
            transform.position = Vector3.Lerp (transform.position, this.getWantedPosition(), Time.deltaTime * damping);
            if (smoothRotation) {
                    transform.rotation = Quaternion.Slerp (transform.rotation, this.getWantedRotation(), Time.deltaTime * rotationDamping);
            }
            else transform.LookAt (target, target.up);            
        }
    }

    public Vector3 getWantedPosition() {
        Transform target = behAgent.getTransform();
        Vector3 wantedPosition;
        if(followBehind)
            wantedPosition = target.TransformPoint(0, height, -distance);
        else
            wantedPosition = target.TransformPoint(0, height, distance);
        return wantedPosition;
    }

    public Quaternion getWantedRotation() {
        Transform target = behAgent.getTransform();
        Vector3 targetDelta = target.position - transform.position;
        if (pitchAtHorizon) targetDelta.y = 0;
        Quaternion wantedRotation = Quaternion.LookRotation(targetDelta, target.up);
        return wantedRotation;
    }

    public void jumpTo() {
        transform.position = this.getWantedPosition();
        transform.rotation = this.getWantedRotation();
    }

}
