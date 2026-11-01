
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.FPS.Game
{
    public class GameFlowManager : MonoBehaviour
    {
        [Header("Parameters")]
        [Tooltip("Duration of the fade-to-black at the end of the game")]
        public float EndSceneLoadDelay = 3f;

        [Tooltip("The canvas group of the fade-to-black screen")]
        public CanvasGroup EndGameFadeCanvasGroup;

        [Header("Win")]
        public float DelayBeforeFadeToBlack = 4f;

        public string WinGameMessage;

        public float DelayBeforeWinMessage = 2f;

        public AudioClip VictorySound;

        [Header("Scene")]
        [Tooltip("Scene loaded when winning or losing")]
        public string MainSceneName = "Main_Scene";

        public bool GameIsEnding { get; private set; }

        float m_TimeLoadEndGameScene;
        string m_SceneToLoad;

        void Awake()
        {
            EventManager.AddListener<AllObjectivesCompletedEvent>(OnAllObjectivesCompleted);
            EventManager.AddListener<PlayerDeathEvent>(OnPlayerDeath);
        }

        void Start()
        {
            AudioUtility.SetMasterVolume(1);
        }

        void Update()
        {
            if (GameIsEnding)
            {
                float timeRatio = 1 - (m_TimeLoadEndGameScene - Time.time) / EndSceneLoadDelay;
                EndGameFadeCanvasGroup.alpha = timeRatio;

                AudioUtility.SetMasterVolume(1 - timeRatio);

                if (Time.time >= m_TimeLoadEndGameScene)
                {
                    SceneManager.LoadScene(MainSceneName);
                    GameIsEnding = false;
                }
            }
        }

        void OnAllObjectivesCompleted(AllObjectivesCompletedEvent evt)
        {
            EndGame(true);
        }

        void OnPlayerDeath(PlayerDeathEvent evt)
        {
            EndGame(false);
        }

        void EndGame(bool win)
        {
            if (GameIsEnding)
                return;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            GameIsEnding = true;

            EndGameFadeCanvasGroup.gameObject.SetActive(true);

            m_SceneToLoad = MainSceneName;

            if (win)
            {
                m_TimeLoadEndGameScene =
                    Time.time + EndSceneLoadDelay + DelayBeforeFadeToBlack;

                if (VictorySound != null)
                {
                    var audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.clip = VictorySound;
                    audioSource.playOnAwake = false;
                    audioSource.outputAudioMixerGroup =
                        AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.HUDVictory);

                    audioSource.PlayScheduled(
                        AudioSettings.dspTime + DelayBeforeWinMessage);
                }

                DisplayMessageEvent displayMessage = Events.DisplayMessageEvent;
                displayMessage.Message = WinGameMessage;
                displayMessage.DelayBeforeDisplay = DelayBeforeWinMessage;
                EventManager.Broadcast(displayMessage);
            }
            else
            {
                m_TimeLoadEndGameScene = Time.time + EndSceneLoadDelay;
            }
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<AllObjectivesCompletedEvent>(OnAllObjectivesCompleted);
            EventManager.RemoveListener<PlayerDeathEvent>(OnPlayerDeath);
        }
    }
}
