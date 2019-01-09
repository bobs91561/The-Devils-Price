#if UNITY_2017_1_OR_NEWER
using UnityEngine;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Sequencer commannd CinemachinePriority(vcam, [priority])
    /// 
    /// Sets the priority of a Cinemachine virtual camera.
    /// 
    /// - vcam: The name of a GameObject containing a CinemachineVirtualCamera.
    /// - priority: (Optional) New priority level. Default: 999.
    /// </summary>
    public class SequencerCommandCinemachinePriority : SequencerCommand
    {

        public void Start()
        {
            var subject = GetSubject(0);
            var vcam = (subject != null) ? subject.GetComponent<Cinemachine.CinemachineVirtualCamera>() : null;
            var priority = GetParameterAsInt(1, 999);
            if (vcam == null)
            {
                if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: Sequencer: CinemachinePriority(" + GetParameters() +
                    "): Can't find virtual camera '" + GetParameter(0) + ".");
            }
            else
            {
                if (DialogueDebug.LogInfo) Debug.Log("Dialogue System: Sequencer: CinemachinePriority(" + vcam + ", " + priority + ")");
                vcam.Priority = priority;
            }
            Stop();
        }

    }

}
#endif
