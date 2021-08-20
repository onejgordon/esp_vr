using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public class Fixation {

    public string objectName;
    public double start_ts;
    public double stop_ts;
    public double duration;

    public Fixation(string objectName, double start_ts, double stop_ts) {
        this.objectName = objectName;
        this.duration = stop_ts - start_ts;
        this.start_ts = start_ts;
        this.stop_ts = stop_ts;
    }
}


[System.Serializable]
public class Record {

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

    public Record(Quaternion hmd_rot, Quaternion ctr_rot, Vector3 gaze_origin, Vector3 gaze_direction, float convDistance, bool blinking) {
        this.ts = Util.timestamp();
        float hmd_x = hmd_rot.x;
        float hmd_y = hmd_rot.y;
        float hmd_z = hmd_rot.z;
        float hmd_w = hmd_rot.w;
        this.hmd_roll  = Mathf.Atan2(2*hmd_y*hmd_w + 2*hmd_x*hmd_z, 1 - 2*hmd_y*hmd_y - 2*hmd_z*hmd_z);
        this.hmd_pitch = Mathf.Atan2(2*hmd_x*hmd_w + 2*hmd_y*hmd_z, 1 - 2*hmd_x*hmd_x - 2*hmd_z*hmd_z);
        this.hmd_yaw   =  Mathf.Asin(2*hmd_x*hmd_y + 2*hmd_z*hmd_w);
        this.hmd_x = hmd_x;
        this.hmd_y = hmd_y;
        this.hmd_z = hmd_z;
        float ctr_x = ctr_rot.x;
        float ctr_y = ctr_rot.y;
        float ctr_z = ctr_rot.z;
        float ctr_w = ctr_rot.w;
        this.ctr_roll  = Mathf.Atan2(2*ctr_y*ctr_w + 2*ctr_x*ctr_z, 1 - 2*ctr_y*ctr_y - 2*ctr_z*ctr_z);
        this.ctr_pitch = Mathf.Atan2(2*ctr_x*ctr_w + 2*ctr_y*ctr_z, 1 - 2*ctr_x*ctr_x - 2*ctr_z*ctr_z);
        this.ctr_yaw   =  Mathf.Asin(2*ctr_x*ctr_y + 2*ctr_z*ctr_w);
        this.ctr_x = ctr_x;
        this.ctr_y = ctr_y;
        this.ctr_z = ctr_z;
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

[System.Serializable]
public class SessionTrial
{
    private string session_id; // Unique id for each subject/session
    public int trial_id;
    public int reward = 0;
    public double ts_start;
    public double ts_end;

    public bool practice = false;

    public List<Fixation> fixations;
    public List<Record> records;

    public MapSpec map;

    public SessionTrial(string session_id, int id, MapSpec map, bool practice) {
        this.session_id = session_id;
        this.map = map;
        this.trial_id = id;
        this.ts_start = Util.timestamp();
        this.reward = 0;
        this.practice = practice;
        this.fixations = new List<Fixation>();
        this.records = new List<Record>();
    }

    public void Finished() {
        this.ts_end = Util.timestamp();
    }

    public void SaveToFile() {
        string json = JsonUtility.ToJson(this);
        string path = this.outfile();
        StreamWriter sw = File.CreateText(path);
        sw.Close();
        File.WriteAllText(path, json);
    }

    public void CleanUpData() {
        // Only to be run after we've saved
        this.records.Clear();
        this.fixations.Clear();
    }
    public string outfile() {
        return SessionSaver.OUTDIR + "session_" + this.session_id + "_trial_" + this.trial_id.ToString() + ".json";
    }

    public bool scored() {
        return !this.practice;
    }

    public bool addRecord(Record record) {
        this.records.Add(record);
        return true;
    }

    public bool addFixation(string objectName, double start, double stop) {
        this.fixations.Add(new Fixation(objectName, start, stop));
        return true;
    }
}
