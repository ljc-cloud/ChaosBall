using ChaosBall.Net;
using UnityEngine;

namespace ChaosBall.Game
{
    public class Player : MonoBehaviour
    {
        public int playerId;
        public string playerNickname;
        public Entity.PlayerType playerType;
        public bool operation;
        public int operationLeft;
    }
}