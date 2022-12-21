using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public static class GameplayUtilityClass
    {
        [Serializable]
        public class AnimatorParameter
        {
            public enum DataType { Float, Int, Bool, Trigger }

            [SerializeField, HorizontalGroup("1")]
            protected string paramName;
            public string ParamName => paramName;

            [SerializeField, HorizontalGroup("2"), LabelWidth(0.1f)]
            DataType dataType;
            public DataType Type => dataType;

            [SerializeField, HorizontalGroup("2"), LabelWidth(0.1f), SuffixLabel("value", true), ShowIf("@" + nameof(dataType) + "==" + nameof(DataType) + "." + nameof(DataType.Int))]
            protected int intValue;
            public int IntValue => intValue;

            [SerializeField, HorizontalGroup("2"), LabelWidth(0.1f), SuffixLabel("value", true), ShowIf("@" + nameof(dataType) + "==" + nameof(DataType) + "." + nameof(DataType.Float))]
            protected float floatValue;
            public float FloatValue => floatValue;

            [SerializeField, HorizontalGroup("2"), LabelWidth(0.1f), SuffixLabel("value", true), ShowIf("@" + nameof(dataType) + "==" + nameof(DataType) + "." + nameof(DataType.Bool))]
            protected bool boolValue;
            public bool BoolValue => boolValue;

            protected int hash;

            public void Init()
            {
                hash = Animator.StringToHash(paramName);
            }

            public AnimatorParameter(string paramName, int intValue)
            {
                this.paramName = paramName;
                this.dataType = DataType.Int;
                this.intValue = intValue;
            }

            public AnimatorParameter(string paramName, float floatValue)
            {
                this.paramName = paramName;
                this.dataType = DataType.Float;
                this.floatValue = floatValue;
            }

            public AnimatorParameter(string paramName, bool boolValue)
            {
                this.paramName = paramName;
                this.dataType = DataType.Bool;
                this.boolValue = boolValue;
            }

            public AnimatorParameter(string paramName)
            {
                this.paramName = paramName;
                this.dataType = DataType.Trigger;
            }

            public void SetParam(Animator animator)
            {
                switch (dataType)
                {
                    case DataType.Float:
                        animator.SetFloat(hash, floatValue);
                        break;
                    case DataType.Int:
                        animator.SetInteger(hash, intValue);
                        break;
                    case DataType.Bool:
                        animator.SetBool(hash, boolValue);
                        break;
                    case DataType.Trigger:
                        animator.SetTrigger(hash);
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
