using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;
using UnityEngine.XR.Management;

namespace VRTraining.Core
{
    // spawns xr simulator only when no headset running
    public class SimulatorBootstrap : MonoBehaviour
    {
        [SerializeField] private GameObject simulatorPrefab;

        private void Start()
        {
            if (simulatorPrefab == null) return;

            // openxr loader running when headset and runtime were found at startup
            // not using isdeviceactive because still false in first frames
            bool headsetRunning = false;
            if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
            {
                headsetRunning = XRGeneralSettings.Instance.Manager.activeLoader != null;
            }

            if (headsetRunning) return;

            GameObject simulator = Instantiate(simulatorPrefab);

            // simulator point and click turns off mouse input of the ui module
            // switched off so plain mouse clicks work on menu buttons
            XRInteractionSimulator interactionSimulator = simulator.GetComponentInChildren<XRInteractionSimulator>();
            if (interactionSimulator != null)
            {
                interactionSimulator.usePointAndClick = false;
            }
        }
    }
}
