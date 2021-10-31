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
    public double duration;

    public Fixation(string modeChar, string objectName, double start_ts, double stop_ts) {
        this.mode = modeChar;
        this.objectName = objectName;
        this.duration = stop_ts - start_ts;
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
    public float ag_yaw;
    public float ag_roll;
    public float ag_pitch;
    public float hmd_yaw;
    public float hmd_roll;
    public float hmd_pitch;
    public float hmd_x;
    public float hmd_y;
    public float hmd_z;
    public float ctr_yaw;
    public float ctr_roll;
    public float ctr_pitch;
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


    public Record(string mode, Transform trAgent, Transform trHMD, Transform trController, Vector3 gaze_origin, Vector3 gaze_direction, float convDistance, bool blinking) {
        this.ts = Util.timestamp();
        this.mode = mode;
        this.ag_x = trAgent.position.x;
        this.ag_y = trAgent.position.y;
        this.ag_z = trAgent.position.z;
        Quaternion ag_rot = trAgent.rotation;
        this.ag_roll  = Mathf.Atan2(2*ag_rot.y*ag_rot.w + 2*ag_rot.x*ag_rot.z, 1 - 2*ag_rot.y*ag_rot.y - 2*ag_rot.z*ag_rot.z);
        this.ag_pitch = Mathf.Atan2(2*ag_rot.x*ag_rot.w + 2*ag_rot.y*ag_rot.z, 1 - 2*ag_rot.x*ag_rot.x - 2*ag_rot.z*ag_rot.z);
        this.ag_yaw   =  Mathf.Asin(2*ag_rot.x*ag_rot.y + 2*ag_rot.z*ag_rot.w);
        Quaternion hmd_rot = trHMD.rotation;
        Quaternion ctr_rot = trController.rotation;
        Vector3 hmd_pos = trHMD.position;
        Vector3 ctr_pos = trController.position;
        float hmd_rot_x = hmd_rot.x;
        float hmd_rot_y = hmd_rot.y;
        float hmd_rot_z = hmd_rot.z;
        float hmd_rot_w = hmd_rot.w;
        this.hmd_roll  = Mathf.Atan2(2*hmd_rot_y*hmd_rot_w + 2*hmd_rot_x*hmd_rot_z, 1 - 2*hmd_rot_y*hmd_rot_y - 2*hmd_rot_z*hmd_rot_z);
        this.hmd_pitch = Mathf.Atan2(2*hmd_rot_x*hmd_rot_w + 2*hmd_rot_y*hmd_rot_z, 1 - 2*hmd_rot_x*hmd_rot_x - 2*hmd_rot_z*hmd_rot_z);
        this.hmd_yaw   =  Mathf.Asin(2*hmd_rot_x*hmd_rot_y + 2*hmd_rot_z*hmd_rot_w);
        this.hmd_x = hmd_pos.x;
        this.hmd_y = hmd_pos.y;
        this.hmd_z = hmd_pos.z;
        float ctr_rot_x = ctr_rot.x;
        float ctr_rot_y = ctr_rot.y;
        float ctr_rot_z = ctr_rot.z;
        float ctr_rot_w = ctr_rot.w;
        this.ctr_roll  = Mathf.Atan2(2*ctr_rot_y*ctr_rot_w + 2*ctr_rot_x*ctr_rot_z, 1 - 2*ctr_rot_y*ctr_rot_y - 2*ctr_rot_z*ctr_rot_z);
        this.ctr_pitch = Mathf.Atan2(2*ctr_rot_x*ctr_rot_w + 2*ctr_rot_y*ctr_rot_z, 1 - 2*ctr_rot_x*ctr_rot_x - 2*ctr_rot_z*ctr_rot_z);
        this.ctr_yaw   =  Mathf.Asin(2*ctr_rot_x*ctr_rot_y + 2*ctr_rot_z*ctr_rot_w);
        this.ctr_x = ctr_pos.x;
        this.ctr_y = ctr_pos.y;
        this.ctr_z = ctr_pos.z;
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
        if (convDistance > -1.0 && gaze_origin != null && gaze_direction != null) {
            Vector3 tgt = gaze_origin + gaze_direction * convDistance;
            this.gaze_tgt_x = tgt.x;
            this.gaze_tgt_y = tgt.y;
            this.gaze_tgt_z = tgt.z;
        }
        this.blinking = blinking;
    }
}
