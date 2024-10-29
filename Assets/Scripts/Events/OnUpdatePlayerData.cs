using System.Collections.Generic;

namespace Events
{
    public struct OnUpdatePlayerData
    {
        public Dictionary<PlayerEnum, PlayerModel> playerData;
    }
}