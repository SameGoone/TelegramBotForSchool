using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotForSchool
{
    public class StateController
    {
        public string HeadEmail { get; private set; }
        public State CurrentState
        {
            get
            {
                if (stateIndex < states.Count)
                    return states[stateIndex];
                else
                    return State.FINISH;
            }
        }

        public List<StateValue> Result
        {
            get
            {
                return stateValues;
            }
        }

        protected List<State> states = new List<State> { State.CATEGORY, State.CLARIFICATION };
        private int stateIndex = 0;
        private List<StateValue> stateValues = new List<StateValue>();

        public StateController(string categoryName, Document document)
        {
            CompleteState(categoryName);
            CompleteState(document.Name);
            HeadEmail = document.HeadEmail;
            AddStateList(document.AdditionalStates);
        }

        public void CompleteState(string stateValue)
        {
            if (CurrentState == State.FINISH)
                return;

            string stateName = Tools.GetStateName(CurrentState);
            int columnNumber = (int)CurrentState;
            stateValues.Add(new StateValue(stateName, stateValue, columnNumber));
            stateIndex++;
        }

        private void AddStateList(List<State> additionalStates)
        {
            states.AddRange(additionalStates);
            states.Add(State.ADDITIONAL_INFO);
        }
    }
}
