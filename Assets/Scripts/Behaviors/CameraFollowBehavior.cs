using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollowBehavior : MonoBehaviour
{
    public AgentBehavior behAgent;
    public ExperimentRunner experimentRunner;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {   
        if (experimentRunner.navigationMode()) {
            gameObject.transform.position = behAgent.overheadCameraPosition();
            gameObject.transform.eulerAngles = new Vector3(0, behAgent.getHeading(), 0);
        }
    }


}
