using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using twoloop;
using Bluaniman.SpaceGame.Debugging;
using Bluaniman.SpaceGame.General;

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

        //public override void OnStartServer() => Globals.networkManager.OnServerReadied += SpawnPlayer;

        [ServerCallback]
        private void OnEnable()
        {
            Globals.networkManager.OnServerReadied += SpawnPlayer;
        }

        [ServerCallback]
        private void OnDisable()
        {
            Globals.networkManager.OnServerReadied -= SpawnPlayer;
        }

        private void OnDestroy()
        {
            Globals.networkManager.OnServerReadied -= SpawnPlayer;
        }

        [Server]
        public void SpawnPlayer(NetworkConnectionToClient conn)
        {
            Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);
            if (spawnPoint == null)
            {
                DebugHandler.NetworkLog($"Missing spawn point for player {nextIndex}!", this);
                return;
            }
            DebugHandler.CheckAndDebugLog(DebugHandler.OriginShift() != DebugHandler.OriginShiftLoggingMode.Disabled, $"Spawn point {nextIndex} at {spawnPoint.position}.");

            GameObject playerInstance = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            NetworkServer.Spawn(playerInstance, conn);
            DebugHandler.CheckAndDebugLog(DebugHandler.OriginShift() != DebugHandler.OriginShiftLoggingMode.Disabled, $"Set OS focus for player {nextIndex} to {playerInstance.transform.position}.");
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
