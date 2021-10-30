using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// using Valve.VR.Extras;
using System.IO;
using Tobii.XR;

public class ExperimentRunner : MonoBehaviour
{
    // Configs / params
    private Color SKY_DEFAULT = new Color(.8f, .8f, .8f);
    private Color DBLUE = new Color(.1f, .1f, 1f);
    private Color DGREEN = new Color(.1f, .7f, .1f);
    public int N_CHIMES = 3;
    private Material goalMat = null;
    public int planningSeconds = 10;
    public int transitionSeconds = 5;
    public int navigationSeconds = 30;
    public int endTrialPauseSecs = 3;

    public SessionSaver session;
    public bool QUICK_DEBUG = true;
    public int N_TRIALS = 10; // Set to 0 for production. Just for short debug data collection
    public int N_MAPS = 20;
    public int practice_rounds = 2;
    public bool left_handed = false;
    public bool record = false;

    // State
    private double ts_exp_start = 0; // Timestamp
    private int trial_index = 0;
    private int practice_remaining = 0;
    private SessionTrial current_trial;
    private bool recording = false;
    private bool practicing = false;
    private int chimesPlayed = 0;
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
    public Camera sceneCamera;


    // Main experiment behaviors
    private MapBehavior mapBehavior;

    // Other scene objects
    public UIBehavior ui;
    public AudioSource chimeAudio;

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
        this.sceneCamera = GameObject.Find("Camera").GetComponent<Camera>();
        this.ui = GameObject.Find("UICanvas").GetComponent<UIBehavior>();
        this.trCameraRig = GameObject.Find("[CameraRig]").GetComponent<Transform>();
        this.controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<Transform>();
        this.chimeAudio = GetComponent<AudioSource>();
        this.mapBehavior = GameObject.Find("Map").GetComponent<MapBehavior>();
        this.maps = new List<MapDef>();
        // TobiiXR_Settings tobii_settings = new TobiiXR_Settings();
        // tobii_settings.FieldOfUse = FieldOfUse.Analytical;
        // TobiiXR.Start(tobii_settings); Performed by TobiiXR_Initializer?
        this.BeginExperiment();
    }

    // Update is called once per frame
    void Update()
    {
        if (recording && this.current_trial != null) {
            Quaternion hmdRot = trCameraRig.rotation;
            Quaternion ctrlRot = controller.rotation;
            Vector3 gazeOrigin = new Vector3();
            Vector3 gazeDirection = new Vector3();
            float convDistance = -1.0f; // Default when not valid
            bool eitherEyeClosed = false;
            var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
            if (eyeTrackingData.GazeRay.IsValid) {
                gazeOrigin = eyeTrackingData.GazeRay.Origin;
                gazeDirection = eyeTrackingData.GazeRay.Direction;
            }
            eitherEyeClosed = eyeTrackingData.IsLeftEyeBlinking || eyeTrackingData.IsRightEyeBlinking;
            if (eyeTrackingData.ConvergenceDistanceIsValid) {
                convDistance = eyeTrackingData.ConvergenceDistance;
            }
            Record record = new Record(
                this.mode[0],  // First letter of mode in [p, t, n]
                this.trAgent,
                hmdRot,
                ctrlRot,
                gazeOrigin,
                gazeDirection,
                convDistance,
                eitherEyeClosed);
            this.current_trial.addRecord(record);
        }

        this.maybePlayChimes();
    }

    public void maybePlayChimes() {
        // TODO: Chimes for end of planning and transition
        SessionTrial trial = this.getCurrentTrial();
        if (this.navigationMode() && trial != null) {
            int seconds_from_end = (int)(this.navigationSeconds - (Util.timestamp() - trial.ts_navigation_start));
            int chimes_needed = this.N_CHIMES - seconds_from_end;
            if (chimes_needed > chimesPlayed) {
                // Play chime
                this.chimeAudio.Play();
                this.chimesPlayed += 1;
            }
        }
    }

    public bool isNavigating() {
        return this.mode == "navigation";
    }

    void RandomizeTrialOrder() {
        this.map_order = Util.Shuffle<int>(Enumerable.Range(1, N_MAPS).ToList());
        // Practice maps start at 100
        for (int i=0; i<this.practice_rounds; i++) {
            this.map_order.Insert(0, 100 + i)
        }
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
        this.getCurrentTrial().ts_planning_start = Util.timestamp();

        // Set timeout to start navigation
        StartCoroutine(WaitThenTransition(this.planningSeconds));
    }

    IEnumerator WaitThenTransition(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        StartTransitionPhase();
    }

    void StartTransitionPhase() {
        Debug.Log("Start transition...");
        this.mode = "transition";
        // TODO: Make everything invisible, but preserve fixation capture
        StartCoroutine(WaitThenNavigate(this.transitionSeconds));
    }

    IEnumerator WaitThenNavigate(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        StartNavigationPhase();
    }

    void StartNavigationPhase() {
        Debug.Log("Start navigation...");
        this.mode = "navigation";        
        this.mapBehavior.setupAgentForNavigation(this.trAgent);
        this.mapBehavior.setupCameraForNavigation();
        this.mapBehavior.showExistingRewards(this.getCurrentTrial().rewards_present);
        this.getCurrentTrial().ts_navigation_start = Util.timestamp();
        this.chimesPlayed = 0;
        if (this.navigationSeconds > 0) {
            StartCoroutine(EndTrialAfterNavigation(this.navigationSeconds, this.current_trial.trial_id));
            //ui.ShowHUDCountdownMessage(this.navigationSeconds, "Collect gems");
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
        this.mode = "ended";
        this.trAgent.gameObject.SetActive(false); // Hide agent
        ui.ClearCountdown();
        ui.ShowHUDScreen("Trial finished, starting next trial shortly...", Color.blue);

        this.current_trial.Finished();
        this.current_trial.SaveToFile();
        this.current_trial.CleanUpData(); // Deletes large data once saved
        session.AddTrial(this.current_trial);
        this.current_trial = null;
        if (this.navigationSeconds != -1) {
            // Time limit, clear timer to avoid double GoTo
            CancelInvoke();
        }
        Invoke("GotoNextTrial", this.endTrialPauseSecs);
    }


    public SessionTrial getCurrentTrial() {
        return this.current_trial;
    }

    void Finish() {
        Debug.Log("Done, saving...");
        session.data.ts_session_end = Util.timestamp();
        session.SaveToFile();
        if (this.recording) this.recording = false;
        this.mapBehavior.maybeClearMap();
        string results = "All trials finished!\n\n";
        int total_points_possible = this.session.data.getPointsPossible();
        double percent = this.session.data.total_points / (double) total_points_possible;
        results += string.Format("Final score is {0} points of of {1} points possible.\nYour final success rate is {2:0.0}%.\n\nYour experimenter will help you take off the VR headset.",
            this.session.data.total_points,
            total_points_possible,
            100.0 * percent
        );
        ui.ShowHUDScreen(results, DGREEN);
        TobiiXR.Stop();
    }

    public bool navigationMode() {
        return this.mode == "navigation";
    }
}
