using UnityEngine.SceneManagement;

namespace VRTraining.Core
{
    // single place knows scene names ui code does not hard-code them
    public static class SceneLoader
    {
        public const string LobbyScene = "Lobby";
        public const string TrainingScene = "Training";

        public static void LoadLobby() => SceneManager.LoadScene(LobbyScene);
        public static void LoadTraining() => SceneManager.LoadScene(TrainingScene);
    }
}
