using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace TelegramBotForSchool
{
    public class Document
    {
        public string Name { get; private set; }

        public List<State> AdditionalStates { get; private set; }

        public string HeadEmail { get; private set; }

        public Document(string name, List<State> additionalStates, string headEmail)
        {
            Name = name;
            AdditionalStates = additionalStates;
            HeadEmail = headEmail;
        }
    }
}

