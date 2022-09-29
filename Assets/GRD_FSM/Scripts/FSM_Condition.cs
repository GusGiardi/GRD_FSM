using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRD.FSM
{
    [System.Serializable]
    public class FSM_Condition
    {
        [SerializeField] int _parameterIndex;

        public enum ConditionOperator
        {
            IsTrue,
            IsNotTrue,
            Equals,
            NotEqual,
            Less,
            Greater,
            LessOrEqual,
            GreaterOrEqual
        }

        public int parameterIndex
        {
            get => _parameterIndex;
#if UNITY_EDITOR
            set => _parameterIndex = value;
#endif
        }

#if UNITY_EDITOR
        public static readonly string[] booleanConditionLabels =
        {
        "Is true",
        "Is not true"
    };

        public static readonly string[] numericConditionLabels =
        {
        "Equals to",
        "Not equal",
        "Less than",
        "Greater than",
        "Less or equal than",
        "Greater or equal than"
    };
#endif

        [SerializeField] ConditionOperator _conditionOperator;
        [SerializeField] float _referenceValue;

        public FSM_Condition(FSM_Parameter.ParameterType parameterType)
        {
            _parameterIndex = 0;
            _referenceValue = 0;
            switch (parameterType)
            {
                case FSM_Parameter.ParameterType.Float:
                case FSM_Parameter.ParameterType.Integer:
                    _conditionOperator = ConditionOperator.Equals;
                    break;
                case FSM_Parameter.ParameterType.Boolean:
                case FSM_Parameter.ParameterType.Trigger:
                    _conditionOperator = ConditionOperator.IsTrue;
                    break;
            }
        }

        public bool CheckCondition(FSM_Manager manager)
        {
            object parameterValue = manager.GetParameterValue(_parameterIndex);

            if (parameterValue == null)
                return false;

            switch (_conditionOperator)
            {
                case ConditionOperator.IsTrue:
                    return (bool)parameterValue;
                case ConditionOperator.IsNotTrue:
                    return !(bool)parameterValue;
                case ConditionOperator.Equals:
                    return (float)parameterValue == _referenceValue;
                case ConditionOperator.NotEqual:
                    return (float)parameterValue != _referenceValue;
                case ConditionOperator.Less:
                    return (float)parameterValue < _referenceValue;
                case ConditionOperator.Greater:
                    return (float)parameterValue > _referenceValue;
                case ConditionOperator.LessOrEqual:
                    return (float)parameterValue <= _referenceValue;
                case ConditionOperator.GreaterOrEqual:
                    return (float)parameterValue >= _referenceValue;
            }

            return false;
        }
    }
}
