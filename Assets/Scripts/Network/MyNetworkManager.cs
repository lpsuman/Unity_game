using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using UnityEngine.SceneManagement;
using Bluaniman.SpaceGame.Lobby;
using Bluaniman.SpaceGame.Debugging;

namespace Bluaniman.SpaceGame.Networking
{
    public class MyNetworkManager : NetworkManager
    {
        private const string spawnablePrefabsFolderPath = "SpawnablePrefabs";
        public const int MinPlayers = 1;
        private const string gameSceneNamePrefix = "Scene Map";
        [Scene] [SerializeField] private string initScene = string.Empty;
        [Scene] [SerializeField] public string menuScene = string.Empty;
        [Scene] [SerializeField] public string gameScene = string.Empty;

        [Header("Room")]
        [SerializeField] private MyNetworkRoomPlayer roomPlayerPrefab = null;

        [Header("Game")]
        [SerializeField] private MyNetworkGamePlayer gamePlayerPrefab = null;
        [SerializeField] private PlayerEventHandler playerEventHandlerPrefab = null;
        [SerializeField] private int sceneMapId = 1;

        public event Action OnClientConnected;
        public event Action OnClientDisconnected;
        public event Action<NetworkConnectionToClient> OnServerReadied;

        public List<MyNetworkRoomPlayer> RoomPlayers { get; } = new();
        public List<MyNetworkGamePlayer> GamePlayers { get; } = new();
        public Dictionary<int, MyNetworkGamePlayer> connIdToPlayerDict = new();
        public List<PlayerEventHandler> playerEventHandlerList = new();

        public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>(spawnablePrefabsFolderPath).ToList();

        public override void OnStartClient()
        {
            var spawnablePrefabs = Resources.LoadAll<GameObject>(spawnablePrefabsFolderPath);
            foreach (var prefab in spawnablePrefabs)
            {
                NetworkClient.RegisterPrefab(prefab);
            }
        }

        public void OnEnable()
        {
            if (SceneManager.GetActiveScene().path == initScene)
            {
                SceneManager.LoadScene(menuScene);
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
                DebugHandler.NetworkLog($"Disconnecting client due to too many players ({numPlayers}).");
                conn.Disconnect();
                return;
            }
            if (SceneManager.GetActiveScene().path != menuScene)
            {
                DebugHandler.NetworkLog("Disconnecting client because we are not in a lobby.");
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
            GamePlayers.Clear();
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
                    connIdToPlayerDict[conn.connectionId] = gamePlayerInstance;
                    gamePlayerInstance.OnDestroyed += () => connIdToPlayerDict.Remove(conn.connectionId);

                    PlayerEventHandler playerEventHandlerInstance = Instantiate(playerEventHandlerPrefab);
                    NetworkServer.Spawn(playerEventHandlerInstance.gameObject, conn);
                    playerEventHandlerList.Add(playerEventHandlerInstance);
                    playerEventHandlerInstance.OnDestroyed += () => playerEventHandlerList.Remove(playerEventHandlerInstance);
                }
            }
            base.ServerChangeScene(newSceneName);
        }

        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            base.OnServerReady(conn);
            OnServerReadied?.Invoke(conn);
        }

        public override void OnClientSceneChanged()
        {
            base.OnClientSceneChanged();
            if (SceneManager.GetActiveScene().path == menuScene)
            {
                FindObjectOfType<MainMenu>().ShowLandingPage();
            }
        }
    }
}