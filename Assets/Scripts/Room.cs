using System.Collections.Generic;
using ChaosBall.Model;
using ChaosBall.Net;
using UnityEngine;

namespace ChaosBall
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        
        private void Start()
        {
            List<RoomPlayerInfo> roomPlayerInfoList = GameInterface.Interface.RoomManager.RoomPlayerList;
            int localPlayerId = GameInterface.Interface.LocalPlayerInfo.id;

            Vector3 position = new Vector3(-50f, 0, -200f);
            foreach (var roomPlayerInfo in roomPlayerInfoList)
            {
                GameObject playerGameObject = Instantiate(playerPrefab, position, Quaternion.identity);
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