using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Scripts.Common.Commands
{
    public enum CommandType
    {
        Name,
        NameResp,
        Disconnect,
        InformClientPlayerJoined,
        InformClientPlayerLeft,
        InformClientGameInit,
        InformClientTurn,
        PlayDone,
        PlayDoneResp,
        InformStatus,
        InformLose
    }
}
