using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Assets.Code.Scripts.Common.Commands
{
    public class Command
    {
        #region Constants

        public const char SEPERATOR = ':';

        #endregion

        #region Constructors

        public Command(CommandType CommandType, object Data)
        {
            this.CommandType = CommandType;
            this.Data = Data;
        }

        #endregion

        #region Properties

        public CommandType CommandType { get; set; }

        public object Data { get; set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            string commandType = CommandType.ToString();
            string data = Data?.ToString() ?? string.Empty;

            return $"{commandType}:{data}";
        }

        public static Command FromString(string command)
        {
            CommandType commandType = (CommandType)Enum.Parse(typeof(CommandType), command.Substring(0, command.IndexOf(SEPERATOR)));
            object data = command.Substring(command.IndexOf(SEPERATOR) + 1);

            return new Command(commandType, data);
        }

        public string ObjectToJsonData()
        {
            return JsonConvert.SerializeObject(Data);
        }

        public T DataToJson<T>()
        { 
            return JsonConvert.DeserializeObject<T>(Data.ToString());
        }

        #endregion
    }
}
