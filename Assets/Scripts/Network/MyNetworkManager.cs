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
        public const int MinPlayers = 1;
        private const string gameSceneNamePrefix = "Scene Map";
        [Scene] [SerializeField] private string menuScene = string.Empty;

        [Header("Room")]
        [SerializeField] private MyNetworkRoomPlayer roomPlayerPrefab = null;

        [Header("Game")]
        [SerializeField] private MyNetworkGamePlayer gamePlayerPrefab = null;
        [SerializeField] private int sceneMapId = 1;

        public static event Action OnClientConnected;
        public static event Action OnClientDisconnected;
        public static event Action<NetworkConnectionToClient> OnServerReadied;

        public List<MyNetworkRoomPlayer> RoomPlayers { get; } = new();
        public List<MyNetworkGamePlayer> GamePlayers { get; } = new();


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
            if (!autoCreatePlayer)
            {
                NetworkClient.AddPlayer();
            }
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
                Debug.Log($"Disconnecting client due to too many players ({numPlayers}).");
                conn.Disconnect();
                return;
            }
            if (SceneManager.GetActiveScene().path != menuScene)
            {
                Debug.Log("Disconnecting client because we are not in a lobby.");
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
            foreach (MyNetworkRoomPlayer player in new List<MyNetworkRoomPlayer>(RoomPlayers))
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

        public void StartGame()
        {
            if (SceneManager.GetActiveScene().path == menuScene)
            {
                if (!IsReadyToStart()) { return; }
                ServerChangeScene($"{gameSceneNamePrefix} {sceneMapId}");
            }
        }

        public override void ServerChangeScene(string newSceneName)
        {
            if (SceneManager.GetActiveScene().path == menuScene && newSceneName.StartsWith(gameSceneNamePrefix))
            {
                for (int i = RoomPlayers.Count - 1; i >= 0; i--)
                {
                    NetworkConnectionToClient conn = RoomPlayers[i].connectionToClient;
                    MyNetworkGamePlayer gamePlayerInstance = Instantiate(gamePlayerPrefab);
                    gamePlayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);
                    NetworkServer.Destroy(conn.identity.gameObject);
                    NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject);
                }
            }
            base.ServerChangeScene(newSceneName);
        }

        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            base.OnServerReady(conn);
            OnServerReadied?.Invoke(conn);
        }

        //public override void OnServerSceneChanged(string sceneName)
        //{
        //    if (sceneName.StartsWith(gameSceneNamePrefix))
        //    {
        //        GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
        //        NetworkServer.Spawn(playerSpawnSystemInstance);
        //    }
        //}
    }
}