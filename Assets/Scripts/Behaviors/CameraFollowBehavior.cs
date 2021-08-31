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
        if (experimentRunner.navigationMode()) {
            Vector3 wantedPosition;
            if(followBehind)
                    wantedPosition = target.TransformPoint(0, height, -distance);
            else
                    wantedPosition = target.TransformPoint(0, height, distance);

            transform.position = Vector3.Lerp (transform.position, wantedPosition, Time.deltaTime * damping);

            if (smoothRotation) {
                    Vector3 targetDelta = target.position - transform.position;
                    if (pitchAtHorizon) targetDelta.y = 0;
                    Quaternion wantedRotation = Quaternion.LookRotation(targetDelta, target.up);
                    transform.rotation = Quaternion.Slerp (transform.rotation, wantedRotation, Time.deltaTime * rotationDamping);
            }
            else transform.LookAt (target, target.up);            
        }
    }

}
