using System.Collections;
using System.Collections.Generic;
using Bluaniman.SpaceGame.Debugging;
using Bluaniman.SpaceGame.Input;
using Bluaniman.SpaceGame.Lobby;
using Bluaniman.SpaceGame.Network;
using Bluaniman.SpaceGame.Player;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Bluaniman.SpaceGame
{
    public class PauseMenuController : MyNetworkBehavior
    {
        private enum CurrentPanelShown
        {
            None,
            Main,
            Controls,
            ConfirmLeave
        }

        [SerializeField] private GameObject pauseMenuCanvas = null;
        [SerializeField] private GameObject pauseMenuPanel = null;
        [SerializeField] private GameObject controlsPanel = null;
        [SerializeField] private GameObject confirmLeavePanel = null;
        [SerializeField] private TMPro.TMP_Text pauseText = null;
        [SerializeField] private Button pauseButton = null;
        [SerializeField] private Button unpauseButton = null;
        private List<GameObject> panelsArray = null;

        private CurrentPanelShown currentlyShownPanel;
        private CurrentPanelShown CurrentlyShownPanel
        {
            get
            {
                return currentlyShownPanel;
            }
            set
            {
                panelsArray.ForEach(panel => panel.SetActive(false));
                if (value != CurrentPanelShown.None)
                {
                    panelsArray[0].SetActive(true);
                    panelsArray[(int)value].SetActive(true);
                }
                currentlyShownPanel = value;
            }
        }

        public void Start()
        {
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), "PauseMenuController start", this);
            panelsArray = new();
            panelsArray.Add(pauseMenuCanvas);
            panelsArray.Add(pauseMenuPanel);
            panelsArray.Add(controlsPanel);
            panelsArray.Add(confirmLeavePanel);
            CurrentlyShownPanel = CurrentPanelShown.None;
            BindEsc();
            HandleGameUnpaused(null);
        }

        public void OnEnable()
        {
            PlayerEventHandler.OnGamePaused += HandleGamePaused;
            PlayerEventHandler.OnGameUnpaused += HandleGamePaused;
        }

        public void OnDisable()
        {
            PlayerEventHandler.OnGamePaused -= HandleGamePaused;
            PlayerEventHandler.OnGameUnpaused -= HandleGamePaused;
        }

        public void OnDestroy()
        {
            PlayerEventHandler.OnGamePaused -= HandleGamePaused;
            PlayerEventHandler.OnGameUnpaused -= HandleGamePaused;
        }

        private void BindEsc()
        {
            Controls controls = new();
            controls.Player.Menu.performed += ctx => HandleEscPressed();
            controls.Enable();
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), "Bound ESC", this);
        }
        
        public void HandleEscPressed()
        {
            DebugHandler.CheckAndDebugLog(DebugHandler.Input(), "ESC pressed", this);
            switch (CurrentlyShownPanel)
            {
                case CurrentPanelShown.None:
                    CurrentlyShownPanel = CurrentPanelShown.Main;
                    break;
                case CurrentPanelShown.Main:
                    CurrentlyShownPanel = CurrentPanelShown.None;
                    break;
                default:
                    CurrentlyShownPanel = CurrentPanelShown.Main;
                    break;
            }
        }

        public void ShowControls()
        {
            CurrentlyShownPanel = CurrentPanelShown.Controls;
        }

        public void ConfirmLeave()
        {
            CurrentlyShownPanel = CurrentPanelShown.ConfirmLeave;
        }

        public void Pause()
        {
            PlayerEventHandler.CmdRequestPauseGame();
        }

        public void Unpause()
        {
            PlayerEventHandler.CmdRequestUnpauseGame();
        }

        public void HandleGamePaused(string pausingPlayer)
        {
            pauseButton.enabled = false;
            unpauseButton.enabled = true;
            pauseText.SetText($"Game paused by {pausingPlayer}");
        }

        public void HandleGameUnpaused(string unpausingPlayer)
        {

            pauseButton.enabled = true;
            unpauseButton.enabled = false;
            pauseText.SetText(unpausingPlayer == null ? "Click to pause the game" : $"Game unpaused by {unpausingPlayer}");
        }
    }
}
