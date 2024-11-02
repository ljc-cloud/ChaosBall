using System.Collections.Generic;
using ChaosBall.Manager;
using ChaosBall.Model;

namespace ChaosBall.Events
{
    public struct OnUpdatePlayerData
    {
        public Dictionary<PlayerEnum, PlayerModel> playerData;
    }
}