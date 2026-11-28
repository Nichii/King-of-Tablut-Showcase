using Junnav.KingOfTablut.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Junnav.KingOfTablut
{
    public class TablutMatchMenu : MonoBehaviour
    {
        private enum Screen
        {
            Start,
            Match,
            Pause,
            Tutorial,
            Result
        }

        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private GameObject backdrop;
        [SerializeField] private GameObject[] tutorialPages;

        [SerializeField] private Button playLocal;
        [SerializeField] private Button playComputer;
        [SerializeField] private Button startTutorial;
        [SerializeField] private Button resume;
        [SerializeField] private Button restart;
        [SerializeField] private Button pauseTutorial;
        [SerializeField] private Button pauseMainMenu;
        [SerializeField] private Button tutorialBack;
        [SerializeField] private Button tutorialNext;
        [SerializeField] private Button playAgain;
        [SerializeField] private Button resultMainMenu;
        [SerializeField] private TMP_Text tutorialNextLabel;
        [SerializeField] private TMP_Text tutorialProgress;
        [SerializeField] private TMP_Text resultTitle;
        [SerializeField] private TMP_Text resultDetail;

        private TablutMatchController match;
        private Canvas matchHud;
        private Screen screen;
        private Screen tutorialReturn;
        private int tutorialPage;

        public bool IsPlaying => screen == Screen.Match;

        private void Awake()
        {
            playLocal.onClick.AddListener(() => StartMatch(false));
            playComputer.onClick.AddListener(() => StartMatch(true));
            startTutorial.onClick.AddListener(OpenTutorial);
            resume.onClick.AddListener(() => Show(Screen.Match));
            restart.onClick.AddListener(RestartMatch);
            pauseTutorial.onClick.AddListener(OpenTutorial);
            pauseMainMenu.onClick.AddListener(() => Show(Screen.Start));
            tutorialBack.onClick.AddListener(PreviousPage);
            tutorialNext.onClick.AddListener(NextPage);
            playAgain.onClick.AddListener(RestartMatch);
            resultMainMenu.onClick.AddListener(() => Show(Screen.Start));
        }

        internal void Bind(TablutMatchController controller, Canvas hud)
        {
            match = controller;
            matchHud = hud;
            Show(Screen.Start);
        }

        public void OpenPause()
        {
            if (IsPlaying)
            {
                Show(Screen.Pause);
            }
        }

        internal void ShowResult(GameState outcome)
        {
            bool defendersWon = outcome == GameState.WhiteWon;
            resultTitle.text = defendersWon ? "Defenders win" : "Attackers win";
            resultDetail.text = defendersWon ? "The king reached the edge." : "The attackers captured the king.";
            Show(Screen.Result);
        }

        private void StartMatch(bool computerDefender)
        {
            Show(Screen.Match);
            match.StartMatch(computerDefender);
        }

        private void RestartMatch()
        {
            Show(Screen.Match);
            match.RestartMatch();
        }

        private void OpenTutorial()
        {
            tutorialReturn = screen;
            tutorialPage = 0;
            UpdateTutorial();
            Show(Screen.Tutorial);
        }

        private void PreviousPage()
        {
            if (tutorialPage == 0)
            {
                Show(tutorialReturn);
                return;
            }

            tutorialPage--;
            UpdateTutorial();
        }

        private void NextPage()
        {
            if (tutorialPage == tutorialPages.Length - 1)
            {
                Show(tutorialReturn);
                return;
            }

            tutorialPage++;
            UpdateTutorial();
        }

        private void UpdateTutorial()
        {
            for (int i = 0; i < tutorialPages.Length; i++)
            {
                tutorialPages[i].SetActive(i == tutorialPage);
            }

            tutorialProgress.text = $"{tutorialPage + 1} / {tutorialPages.Length}";
            tutorialNextLabel.text = tutorialPage == tutorialPages.Length - 1 ? "Done" : "Next";
        }

        private void Show(Screen next)
        {
            screen = next;
            if (matchHud != null)
            {
                matchHud.enabled = IsPlaying;
            }

            backdrop.SetActive(!IsPlaying);
            startPanel.SetActive(next == Screen.Start);
            pausePanel.SetActive(next == Screen.Pause);
            tutorialPanel.SetActive(next == Screen.Tutorial);
            resultPanel.SetActive(next == Screen.Result);

            // Set focus after activating the panel so its buttons can be reached from the keyboard.
            Button firstButton = next switch
            {
                Screen.Start => playLocal,
                Screen.Pause => resume,
                Screen.Tutorial => tutorialNext,
                Screen.Result => playAgain,
                _ => null
            };

            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(firstButton != null ? firstButton.gameObject : null);
            }
        }
    }
}
