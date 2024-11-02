using System.Collections.Generic;
using ChaosBall.Manager;
using ChaosBall.Model;

namespace ChaosBall.Events
{
    public struct OnChangePlayerData
    {
        public Dictionary<PlayerEnum, PlayerModel> playerData;
    }
}