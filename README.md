# Embodied Spatial Prospection | VR Experiment

## Dependencies

Tested with:

- OpenVR XR Plugin (1.1.4)
- Tobii XR SDK (3.0.1)
- Unity editor (2021.1.24f1)
- Steam VR 1.19.7
- Unity Hub 2.2.2
- SRAnipal SDK 1.3.2

## Issues

1. NoseDirectionProvider used and not Tobii/Vive

* SRanipal SDK running?

2. Tobii disconnecting, losing eye tracking (defaults back to nose direction)

ERROR [prp-client] "PRP_ERROR_ENUM_CONNECTION_FAILED (00000003)" {FileName:"prp_client.cpp(879)",Function:"prp_client_process_subscriptions::<lambda_f7cd8d5b0fb2cd7e6273b0700a8ee96e>::operator ()",Tags:["PRP"]}
UnityEngine.Debug:Log (object)
Tobii.XR.StreamEngineTracker:LogCallback (intptr,Tobii.StreamEngine.tobii_log_level_t,string) (at C:/Users/VR_Research_1/Documents/TobiiXRSDK_3.0.1.179/Runtime/Core/Providers/Tobii/StreamEngineTracker.cs:251)
Tobii.XR.StreamEngineTracker:ProcessCallback (intptr,System.Diagnostics.Stopwatch) (at C:/Users/VR_Research_1/Documents/TobiiXRSDK_3.0.1.179/Runtime/Core/Providers/Tobii/StreamEngineTracker.cs:195)
Tobii.XR.StreamEngineTracker:ProcessLoop () (at C:/Users/VR_Research_1/Documents/TobiiXRSDK_3.0.1.179/Runtime/Core/Providers/Tobii/StreamEngineTracker.cs:158)
System.Threading.ThreadHelper:ThreadStart ()

internal_device.cpp(2517): error "DEVICE_ERROR_CONNECTION_FAILED" (00000004) in function "process_callbacks"
UnityEngine.Debug:Log (object)
Tobii.XR.StreamEngineTracker:LogCallback (intptr,Tobii.StreamEngine.tobii_log_level_t,string) (at C:/Users/VR_Research_1/Documents/TobiiXRSDK_3.0.1.179/Runtime/Core/Providers/Tobii/StreamEngineTracker.cs:251)
Tobii.XR.StreamEngineTracker:ProcessCallback (intptr,System.Diagnostics.Stopwatch) (at C:/Users/VR_Research_1/Documents/TobiiXRSDK_3.0.1.179/Runtime/Core/Providers/Tobii/StreamEngineTracker.cs:195)
Tobii.XR.StreamEngineTracker:ProcessLoop () (at C:/Users/VR_Research_1/Documents/TobiiXRSDK_3.0.1.179/Runtime/Core/Providers/Tobii/StreamEngineTracker.cs:158)
System.Threading.ThreadHelper:ThreadStart ()

tobii.cpp(286): error "TOBII_ERROR_CONNECTION_FAILED" (00000005) in function "tobii_device_process_callbacks"
UnityEngine.Debug:Log (object)
Tobii.XR.StreamEngineTracker:LogCallback (intptr,Tobii.StreamEngine.tobii_log_level_t,string) (at C:/Users/VR_Research_1/Documents/TobiiXRSDK_3.0.1.179/Runtime/Core/Providers/Tobii/StreamEngineTracker.cs:251)
Tobii.XR.StreamEngineTracker:ProcessCallback (intptr,System.Diagnostics.Stopwatch) (at C:/Users/VR_Research_1/Documents/TobiiXRSDK_3.0.1.179/Runtime/Core/Providers/Tobii/StreamEngineTracker.cs:195)
Tobii.XR.StreamEngineTracker:ProcessLoop () (at C:/Users/VR_Research_1/Documents/TobiiXRSDK_3.0.1.179/Runtime/Core/Providers/Tobii/StreamEngineTracker.cs:158)
System.Threading.ThreadHelper:ThreadStart ()

Failed to process callback. Error TOBII_ERROR_CONNECTION_FAILED
UnityEngine.Debug:LogError (object)
Tobii.XR.StreamEngineTracker:ProcessCallback (intptr,System.Diagnostics.Stopwatch) (at C:/Users/VR_Research_1/Documents/TobiiXRSDK_3.0.1.179/Runtime/Core/Providers/Tobii/StreamEngineTracker.cs:201)
Tobii.XR.StreamEngineTracker:ProcessLoop () (at C:/Users/VR_Research_1/Documents/TobiiXRSDK_3.0.1.179/Runtime/Core/Providers/Tobii/StreamEngineTracker.cs:158)
System.Threading.ThreadHelper:ThreadStart ()

