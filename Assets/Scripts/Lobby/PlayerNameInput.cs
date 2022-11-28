using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Bluaniman.SpaceGame.Debugging;

namespace Bluaniman.SpaceGame.Lobby
{
    public class PlayerNameInput : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_InputField nameInputField = null;
        [SerializeField] private Button confirmButton = null;
        [SerializeField] private GameObject playerNamePanel = null;
        [SerializeField] private TMP_Text playerNameTextField = null;
        public static string DisplayName { get; private set; }
        private const string PlayerPrefsNameKey = "PlayerName";

        private void Start()
        {
            SetUpInputField();
            playerNamePanel.SetActive(false);
        }

        private void SetUpInputField()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) {
                DebugHandler.CheckAndDebugLog(DebugHandler.mainMenu, "No default player name found!");
                return;
            }
            string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);
            nameInputField.text = defaultName;
            SetPlayerName(defaultName);
            DebugHandler.CheckAndDebugLog(DebugHandler.mainMenu, $"Default player name set as {defaultName}.");
        }

        public void SetPlayerName(string name)
        {
            confirmButton.interactable = !string.IsNullOrEmpty(name);
        }

        public void SavePlayerName()
        {
            DisplayName = nameInputField.text;
            PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);
            playerNamePanel.SetActive(true);
            playerNameTextField.text = DisplayName;
            DebugHandler.CheckAndDebugLog(DebugHandler.mainMenu, $"Saved player's name as {DisplayName}.");
        }
    }
}