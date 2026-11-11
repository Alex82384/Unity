using UnityEngine;
using UnityEngine.UI;
using VRTraining.Core;
using VRTraining.Scenario;

namespace VRTraining.UI
{
    // final screen every group with steps and their statuses plus restart lobby buttons
    public class ResultsPresenter : MonoBehaviour
    {
        [SerializeField] private ScenarioRunner runner;
        [SerializeField] private GameObject panel;
        [SerializeField] private RectTransform listRoot;
        [SerializeField] private Text groupHeaderPrefab;
        [SerializeField] private StepRowView stepRowPrefab;
        [SerializeField] private Text summaryLabel;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button lobbyButton;

        private void Awake() => panel.SetActive(false);

        private void OnEnable()
        {
            runner.ScenarioFinished += Show;
            restartButton.onClick.AddListener(SceneLoader.LoadTraining); // reload = clean scene state
            lobbyButton.onClick.AddListener(SceneLoader.LoadLobby);
        }

        private void OnDisable()
        {
            runner.ScenarioFinished -= Show;
            restartButton.onClick.RemoveListener(SceneLoader.LoadTraining);
            lobbyButton.onClick.RemoveListener(SceneLoader.LoadLobby);
        }

        private void Show()
        {
            int total = 0, success = 0;

            foreach (var group in runner.Groups)
            {
                var header = Instantiate(groupHeaderPrefab, listRoot);
                header.text = group.Definition.title;

                foreach (var step in group.Steps)
                {
                    Instantiate(stepRowPrefab, listRoot).Bind(step);
                    total++;
                    if (step.Status == StepStatus.Success) success++;
                }
            }

            summaryLabel.text = $"Итог: {success} из {total} шагов выполнено без ошибок";
            panel.SetActive(true);
        }
    }
}
