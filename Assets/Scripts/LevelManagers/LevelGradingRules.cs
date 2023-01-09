using Encore.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Phoenix.HealthController;

namespace Phoenix
{
    [CreateAssetMenu(menuName = "SO/Level/Grade Rules")]
    public class LevelGradingRules : ScriptableObject
    {
        [Serializable]
        public class Score
        {
            public int killCount;
            public float damaged;
            public int timeRemaining;
            public int timeElapsed;

            public Score(int killCount, float damaged, int timeRemaining, int timeElapsed)
            {
                this.killCount = killCount;
                this.damaged = damaged;
                this.timeRemaining = timeRemaining;
                this.timeElapsed = timeElapsed;
            }

            public Score() { }
        }

        [Serializable]
        public class ScoringRules
        {
            [SerializeField]
            public int killPoint = 10;
            public int KillPoint => killPoint;

            [SerializeField]
            public int damagedPoint = -10;
            public int DamagedPoint => damagedPoint;

            [SerializeField]
            public int timeRemainingPoint = 10;
            public int TimeRemainingPoint => timeRemainingPoint;

            public int Evaluate(Score score)
            {
                return
                    score.killCount * killPoint +
                    (int)score.damaged * damagedPoint +
                    score.timeRemaining * timeRemainingPoint;
            }
        }

        [Serializable]
        public class Grade
        {
            [SerializeField]
            string gradeName;
            public string GradeName => gradeName;

            [SerializeField]
            int atPoint;
            public int AtPoint => atPoint;

            [SerializeField, PreviewField]
            Sprite seal;
            public Sprite Seal => seal;

        }

        [SerializeField]
        ScoringRules scoringRules;
        public ScoringRules LevelScoringRules => scoringRules;

        [SerializeField]
        List<Grade> grades = new();
        public List<Grade> Grades => grades;

        [Button]
        void ArrangeRulesFromHighest()
        {
            var newList = new List<Grade>();
            for (int i = grades.Count - 1; i >= 0; i--)
            {
                var top = grades[0];
                var topIndex = 0;
                for (int k = 0; k < grades.Count; k++)
                {
                    if (grades[k].AtPoint >= top.AtPoint)
                    {
                        top = grades[k];
                        topIndex = k;
                    }
                }

                grades.RemoveAt(topIndex);
                newList.Add(top);
            }
            grades = newList;
        }


        public Grade Evalutate(int totalPoint)
        {
            foreach (var rule in grades)
                if (totalPoint >= rule.AtPoint)
                    return rule;

            return grades.GetLast();
        }
    }
}
