using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using Bluaniman.SpaceGame.Networking;
using twoloop;
using Bluaniman.SpaceGame.Debugging;

namespace Bluaniman.SpaceGame.Lobby
{
    public class PlayerSpawnSystem : NetworkBehaviour
    {
        [SerializeField] private GameObject playerPrefab = null;

        private static List<Transform> spawnPoints = new();
        private int nextIndex = 0;

        [Header("Debug")]
        [SerializeField] private UpdateStartLogger updateStartLogger = null;
        [SerializeField] private UpdateEndLogger updateEndLogger = null;

        public static void AddSpawnPoint(Transform transform)
        {
            spawnPoints.Add(transform);
            spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
        }

        public static void RemoveSpawnPoint(Transform transform)
        {
            spawnPoints.Remove(transform);
        }

        public override void OnStartServer() => MyNetworkManager.OnServerReadied += SpawnPlayer;

        [ServerCallback]

        private void OnDestroy() => MyNetworkManager.OnServerReadied -= SpawnPlayer;

        [Server]
        public void SpawnPlayer(NetworkConnection conn)
        {
            Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);
            if (spawnPoint == null)
            {
                Debug.LogError($"Missing spawn point for player {nextIndex}");
                return;
            }
            GameObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            NetworkServer.Spawn(playerInstance, conn);
            TargetSetOriginShiftFocus(conn, playerInstance.transform);
            if (updateStartLogger != null)
            {
                updateStartLogger.optionalTransformToLog = playerInstance.transform;
            }
            if (updateEndLogger != null)
            {
                updateEndLogger.optionalTransformToLog = playerInstance.transform;
            }
            nextIndex++;
        }

        [TargetRpc]
        public void TargetSetOriginShiftFocus(NetworkConnection target, Transform focusTransform)
        {
            OriginShift.singleton.focus = focusTransform;
        }
    }
}
