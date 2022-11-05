using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    [SerializeField]
    GameObject       parent = null;
    [SerializeField]
    TextMeshProUGUI  easename = null;
    [SerializeField]
    Image            circle = null;
    [SerializeField]
    Slider           slider = null;
    [SerializeField]
    TextMeshProUGUI  sliderValue = null;

    class EaseGroup
    {
        public string Name;
        public EaseValue.eEase  Type;
        public Image Point;
        public EaseGroup(string name, EaseValue.eEase method)
        {
            Name   = name;
            Type = method;
        }
    }

    List<EaseGroup> list;

    // Start is called before the first frame update
    void Start()
    {
        list = new List<EaseGroup>()
        {
            new EaseGroup("linear", EaseValue.eEase.Linear),
            new EaseGroup("sin_in", EaseValue.eEase.SinusoidaiIn),
            new EaseGroup("circ_in", EaseValue.eEase.CircularIn),
            new EaseGroup("quad_in(2x)", EaseValue.eEase.QuadraticIn),
            new EaseGroup("cubic_in(3x)", EaseValue.eEase.CubicIn),
            new EaseGroup("quarantic_in(4x)", EaseValue.eEase.QuaranticIn),
            new EaseGroup("quintic_in(5x)", EaseValue.eEase.QuinticIn),
            new EaseGroup("exp_in", EaseValue.eEase.ExponentialIn),
            new EaseGroup("elastic_in", EaseValue.eEase.ElasticIn),
            new EaseGroup("bounce_in", EaseValue.eEase.BounceIn),
            new EaseGroup("back_in", EaseValue.eEase.BackIn),

            new EaseGroup("sin_out", EaseValue.eEase.SinusoidaiOut),
            new EaseGroup("circ_out", EaseValue.eEase.CircularOut),
            new EaseGroup("quad_out(2x)", EaseValue.eEase.QuadraticOut),
            new EaseGroup("cubic_out(3x)", EaseValue.eEase.CubicOut),
            new EaseGroup("quarantic_out(4x)", EaseValue.eEase.QuaranticOut),
            new EaseGroup("quintic_out(5x)", EaseValue.eEase.QuinticOut),
            new EaseGroup("exp_out", EaseValue.eEase.ExponentialOut),
            new EaseGroup("elastic_out", EaseValue.eEase.ElasticOut),
            new EaseGroup("bounce_out", EaseValue.eEase.BounceOut),
            new EaseGroup("back_out", EaseValue.eEase.BackOut),

            new EaseGroup("sin_io", EaseValue.eEase.SinusoidaiInout),
            new EaseGroup("circ_io", EaseValue.eEase.CircularInout),
            new EaseGroup("quad_io(2x)", EaseValue.eEase.QuadraticInout),
            new EaseGroup("cubic_io(3x)", EaseValue.eEase.CubicInout),
            new EaseGroup("quarantic_io(4x)", EaseValue.eEase.QuaranticInout),
            new EaseGroup("quintic_io(5x)", EaseValue.eEase.QuinticInout),
            new EaseGroup("exp_io", EaseValue.eEase.ExponentialInout),
            new EaseGroup("elastic_io", EaseValue.eEase.ElasticInout),
            new EaseGroup("bounce_io", EaseValue.eEase.BounceInout),
            new EaseGroup("back_io", EaseValue.eEase.BackInout),
        };

        for (int no = 0; no < list.Count; no++)
        {
            EaseGroup group = list[no];
            float     y     = 510 - 34 * no;

            // Ease Type
            TextMeshProUGUI txt = Instantiate(easename, parent.transform);
            txt.SetXY(-850, y);
            txt.SetText(group.Name);

            // スライダーで移動するマーカー
            group.Point = Instantiate(circle, parent.transform);
            group.Point.SetXY(EaseValue.Get(0, 1, -450, 600, group.Type), y);
            if (group.Name.IndexOf("_out") >= 0)
            {
                group.Point.color = new Color(0.5f, 1, 0.5f, 1);
                txt.color = new Color(0.5f, 0.5f, 0.5f, 1);
            }
            else
            if (group.Name.IndexOf("_io") >= 0)
            {
                group.Point.color = new Color(0.5f, 0.5f, 1, 1);
                txt.color = new Color(1, 1, 1, 1);
            }
            else
            {
                group.Point.color = new Color(1, 0.5f, 0.5f, 1);
                txt.color = new Color(1, 1, 1, 1);
            }
            group.Point.SetScaleXY(1.6f, 1.6f);

            // 軌跡ポイント
            for (int i = 0; i <= 20; i++)
            {
                Image img = Instantiate(circle, parent.transform);
                float x = EaseValue.Get((float)i / 20, 1, -450, 600, group.Type);
                img.color = new Color(0.8f, 0.8f, 0.8f, 0.1f);
                img.SetXY(x, y);
//                Debug.Log(x);
            }
        }

        slider.onValueChanged.AddListener(onChanged);
    }

    void onChanged(float value)
    {
        value = Mathf.Clamp01(value);

        sliderValue?.SetText(value.ToString());

        for (int no = 0; no < list.Count; no++)
        {
            EaseGroup group = list[no];
            group.Point.SetX(EaseValue.Get(value, 1, -450, 600, group.Type));
        }
    }
}
