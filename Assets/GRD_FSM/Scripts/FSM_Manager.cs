using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GRD.FSM
{
    public class FSM_Manager : MonoBehaviour
    {
        [SerializeField] string FSM_Name;

        [SerializeField] List<FSM_State> _states;
        [SerializeField] List<FSM_Parameter> _parameters;
        [SerializeField] int _defaultState;
        [SerializeField] FSM_Transition[] _anyStateTransitions;

        enum StartMethod
        {
            Awake,
            Start
        }
        [SerializeField] StartMethod _startFSMMethod;

        [System.Flags]
        enum TransitionMethod
        {
            Update = 1,
            FixedUpdate = 2,
            LateUpdate = 4
        }
        [SerializeField] TransitionMethod _transitionExecutionMethod = TransitionMethod.Update;

        [SerializeField] int _currentState = 0;

        #region Any State Editor
#if UNITY_EDITOR
        [SerializeField] private Rect anyStateEditorRect = new Rect(300, 0, 200, 50);
        [SerializeField] private bool anyStateIsDragged;
        [SerializeField] private bool anyStateIsSelected;

        public void DragAnyStateBox(Vector2 delta)
        {
            anyStateEditorRect.position += delta;
        }

        public void AddAnyStateTransition(int endStateIndex)
        {
            FSM_Transition newTransition = new FSM_Transition(endStateIndex);
            if (_anyStateTransitions == null)
            {
                _anyStateTransitions = new FSM_Transition[] { newTransition };
                return;
            }

            FSM_Transition[] newArray = new FSM_Transition[_anyStateTransitions.Length + 1];
            for (int i = 0; i < _anyStateTransitions.Length; i++)
            {
                newArray[i] = _anyStateTransitions[i];
            }
            newArray[newArray.Length - 1] = newTransition;
            _anyStateTransitions = newArray;
        }

        public void ChangeAnyStateTransitionIndex(int oldIndex, int newIndex)
        {
            if (newIndex < 0 || newIndex >= _anyStateTransitions.Length)
                return;

            FSM_Transition temp = _anyStateTransitions[oldIndex];
            int currentIndex = oldIndex;

            while (currentIndex != newIndex)
            {
                int nextIndex = oldIndex > newIndex ? (currentIndex - 1) : (currentIndex + 1);
                _anyStateTransitions[currentIndex] = _anyStateTransitions[nextIndex];
                currentIndex = nextIndex;
            }

            _anyStateTransitions[newIndex] = temp;
        }

        public void OnStateDeleted(int deletedStateIndex)
        {
            if (_anyStateTransitions == null)
                return;

            FSM_Transition[] newTransitionArray = _anyStateTransitions.
                Where(transition => transition.toStateIndex != deletedStateIndex).ToArray();
            _anyStateTransitions = newTransitionArray;
            foreach (FSM_Transition t in _anyStateTransitions)
            {
                if (t.toStateIndex > deletedStateIndex)
                {
                    t.toStateIndex--;
                }
            }
        }
#endif
        #endregion

        #region Get and Set Parameters
        public object GetParameterValue(int parameterIndex)
        {
            return _parameters[parameterIndex].GetValue();
        }

        public object GetParameterValue(string parameterName)
        {
            foreach (FSM_Parameter p in _parameters)
            {
                if (p.name == parameterName)
                    return p.GetValue();
            }

            return null;
        }

        public void SetInt(int parameterIndex, int value)
        {
            FSM_Parameter parameter = _parameters[parameterIndex];
            if (parameter.parameterType != FSM_Parameter.ParameterType.Integer)
            {
                Debug.LogError("FSM_Parameter " + parameter.name + " is not an Integer");
                return;
            }

            parameter.intValue = value;
        }

        public void SetInt(string parameterName, int value)
        {
            FSM_Parameter parameter = _parameters.Where((p) => p.name == parameterName).FirstOrDefault();
            if (parameter == null)
            {
                Debug.LogError("Parameter " + parameterName + " not found.");
            }
            else
            {
                SetInt(_parameters.IndexOf(parameter), value);
            }
        }

        public void SetFloat(int parameterIndex, float value)
        {
            FSM_Parameter parameter = _parameters[parameterIndex];
            if (parameter.parameterType != FSM_Parameter.ParameterType.Float)
            {
                Debug.LogError("FSM_Parameter " + parameter.name + " is not a Float");
                return;
            }

            parameter.floatValue = value;
        }

        public void SetFloat(string parameterName, float value)
        {
            FSM_Parameter parameter = _parameters.Where((p) => p.name == parameterName).FirstOrDefault();
            if (parameter == null)
            {
                Debug.LogError("Parameter " + parameterName + " not found.");
            }
            else
            {
                SetFloat(_parameters.IndexOf(parameter), value);
            }
        }

        public void SetBool(int parameterIndex, bool value)
        {
            FSM_Parameter parameter = _parameters[parameterIndex];
            if (parameter.parameterType != FSM_Parameter.ParameterType.Boolean)
            {
                Debug.LogError("FSM_Parameter " + parameter.name + " is not a Boolean");
                return;
            }

            parameter.boolValue = value;
        }

        public void SetBool(string parameterName, bool value)
        {
            FSM_Parameter parameter = _parameters.Where((p) => p.name == parameterName).FirstOrDefault();
            if (parameter == null)
            {
                Debug.LogError("Parameter " + parameterName + " not found.");
            }
            else
            {
                SetBool(_parameters.IndexOf(parameter), value);
            }
        }

        public void SetTrigger(int parameterIndex)
        {
            FSM_Parameter parameter = _parameters[parameterIndex];
            if (parameter.parameterType != FSM_Parameter.ParameterType.Trigger)
            {
                Debug.LogError("FSM_Parameter " + parameter.name + " is not a Trigger");
                return;
            }

            parameter.boolValue = true;
        }

        public void SetTrigger(string parameterName)
        {
            FSM_Parameter parameter = _parameters.Where((p) => p.name == parameterName).FirstOrDefault();
            if (parameter == null)
            {
                Debug.LogError("Parameter " + parameterName + " not found.");
            }
            else
            {
                SetTrigger(_parameters.IndexOf(parameter));
            }
        }

        private void ResetAllTriggers()
        {
            IEnumerable<FSM_Parameter> triggers = _parameters.Where((parameter) => parameter.parameterType == FSM_Parameter.ParameterType.Trigger);

            foreach (FSM_Parameter trigger in triggers)
            {
                trigger.boolValue = false;
            }
        }

#if UNITY_EDITOR
        public void SetParameterIndex(int oldIndex, int newIndex)
        {
            if (newIndex < 0 || newIndex >= _parameters.Count)
                return;

            FSM_Parameter temp = _parameters[oldIndex];
            int currentIndex = oldIndex;

            while (currentIndex != newIndex)
            {
                int nextIndex = oldIndex > newIndex ? (currentIndex - 1) : (currentIndex + 1);
                _parameters[currentIndex] = _parameters[nextIndex];
                currentIndex = nextIndex;
            }

            _parameters[newIndex] = temp;
            UpdateParameterIndexInTransitions(oldIndex, newIndex);
        }

        public void UpdateParameterIndexInTransitions(int oldIndex, int newIndex)
        {
            foreach (FSM_State state in _states)
            {
                if (state.transitions == null)
                    continue;

                foreach (FSM_Transition transition in state.transitions)
                {
                    transition.OnParameterIndexChange(oldIndex, newIndex);
                }
            }

            foreach (FSM_Transition anyStateTransition in _anyStateTransitions)
            {
                anyStateTransition.OnParameterIndexChange(oldIndex, newIndex);
            }
        }

        public void DeleteParameterAtIndex(int parameterIndex)
        {
            DeleteTransitionConditionsWithParameter(parameterIndex);
            _parameters.RemoveAt(parameterIndex);
        }

        public void DeleteTransitionConditionsWithParameter(int parameterIndex)
        {
            foreach (FSM_State state in _states)
            {
                if (state.transitions == null)
                    continue;

                foreach (FSM_Transition transition in state.transitions)
                {
                    transition.OnParameterDeleted(parameterIndex);
                }
            }

            foreach (FSM_Transition anyStateTransition in _anyStateTransitions)
            {
                anyStateTransition.OnParameterDeleted(parameterIndex);
            }
        }
#endif
        #endregion

        #region Get State Info
        public int GetStateIdByName(string stateName)
        {
            if (!_states.Exists(x => x.name == stateName))
            {
                Debug.LogError("FSM does not contain a state named as '" + stateName + "'");
                return -1;
            }
            return _states.IndexOf(_states.First(x => x.name == stateName));
        }

        public int GetCurrentStateId()
        {
            return _currentState;
        }

        public string GetCurrentStateName()
        {
            return _states[_currentState].name;
        }
        #endregion

        #region Behaviour
        private void Awake()
        {
            foreach (FSM_State state in _states)
            {
                if (state.behaviour != null)
                {
                    state.behaviour.Setup(this);
                }
            }

            if (_startFSMMethod == StartMethod.Awake)
            {
                StartFSM();
            }
        }

        private void Start()
        {
            if (_startFSMMethod == StartMethod.Start)
            {
                StartFSM();
            }
        }

        private void StartFSM()
        {
            _currentState = _defaultState;
            if (_states[_currentState].behaviour != null)
            {
                _states[_currentState].behaviour.OnEnter();
            }
        }

        private void Update()
        {
            if (_states[_currentState].behaviour != null)
            {
                _states[_currentState].behaviour.OnUpdate();
            }

            //Check Transitions
            if (_transitionExecutionMethod.HasFlag(TransitionMethod.Update))
            {
                if (CheckTransitions())
                {
                    ResetAllTriggers();
                }
            }
        }

        private void FixedUpdate()
        {
            if (_states[_currentState].behaviour != null)
            {
                _states[_currentState].behaviour.OnFixedUpdate();
            }

            //Check Transitions
            if (_transitionExecutionMethod.HasFlag(TransitionMethod.FixedUpdate))
            {
                if (CheckTransitions())
                {
                    ResetAllTriggers();
                }
            }
        }

        private void LateUpdate()
        {
            if (_states[_currentState].behaviour != null)
            {
                _states[_currentState].behaviour.OnLateUpdate();
            }

            //Check Transitions
            if (_transitionExecutionMethod.HasFlag(TransitionMethod.LateUpdate))
            {
                if (CheckTransitions())
                {
                    ResetAllTriggers();
                }
            }

            ResetAllTriggers();
        }

        private bool CheckTransitions()
        {
            int newStateIndex = CheckAnyStateTransitions();
            if (newStateIndex >= 0 && newStateIndex != _currentState)
            {
                ChangeState(newStateIndex);
                return true;
            }

            newStateIndex = _states[_currentState].CheckTransitions(this);
            if (newStateIndex >= 0)
            {
                ChangeState(newStateIndex);
                return true;
            }

            return false;
        }

        private int CheckAnyStateTransitions()
        {
            foreach (FSM_Transition transition in _anyStateTransitions)
            {
                if (transition.CheckConditions(this))
                {
                    return transition.toStateIndex;
                }
            }

            return -1;
        }

        private void ChangeState(int newStateIndex)
        {
            if (_states[_currentState].behaviour != null)
            {
                _states[_currentState].behaviour.OnExit();
            }

            _currentState = newStateIndex;

            if (_states[_currentState].behaviour != null)
            {
                _states[_currentState].behaviour.OnEnter();
            }
        }
        #endregion

        #region Collision Events
        private void OnCollisionEnter(Collision collision)
        {
            if (_states[_currentState].behaviour != null)
            {
                _states[_currentState].behaviour.OnCollisionEnter(collision);
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (_states[_currentState].behaviour != null)
            {
                _states[_currentState].behaviour.OnCollisionStay(collision);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (_states[_currentState].behaviour != null)
            {
                _states[_currentState].behaviour.OnCollisionExit(collision);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_states[_currentState].behaviour != null)
            {
                _states[_currentState].behaviour.OnCollisionEnter2D(collision);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (_states[_currentState].behaviour != null)
            {
                _states[_currentState].behaviour.OnCollisionStay2D(collision);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (_states[_currentState].behaviour != null)
            {
                _states[_currentState].behaviour.OnCollisionExit2D(collision);
            }
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        public int GetCurrentState()
        {
            return _currentState;
        }

        public void CheckAllBehaviourReferences()
        {
            print(1);
            foreach (FSM_State st in _states)
            {
                print(2);
                if (st.behaviour != null)
                {
                    print(3);
                    print(st.behaviour.GetType().ToString());
                }
            }
        }
#endif
        #endregion
    }
}
