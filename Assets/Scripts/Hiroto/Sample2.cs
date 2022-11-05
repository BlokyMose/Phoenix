using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample2 : MonoBehaviour
{
    [SerializeField]
    SimpleUITransition[]    transitions;

    void Awake()
    {
        // 開始時に全部消す
        foreach (var trans in transitions)
        {
            trans.SetValue(0);
        }
    }

    public void OnClickShow()
    {
        foreach (var trans in transitions)
        {
            trans.Show();
        }
    }

    public void OnClickHide()
    {
        foreach (var trans in transitions)
        {
            trans.Hide();
        }
    }
}
