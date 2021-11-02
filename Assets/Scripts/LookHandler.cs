using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.G2OM;
using Tobii.XR;

public class LookHandler : MonoBehaviour, IGazeFocusable 
{
    private ExperimentRunner exp;
    bool gazedAt = false;
    double last_gaze_start_ts = 0.0f;
    private string name;

    void Start() {
        this.exp = GameObject.Find("World").GetComponent<ExperimentRunner>();
        this.name = this.objectName();
    }
    void Update()
    {

    }

    private string objectName() {
        string oname = gameObject.name;
        GameObject parent = gameObject.transform.parent.gameObject;
        if (parent.CompareTag("tile")) {
            oname = parent.name;
        } else if (gameObject.CompareTag("reward")) {
            oname = parent.name;
        }
        return oname;
    }

    public void GazeFocusChanged(bool focused) {
        this.gazedAt = focused;
        if (focused) {
            // Start gaze timer
            this.last_gaze_start_ts = Util.timestamp();
            // Debug.Log("Starting fixation on " + this.name);
        } else {
            // Stop gaze timer and record fixation
            if (this.last_gaze_start_ts > 0.0f) {
                Debug.Log(">> Adding fixation on " + this.name);
                SessionTrial trial = exp.getCurrentTrial();
                if (trial != null) trial.addFixation(this.exp.modeChar(), this.name, this.last_gaze_start_ts, Util.timestamp());
                this.last_gaze_start_ts = 0.0f;
            }
        }
    }

}

