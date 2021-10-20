using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// using Valve.VR.Extras;
using System.IO;
// using Tobii.XR;

public class ExperimentRunner : MonoBehaviour
{
    // Configs / params
    private Color SKY_DEFAULT = new Color(.8f, .8f, .8f);
    private Color DBLUE = new Color(.1f, .1f, 1f);
    private Color DGREEN = new Color(.1f, 1f, .1f);
    private Material goalMat = null;
    public int planningSeconds = 10;
    public int navigationSeconds = 30;

    public SessionSaver session;
    public bool QUICK_DEBUG = true;
    public int N_TRIALS = 10; // Set to 0 for production. Just for short debug data collection
    public int practice_rounds = 0;
    public bool left_handed = false;
    public bool record = false;

    // State
    private double ts_exp_start = 0; // Timestamp
    private int trial_index = 0;
    private int practice_remaining = 0;
    private SessionTrial current_trial;
    private bool recording = false;
    private bool practicing = false;
    private string mode = "planning";

    // Session specs
    private string session_id;
    private List<int> map_order = new List<int>();
    private List<MapDef> maps = new List<MapDef>();

    // Main experiment objects

    private Transform trCameraRig;
    public Transform trAgent;
    private Transform controller;
    public Transform trSceneLight;


    // Main experiment behaviors
    private MapBehavior mapBehavior;

    // Other scene objects
    public UIBehavior ui;
    // public SteamVR_LaserPointer laserPointer;

    void Start()
    {
        this.goalMat =  Resources.Load("GoalMat", typeof(Material)) as Material;
        if (QUICK_DEBUG) {
            practice_rounds = 0;
            N_TRIALS = 5;
            this.session_id = "DEBUG";
        } else {
            this.session_id = ((int)Util.timestamp()).ToString();
        }
        this.session.data.session_id = this.session_id;
        this.session.data.left_handed = this.left_handed;
        this.practice_remaining = this.practice_rounds;
        this.ui = GameObject.Find("UICanvas").GetComponent<UIBehavior>();
        this.trCameraRig = GameObject.Find("[CameraRig]").GetComponent<Transform>();
        // this.controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<Transform>();

        this.mapBehavior = GameObject.Find("Map").GetComponent<MapBehavior>();
        this.maps = new List<MapDef>();
        // TobiiXR_Settings tobii_settings = new TobiiXR_Settings();
        // tobii_settings.FieldOfUse = FieldOfUse.Analytical;
        // TobiiXR.Start(tobii_settings);
        this.BeginExperiment();
    }

    // Update is called once per frame
    void Update()
    {
        if (recording && this.current_trial != null) {
            Quaternion hmdRot = trCameraRig.rotation;
            // Quaternion ctrlRot = controller.rotation;
            Vector3 gazeOrigin = new Vector3();
            Vector3 gazeDirection = new Vector3();
            float convDistance = -1.0f; // Default when not valid
            bool eitherEyeClosed = false;
            // var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
            // if (eyeTrackingData.GazeRay.IsValid) {
            //     gazeOrigin = eyeTrackingData.GazeRay.Origin;
            //     gazeDirection = eyeTrackingData.GazeRay.Direction;
            // }
            // eitherEyeClosed = eyeTrackingData.IsLeftEyeBlinking || eyeTrackingData.IsRightEyeBlinking;
            // if (eyeTrackingData.ConvergenceDistanceIsValid) {
            //     convDistance = eyeTrackingData.ConvergenceDistance;
            // }
            // Record record = new Record(hmdRot, ctrlRot, gazeOrigin, gazeDirection, convDistance, eitherEyeClosed);
            // this.current_trial.addRecord(record);
        }
    }

    public void Calibrate() {

    }

    private double minutes_in() {
        return (Util.timestamp() - this.ts_exp_start) / 60;
    }

    public bool isNavigating() {
        return this.mode == "navigation";
    }

    void RandomizeTrialOrder() {
        // TODO
        this.map_order = Enumerable.Range(1, N_TRIALS).ToList();
        Debug.Log(map_order.ToString());
    }

    public void BeginExperiment() {
        if (record) {
            this.recording = true;
        }
        this.RandomizeTrialOrder();
        this.ts_exp_start = Util.timestamp();
        this.session.data.ts_session_start = this.ts_exp_start;
        GotoNextTrial();
    }

    public void GotoNextTrial() {
        this.trial_index += 1;
        double mins = this.minutes_in();
        if ((N_TRIALS != 0 && this.trial_index > N_TRIALS) || this.trial_index > this.N_TRIALS + this.practice_rounds) Finish();
        else {
            RunOneTrial();
        }
    }

    void RunOneTrial() {
        Debug.Log("Running trial " + this.trial_index.ToString());
        this.ui.HideHUDScreen();
        bool first_real = false;
        int map_index = this.map_order[this.trial_index - this.practice_rounds - 1];
        this.mapBehavior.load(map_index);
        if (this.practicing) {
            if (this.practice_remaining == 0) {
                this.practicing = false;
                first_real = true;
            } else {
                this.practice_remaining -= 1;
            }
        }
        this.current_trial = new SessionTrial(this.session_id, this.trial_index, this.mapBehavior.map, this.practicing);
        if (this.practicing) {
            ui.ShowHUDScreenWithConfirm(
                string.Format(
                    "This is practice round {0} of {1}. Your performance on these rounds won't affect your score. Click your controller trigger to proceed.",
                    this.practice_rounds - this.practice_remaining,
                    this.practice_rounds
                ),
                Color.black, "BeginTrial");
        } else {
            if (first_real) {
                // Show message indicating we're starting real trials
                ui.ShowHUDScreenWithConfirm("Great job. Practice rounds finished. All remaining trials are real and will be scored. Click your controller trigger to proceed.",
                DBLUE, "BeginTrial");
            } else {
                BeginTrial();
            }
        }
    }


    void BeginTrial() {
        this.StartPlanningPhase();
    }


    void StartPlanningPhase() {
        Debug.Log("Start planning...");
        this.mode = "planning";
        this.trAgent.gameObject.SetActive(true);
        this.mapBehavior.setupCameraForPlanning(this.trCameraRig);
        this.mapBehavior.setupAgentForPlanning(this.trAgent);
        // Set timeout to start navigation
        if (this.planningSeconds > 0) {
            StartCoroutine(WaitThenNavigate(this.planningSeconds));
            ui.ShowHUDCountdownMessage(this.planningSeconds, "Navigation will start in... ");
        }
    }

    IEnumerator WaitThenNavigate(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        StartNavigationPhase();
    }

    void StartNavigationPhase() {
        Debug.Log("Start navigation...");
        this.mode = "navigation";        
        this.mapBehavior.setupAgentForNavigation(this.trAgent);
        this.mapBehavior.showExistingRewards(this.getCurrentTrial().rewards_present);
        if (this.navigationSeconds > 0) {
            StartCoroutine(EndTrialAfterNavigation(this.navigationSeconds, this.current_trial.trial_id));
            ui.ShowHUDCountdownMessage(this.navigationSeconds, "Collect rewards");
        }
    }

    IEnumerator EndTrialAfterNavigation(float waitTime, int currentTrialId) {
        yield return new WaitForSeconds(waitTime);
        if (this.current_trial != null && this.current_trial.trial_id == currentTrialId) {
            Debug.Log("Trial timeout...");
            EndTrial();
        }
    }

    public void EndTrial() {
        this.trAgent.gameObject.SetActive(false); // Hide agent
        ui.ClearCountdown();
        ui.ShowHUDScreen("Trial finished", Color.blue);

        this.current_trial.Finished();
        this.current_trial.SaveToFile();
        this.current_trial.CleanUpData(); // Deletes large data once saved
        session.AddTrial(this.current_trial);
        this.current_trial = null;
        if (this.navigationSeconds != -1) {
            // Time limit, clear timer to avoid double GoTo
            CancelInvoke();
        }
        Invoke("GotoNextTrial", 3);
    }


    public SessionTrial getCurrentTrial() {
        return this.current_trial;
    }

    public void SaveToPath(string node_id) {
        // TODO
    }

    void Finish() {
        Debug.Log("Done, saving...");
        session.data.ts_session_end = Util.timestamp();
        session.SaveToFile();
        if (this.recording) this.recording = false;
        this.mapBehavior.maybeClearMap();
        string results = "All trials finished!\n\n";
        double percent = this.session.data.total_points / (double) this.session.data.total_points_possible;
        int base_compensation = 20;
        int bonus = 0;
        if (percent >= .55 && percent < .70) bonus = 3;
        else if (percent >= .7 && percent < .85) bonus = 4;
        else if (percent >= .85) bonus = 5;
        int total = base_compensation + bonus;
        results += string.Format("Your final score is {0} points of of {1} points possible.\nYour final success rate is {2:0.0}% (${3} bonus, ${4} total).\n\nYour experimenter will help you take off the VR headset.",
            this.session.data.total_points,
            this.session.data.total_points_possible,
            100.0 * percent,
            bonus,
            total
            );
        ui.ShowHUDScreen(results, DGREEN);
        Debug.Log(">>>>>> Subject Bonus: $" + bonus.ToString() + " TOTAL: $" + total.ToString());
        // TobiiXR.Stop();
    }

    public bool navigationMode() {
        return this.mode == "navigation";
    }
}
