using System.Collections.Generic;
using ChaosBall.Game;
using ChaosBallServer.Model;
using UnityEngine;

namespace ChaosBall
{
    public class Room : MonoBehaviour
    {
        private void Start()
        {
            List<RoomPlayerInfo> roomPlayerInfoList = GameInterface.Interface.RoomManager.RoomPlayerList;
            int localPlayerId = GameInterface.Interface.PlayerInfo.id;

            Vector3 position = new Vector3(-50f, 0, -200f);
            GameObject prefab = GameInterface.Interface.PlayerPrefab;
            foreach (var roomPlayerInfo in roomPlayerInfoList)
            {
                GameObject playerGameObject = Instantiate(prefab, position, Quaternion.identity);
                Entity entity = playerGameObject.GetComponent<Entity>();
                if (roomPlayerInfo.id == localPlayerId)
                {
                    entity.playerType = Entity.PlayerType.Local;
                    entity.playerId = localPlayerId;
                }
                else
                {
                    entity.playerType = Entity.PlayerType.Remote;
                    entity.playerId = roomPlayerInfo.id;
                }

                GameInterface.Interface.GameFrameSyncManager.AddEntity(entity);
                position.x += 100f;
            }
        }
    }
}