using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
            playerNamePanel.SetActive(false);
            SetUpInputField();
        }

        private void SetUpInputField()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) { return; }
            string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);
            nameInputField.text = defaultName;
            SetPlayerName(defaultName);
        }

        public void SetPlayerName(string name)
        {
            Debug.Log($"Set player name as: {name}");
            confirmButton.interactable = !string.IsNullOrEmpty(name);
        }

        public void SavePlayerName()
        {
            DisplayName = nameInputField.text;
            PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);
            playerNamePanel.SetActive(true);
            playerNameTextField.text = DisplayName;
        }
    }
}