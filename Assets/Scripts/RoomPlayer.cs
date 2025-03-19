using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChaosBall
{
    public class RoomPlayer : MonoBehaviour
    {
        [SerializeField] private TextMeshPro nickNameText;
        [SerializeField] private Image readyImage;
        [SerializeField] private SkinnedMeshRenderer bodyMeshRenderer;
        [SerializeField] private SkinnedMeshRenderer faceMeshRenderer;


        private const string READY_TEXT = "准备";
        private const string UNREADY_TEXT = "取消准备";
        
        public int RoomIndex { get; private set; }

        public void SetRoomPlayer(int index, string nickname, Material bodyMaterial, Material faceMaterial)
        {
            RoomIndex = index;
            nickNameText.text = nickname;
            bodyMeshRenderer.material = bodyMaterial;
            faceMeshRenderer.material = faceMaterial;
            faceMeshRenderer.material = faceMaterial;
        }

        public void SetReady(bool ready)
        {
            readyImage.enabled = ready;
        }
    }
}