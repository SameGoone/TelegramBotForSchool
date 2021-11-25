using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotForSchool
{
    public struct StateValue
    {
        public readonly string Name;
        public readonly string Value;
        public readonly int ColumnNumber;

        public StateValue(string name, string value, int columnNumber)
        {
            Name = name;
            Value = value;
            ColumnNumber = columnNumber;
        }
    }
}
