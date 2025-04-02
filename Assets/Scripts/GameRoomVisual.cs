using System;
using System.Collections.Generic;
using System.Linq;
using ChaosBall.Event;
using ChaosBall.Model;
using ChaosBall.Net.Request;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall
{
    public class GameRoomVisual : MonoBehaviour
    {
        [SerializeField] private Material[] bodyMaterialArray;
        [SerializeField] private Material[] faceMaterialArray;
        [SerializeField] private GameObject roomPlayerPrefab;
        [SerializeField] private Transform[] roomPlayerPositionArray;

        // private int _mCurrentPlayerIndex;
        private Dictionary<RoomPlayerInfo, RoomPlayer> _mRoomPlayerInfoToRoomPlayerDict = new();
        // private Dictionary<int, RoomPlayer>

        private bool[] _mRoomPlayerPositionAvailable;

        private void Start()
        {
            _mRoomPlayerPositionAvailable = new bool[roomPlayerPositionArray.Length];
            Array.Fill(_mRoomPlayerPositionAvailable, true);
            
            // GameInterface.Interface.RoomManager.OnRoomPlayerJoin += OnRoomPlayerJoin;
            GameInterface.Interface.EventSystem.Subscribe<RoomPlayerJoinEvent>(OnRoomPlayerJoin);
            GameInterface.Interface.EventSystem.Subscribe<RoomPlayerQuitEvent>(OnRoomPlayerQuit);
            // GameInterface.Interface.RoomManager.OnRoomPlayerQuit += OnRoomPlayerQuit;
            GameInterface.Interface.EventSystem.Subscribe<RoomPlayerReadyChangeEvent>(OnRoomPlayerReadyChanged);
            // GameInterface.Interface.RoomManager.OnRoomPlayerReadyChanged += OnRoomPlayerReadyChanged;
            SpawnRoomPlayers();
        }
        private void OnDestroy()
        {
            // GameInterface.Interface.RoomManager.OnRoomPlayerJoin -= OnRoomPlayerJoin;
            GameInterface.Interface.EventSystem.Unsubscribe<RoomPlayerJoinEvent>(OnRoomPlayerJoin);
            GameInterface.Interface.EventSystem.Unsubscribe<RoomPlayerQuitEvent>(OnRoomPlayerQuit);
            GameInterface.Interface.EventSystem.Unsubscribe<RoomPlayerReadyChangeEvent>(OnRoomPlayerReadyChanged);
            // GameInterface.Interface.RoomManager.OnRoomPlayerQuit -= OnRoomPlayerQuit;
            // GameInterface.Interface.RoomManager.OnRoomPlayerReadyChanged -= OnRoomPlayerReadyChanged;
        }

        private void OnRoomPlayerJoin(RoomPlayerJoinEvent e)
        {
            Invoker.Instance.DelegateList.Add(() =>
            {
                Debug.Log("OnRoomPlayerJoin");

                int availableIndex = -1;
                for (int i = 0; i < _mRoomPlayerPositionAvailable.Length; i++)
                {
                    if (_mRoomPlayerPositionAvailable[i])
                    {
                        availableIndex = i;
                        break;
                    }
                }
                
                RoomPlayer roomPlayer = SpawnRoomPlayer(availableIndex, e.roomPlayerInfo);
                _mRoomPlayerInfoToRoomPlayerDict.Add(e.roomPlayerInfo, roomPlayer);
            });
        }
        private void OnRoomPlayerQuit(RoomPlayerQuitEvent e)
        {
            Invoker.Instance.DelegateList.Add(() =>
            {
                RoomPlayer roomPlayer = _mRoomPlayerInfoToRoomPlayerDict[e.roomPlayerInfo];
                _mRoomPlayerInfoToRoomPlayerDict.Remove(e.roomPlayerInfo);
                int playerIndex = roomPlayer.RoomIndex;
                _mRoomPlayerPositionAvailable[playerIndex] = true;
                Destroy(roomPlayer.gameObject);
            });
        }
        private void OnRoomPlayerReadyChanged(RoomPlayerReadyChangeEvent e)
        {
            Invoker.Instance.DelegateList.Add(() =>
            {
                RoomPlayer roomPlayer = _mRoomPlayerInfoToRoomPlayerDict[e.roomPlayerInfo];
                roomPlayer.SetReady(e.roomPlayerInfo.ready);
                // int localPlayerId = GameInterface.Interface.PlayerInfo.id;
            });
        }

        private void SpawnRoomPlayers()
        {
            List<RoomPlayerInfo> roomPlayerInfoList = GameInterface.Interface.RoomManager.RoomPlayerList;
            for (int i = 0; i < roomPlayerInfoList.Count; i++)
            {
                RoomPlayer roomPlayer = SpawnRoomPlayer(i, roomPlayerInfoList[i]);
                _mRoomPlayerInfoToRoomPlayerDict.Add(roomPlayerInfoList[i], roomPlayer);
                _mRoomPlayerPositionAvailable[i] = false;
            }
        }

        private RoomPlayer SpawnRoomPlayer(int index, RoomPlayerInfo roomPlayerInfo)
        {
            Transform roomPlayerPosition = roomPlayerPositionArray[index];
            GameObject roomPlayerGameObject = Instantiate(roomPlayerPrefab, roomPlayerPosition.position, roomPlayerPosition.rotation);
            RoomPlayer roomPlayer = roomPlayerGameObject.GetComponent<RoomPlayer>();
            roomPlayer.SetRoomPlayer(index, roomPlayerInfo.nickname, bodyMaterialArray[index], faceMaterialArray[index]);
            roomPlayer.SetReady(roomPlayerInfo.ready);
            return roomPlayer;
        }
    }
}