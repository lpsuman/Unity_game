using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using UnityEngine.SceneManagement;
using Bluaniman.SpaceGame.Lobby;

namespace Bluaniman.SpaceGame.Networking
{
    public class MyNetworkManager : NetworkManager
    {
        private const string spawnablePrefabsFolderPath = "SpawnablePrefabs";
        public const int MinPlayers = 2;
        [Scene] [SerializeField] private string menuScene = string.Empty;

        [Header("Room")]
        [SerializeField] private MyNetworkRoomPlayer roomPlayerPrefab;

        public static event Action OnClientConnected;
        public static event Action OnClientDisconnected;

        public List<MyNetworkRoomPlayer> RoomPlayers { get; } = new List<MyNetworkRoomPlayer>();

        public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>(spawnablePrefabsFolderPath).ToList();

        public override void OnStartClient()
        {
            var spawnablePrefabs = Resources.LoadAll<GameObject>(spawnablePrefabsFolderPath);
            foreach (var prefab in spawnablePrefabs)
            {
                NetworkClient.RegisterPrefab(prefab);
            }
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            OnClientConnected?.Invoke();
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            OnClientDisconnected?.Invoke();
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            if (numPlayers >= maxConnections)
            {
                conn.Disconnect();
                return;
            }
            if (SceneManager.GetActiveScene().path != menuScene)
            {
                conn.Disconnect();
                return;
            }
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            if (SceneManager.GetActiveScene().path == menuScene)
            {
                MyNetworkRoomPlayer roomPlayerInstance = Instantiate(roomPlayerPrefab);
                roomPlayerInstance.IsLeader = RoomPlayers.Count == 0;
                NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
            }
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            if (conn.identity != null)
            {
                MyNetworkRoomPlayer player = conn.identity.GetComponent<MyNetworkRoomPlayer>();
                RoomPlayers.Remove(player);
                NotifyPlayersOfReadyState();
            }
            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            RoomPlayers.Clear();
        }

        public void NotifyPlayersOfReadyState()
        {
            foreach (MyNetworkRoomPlayer player in RoomPlayers)
            {
                player.HandleReadyToStart(IsReadyToStart());
            }
        }

        private bool IsReadyToStart()
        {
            if (numPlayers < MinPlayers) { return false; }
            foreach (MyNetworkRoomPlayer player in RoomPlayers)
            {
                if (!player.IsReady) { return false; }
            }
            return true;
        }
    }
}