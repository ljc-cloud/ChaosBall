using System;
using System.Collections.Generic;
using ChaosBall.Model;
using ChaosBall.Net.Request;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall.UI
{
    public class RoomListUI : BaseUIPanel
    {
        [SerializeField] private TMP_InputField searchRoomNameInput;
        [SerializeField] private TMP_Dropdown searchRoomVisibilityDropdown;
        [SerializeField] private Button searchRoomButton;
        [SerializeField] private Button createRoomButton;
        [SerializeField] private Button roomCodeButton;
        [SerializeField] private GameObject roomTabPrefab;
        [SerializeField] private RectTransform roomTabsContainer;
        [SerializeField] private Button closeButton;

        private SearchRoomRequest _searchRoomRequest;
        private bool _show;
        private float _searchRoomTimer;
        private float _searchRoomTimerMax = 2f;
        
        public override void OnInit()
        {
            _searchRoomRequest = GameInterface.Interface.RequestManager.GetRequest<SearchRoomRequest>();
            base.OnInit();
        }

        private void Start()
        {
            searchRoomButton.onClick.AddListener(SearchRoom);
            createRoomButton.onClick.AddListener(() =>
            {
                GameInterface.Interface.UIManager.PushUIPanelAppend(UIPanelType.CreateRoomUI, ShowUIPanelType.MoveFadeIn);
            });
            roomCodeButton.onClick.AddListener(() =>
            {
                // TODO: 创建房间码加入房间UI
            });
            closeButton.onClick.AddListener(() =>
            {
                GameInterface.Interface.UIManager.PopUIPanel();
            });
        }

        private void Update()
        {
            if (!_show) return;
            _searchRoomTimer -= Time.deltaTime;
            if (_searchRoomTimer < 0f)
            {
                // 执行搜索房间
                _searchRoomRequest.SendSearchRoomRequest(roomInfo =>
                {
                    roomInfo.roomVisibility = ChaosBall.Model.RoomVisibility.None;
                    roomInfo.roomName = string.Empty;
                }, UpdateRoomList);
                _searchRoomTimer = _searchRoomTimerMax;
            }
        }

        public override void OnShow()
        {
            Debug.Log("搜索房间中...");
            // 搜索所有房间
            // SearchRoomRequest searchRoomRequest = GameInterface.Interface.RequestManager.GetRequest<SearchRoomRequest>();
            _searchRoomRequest.SendSearchRoomRequest(roomInfo =>
            {
                roomInfo.roomVisibility = ChaosBall.Model.RoomVisibility.None;
                roomInfo.roomName = string.Empty;
            }, UpdateRoomList);

            _show = true;
            _searchRoomTimer = _searchRoomTimerMax;
            
            base.OnShow();
        }

        public override void OnHide()
        {
            _show = false;
            base.OnHide();
        }

        private RoomInfo LoadRoomSearchCondition()
        {
            string roomName = searchRoomNameInput.text;
            string visibilityText = searchRoomVisibilityDropdown.options[searchRoomVisibilityDropdown.value].text;
            ChaosBall.Model.RoomVisibility roomVisibility = Enum.Parse<ChaosBall.Model.RoomVisibility>(visibilityText);
            return new RoomInfo
            {
                roomName = roomName,
                roomVisibility = roomVisibility
            };
        }

        private void SearchRoom()
        {
            string roomName = searchRoomNameInput.text;
            string visibilityText = searchRoomVisibilityDropdown.options[searchRoomVisibilityDropdown.value].text;
            ChaosBall.Model.RoomVisibility roomVisibility = Enum.Parse<ChaosBall.Model.RoomVisibility>(visibilityText);
            _searchRoomRequest.SendSearchRoomRequest(roomInfo =>
            {
                roomInfo.roomName = roomName;
                roomInfo.roomVisibility = roomVisibility;
            }, UpdateRoomList);
        }

        private void UpdateRoomList(List<RoomInfo> roomInfoList)
        {
            Debug.Log($"搜索结果:{roomInfoList.Count}");
            foreach (Transform roomTabTransform in roomTabsContainer)
            {
                Destroy(roomTabTransform.gameObject);
            }

            float spacing = roomTabsContainer.GetComponent<VerticalLayoutGroup>().spacing;
            float perHeight = roomTabPrefab.GetComponent<RectTransform>().rect.height;
            Debug.Log("RoomTab Per Height:" + perHeight);
            float height = perHeight * roomInfoList.Count + (roomInfoList.Count - 1) * spacing;
            Debug.Log("RoomTab Container Height:" + height);
            roomTabsContainer.sizeDelta = new Vector2(roomTabsContainer.sizeDelta.x, height);
            
            foreach (var roomInfo in roomInfoList)
            {
                GameObject roomTabUIGameObject = Instantiate(roomTabPrefab, roomTabsContainer);
                RoomTabUI roomTabUI = roomTabUIGameObject.GetComponent<RoomTabUI>();
                roomTabUI.SetRoomTab(roomInfo);
            }
        }
    }
}