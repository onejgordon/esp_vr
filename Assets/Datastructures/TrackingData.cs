using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public class Fixation {
    public string mode;
    public string objectName;
    public double start_ts;
    public double stop_ts;

    public Fixation(string modeChar, string objectName, double start_ts, double stop_ts) {
        this.mode = modeChar;
        this.objectName = objectName;
        this.start_ts = start_ts;
        this.stop_ts = stop_ts;
    }
}


[System.Serializable]
public class Record {
    public string mode;
    public float ag_x;
    public float ag_y;
    public float ag_z;
    public float ag_rot_x;
    public float ag_rot_y;
    public float ag_rot_z;
    public float hmd_rot_x;
    public float hmd_rot_y;
    public float hmd_rot_z;
    public float hmd_x;
    public float hmd_y;
    public float hmd_z;
    public float ctr_rot_x;
    public float ctr_rot_y;
    public float ctr_rot_z;
    public float ctr_x;
    public float ctr_y;
    public float ctr_z;
    public float gaze_or_x;
    public float gaze_or_y;
    public float gaze_or_z;
    public float gaze_dir_x;
    public float gaze_dir_y;
    public float gaze_dir_z;
    public float gaze_tgt_x; // Inferred from distance
    public float gaze_tgt_y; // Inferred from distance
    public float gaze_tgt_z; // Inferred from distance

    public float gaze_conv_dist; // -1 = N/A
    public bool blinking;
    public double ts;


    public Record(string mode, Transform trAgent, Transform trHMD, Transform trController, Vector3 gaze_origin, 
            Vector3 gaze_direction, Vector3 gaze_target, float convDistance, bool blinking) {
        this.ts = Util.timestamp();
        Vector3 hmd_pos = trHMD.position;
        Vector3 ctr_pos = trController.position;
        Vector3 ag_pos = trAgent.position;

        this.mode = mode;
        this.ag_x = ag_pos.x;
        this.ag_y = ag_pos.y;
        this.ag_z = ag_pos.z;
        this.hmd_x = hmd_pos.x;
        this.hmd_y = hmd_pos.y;
        this.hmd_z = hmd_pos.z;
        this.ctr_x = ctr_pos.x;
        this.ctr_y = ctr_pos.y;
        this.ctr_z = ctr_pos.z;

        this.ag_rot_x = trAgent.localEulerAngles.x;
        this.ag_rot_y = trAgent.localEulerAngles.y;
        this.ag_rot_z = trAgent.localEulerAngles.z;
        this.hmd_rot_x = trHMD.localEulerAngles.x;
        this.hmd_rot_y = trHMD.localEulerAngles.y;
        this.hmd_rot_z = trHMD.localEulerAngles.z;
        this.ctr_rot_x = trController.localEulerAngles.x;
        this.ctr_rot_y = trController.localEulerAngles.y;
        this.ctr_rot_z = trController.localEulerAngles.z;

        if (gaze_origin != null) {
            this.gaze_or_x = gaze_origin.x;
            this.gaze_or_y = gaze_origin.y;
            this.gaze_or_z = gaze_origin.z;
        }
        if (gaze_direction != null) {
            this.gaze_dir_x = gaze_direction.x;
            this.gaze_dir_y = gaze_direction.y;
            this.gaze_dir_z = gaze_direction.z;
        }
        this.gaze_conv_dist = convDistance;
        if (gaze_target != null) {
            this.gaze_tgt_x = gaze_target.x;
            this.gaze_tgt_y = gaze_target.y;
            this.gaze_tgt_z = gaze_target.z;
        }
        this.blinking = blinking;
    }
}
