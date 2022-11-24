using System;
using UnityEngine;
using Mirror;
using TMPro;

namespace Bluaniman.SpaceGame.Chat
{
    public class ChatController : NetworkBehaviour
    {
        [SerializeField] private GameObject chatUI = null;
        [SerializeField] private TMP_Text chatText = null;
        [SerializeField] private TMP_InputField chatInputField = null;

        private static event Action<string> OnMessage;

        public override void OnStartAuthority()
        {
            Debug.Log("start auth");
            chatUI.SetActive(true);
            OnMessage += HandleNewMessage;
        }

        [ClientCallback]
        private void OnDestroy()
        {
            if (!isOwned) { return; }

            OnMessage -= HandleNewMessage;
        }

        private void HandleNewMessage(string message)
        {
            chatText.text += message;
        }

        [Client]
        public void Send(string message)
        {
            // changed input system
            // if (!Input.GetKeyDown(KeyCode.Return)) { return; }
            if (string.IsNullOrWhiteSpace(message)) { return; }

            CmdSendMessage(message);
            chatInputField.text = string.Empty;
        }

        [Command]
        private void CmdSendMessage(string message)
        {
            RpcHandleMessage($"[{connectionToClient.connectionId}]: {message}");
        }

        [ClientRpc]
        private void RpcHandleMessage(string message)
        {
            OnMessage?.Invoke($"\n{message}");
        }
    }
}