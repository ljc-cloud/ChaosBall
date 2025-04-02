using System.Collections.Generic;
using ChaosBall.Game;
using ChaosBall.Model;
using ChaosBall.Net;
using UnityEngine;

namespace ChaosBall
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        
        private void Awake()
        {
            List<RoomPlayerInfo> roomPlayerInfoList = GameInterface.Interface.RoomManager.RoomPlayerList;
            PlayerInfo localPlayerInfo = GameInterface.Interface.LocalPlayerInfo;
            
            Vector3 position = new Vector3(-50f, 0, -200f);
            foreach (var roomPlayerInfo in roomPlayerInfoList)
            {
                GameObject playerGameObject = Instantiate(playerPrefab, position, Quaternion.identity);
                Player player = playerGameObject.GetComponent<Player>();
                if (roomPlayerInfo.id == localPlayerInfo.id)
                {
                    player.playerType = Entity.PlayerType.Local;
                    player.playerId = localPlayerInfo.id;
                    player.playerNickname = localPlayerInfo.nickname;
                }
                else
                {
                    player.playerType = Entity.PlayerType.Remote;
                    player.playerId = roomPlayerInfo.id;
                    player.playerNickname = roomPlayerInfo.nickname;
                }

                player.operation = false;
                GameManager.Instance.AddPlayer(player);
            }
            
        }
    }
}