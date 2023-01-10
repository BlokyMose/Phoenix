using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix
{
    public class ElementContainerSR : ElementContainer
    {
        [Serializable]
        public class SRChanger
        {
            public enum MatchElementColorMode { Color, ColorBright, ColorDim, Off, Custom }

            [Serializable]
            public class SRCustomization
            {
                [VerticalGroup("1/1"), LabelWidth(0.1f), SerializeField]
                Element element;
                public Element Element => element;

                [VerticalGroup("1/1"), LabelText("Match"), SerializeField]
                MatchElementColorMode matchMode = MatchElementColorMode.Off;
                public MatchElementColorMode MatchMode => matchMode;

                [VerticalGroup("1/1"), LabelWidth(0.1f), SerializeField, ShowIf("@"+nameof(matchMode)+"=="+nameof(MatchElementColorMode)+"."+nameof(MatchElementColorMode.Custom))]
                Color color;
                public Color Color => color;


                [HorizontalGroup("1"), LabelWidth(0.1f), PreviewField, SerializeField]
                Sprite sprite;
                public Sprite Sprite => sprite;
            }

            [HorizontalGroup("1"),LabelWidth(0.1f), SerializeField]
            SpriteRenderer sr;
            public SpriteRenderer SR => sr;

            [HorizontalGroup("1"), LabelWidth(0.1f), SerializeField]
            MatchElementColorMode matchColor;
            public MatchElementColorMode MatchColor => matchColor;

            [SerializeField]
            List<SRCustomization> srCustomizations = new();
            public List<SRCustomization> SRCustomizations => srCustomizations;

            public void Evaluate(Element element)
            {
                foreach (var sr in srCustomizations)
                {
                    if(sr.Element == element)
                    {
                        this.sr.sprite = sr.Sprite;
                        break;
                    }
                }

                switch (matchColor)
                {
                    case MatchElementColorMode.Color:
                        sr.color = element.Color;
                        break;
                    case MatchElementColorMode.ColorBright:
                        sr.color = element.ColorBright;
                        break;
                    case MatchElementColorMode.ColorDim:
                        sr.color = element.ColorDim;
                        break;
                }
            }
        }

        [SerializeField]
        List<SRChanger> srs = new();

        protected override void Awake()
        {
            base.Awake();
            OnNewElement += EvaluateSRs;
        }

        void EvaluateSRs(Element element)
        {
            foreach (var changer in srs)
                changer.Evaluate(element);
        }
    }
}
