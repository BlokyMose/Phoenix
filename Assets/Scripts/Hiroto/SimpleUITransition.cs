using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]

[System.Serializable]
public class SimpleUITransition : MonoBehaviour
{
    [SerializeField, Tooltip("この GameObject の PosX を設定してください。")]
    float PosX;
    [SerializeField, Tooltip("この GameObject の PosY を設定してください。")]
    float PosY;
    [SerializeField, Range(0.05f, 10f), Tooltip("IN、または OUT するまでの時間を設定します。")]
    float TotalTime = 0.3f;
    [SerializeField, Tooltip("フェードイン・アウトアニメーションを行います。")]
    bool AlphaFade = true;
    [SerializeField, Space(10), Tooltip("水平アニメーションを行います。")]
    bool HorizontalFade = false;
    [SerializeField, Range(-10f, 10f), Tooltip("水平アニメーションでどちらの方向からINするか、またはその移動量を指定します。通常は -1 or 1 が適切です。")]
    float HorizontalRatio = -1;
    [SerializeField, Tooltip("水平アニメーションの Ease パターンを設定します。")]
    EaseValue.eEase HorizontalEase = EaseValue.eEase.CubicOut;
    [SerializeField, Space(10), Tooltip("垂直アニメーションを行います。")]
    bool VerticalFade = false;
    [SerializeField, Range(-10f, 10f), Tooltip("垂直アニメーションでどちらの方向からINするか、またはその移動量を指定します。通常は -1 or 1 が適切です。")]
    float VerticalRatio = -1;
    [SerializeField, Tooltip("垂直アニメーションの Ease パターンを設定します。")]
    EaseValue.eEase VerticalEase = EaseValue.eEase.CubicOut;
    [SerializeField, Space(10), Range(0, 10f), Tooltip("表示アニメーションが始まるまでのディレイタイムを指定します。")]
    float DelayTimeBeforeShow;
    [SerializeField, Range(0, 10f), Tooltip("非表示アニメーションが始まるまでのディレイタイムを指定します。")]
    float DelayTimeBeforeHide;
    [SerializeField, Space(10), Tooltip("表示・非表示に合わせて自動的に SetActive() を実行します。")]
    bool AutoActivate = true;
    [SerializeField, Tooltip("表示・非表示に合わせて自動的に CanvasGroup の入力可否を設定します。")]
    bool AutoBlockRaycasts = true;

    [SerializeField, Header("Debug"), Range(0, 1), Tooltip("アニメーションの確認を行います。0 が非表示、1 が表示。")]
    float Value;


    [SerializeField]
    SimpleUITransition[] transitions;

    [SerializeField, Header("Event")]
    public UnityEvent OnFadein = null;
    [SerializeField]
    public UnityEvent OnFadeout = null;



    RectTransform rectTransform;
    CanvasGroup canvasGroup;

    Coroutine co_fadein;
    Coroutine co_fadeout;

    /// <summary>
    /// awake
    /// </summary>
    void Awake()
    {
        if (rectTransform != null)
        {
            return;
        }

        initCache();

        transitionUpdate(rectTransform, canvasGroup, Value);
    }

    /// <summary>
    /// start
    /// </summary>
    void Start()
    {
        foreach (var trans in transitions)
        {
            trans.Show();
        }

        Invoke(nameof(Hide), 3f);

        if (Value > 0 || co_fadein != null)
        {
            if (AutoActivate == true)
            {
                this.SetActive(true);
            }
            if (AutoBlockRaycasts == true)
            {
                canvasGroup.blocksRaycasts = true;
            }
        }
        else
        {
            if (AutoActivate == true)
            {
                this.SetActive(false);
            }
            if (AutoBlockRaycasts == true)
            {
                canvasGroup.blocksRaycasts = false;
            }
        }
    }

    /// <summary>
    /// アタッチする瞬間、RectTransform で設定された値を自動的に入れる
    /// </summary>
    void Reset()
    {
        PosX = rectTransform.GetX();
        PosY = rectTransform.GetY();
    }

    /// <summary>
    /// on validate
    /// </summary>
    void OnValidate()
    {
        initCache();

#if UNITY_EDITOR
        if (EditorApplication.isPlaying == false)
        {
            rectTransform.SetX(PosX);
            rectTransform.SetY(PosY);
        }
#endif
        transitionUpdate(rectTransform, canvasGroup, Value);
    }

    /// <summary>
    /// Value を強制的に変更
    /// </summary>
    /// <param name="value">0:hide～1:show</param>
    public void SetValue(float value)
    {
        if (Value == value)
        {
            return;
        }
        if (Value == 1 || co_fadein != null)
        {
            OnFadein?.Invoke();
        }
        if (Value == 0 || co_fadeout != null)
        {
            OnFadeout?.Invoke();
        }
        stopCoroutine();

        Value = value;
        transitionUpdate(rectTransform, canvasGroup, Value);
    }

    /// <summary>
    /// 表示
    /// </summary>
    public void Show()
    {
        if (AutoActivate == true)
        {
            this.SetActive(true);
        }
        if (AutoBlockRaycasts == true)
        {
            // まだ許可は出さない
            canvasGroup.blocksRaycasts = false;
        }

        stopCoroutine();
        co_fadein = StartCoroutine(fadein());
    }

    /// <summary>
    /// 非表示
    /// </summary>
    public void Hide()
    {
        if (AutoBlockRaycasts == true)
        {
            canvasGroup.blocksRaycasts = false;
        }

        stopCoroutine();
        co_fadeout = StartCoroutine(fadeout());
    }

    /// <summary>
    /// cache entry
    /// </summary>
    void initCache()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    /// <summary>
    /// in out 両方のコルーチン停止
    /// </summary>
    void stopCoroutine()
    {
        if (co_fadein != null)
        {
            StopCoroutine(co_fadein);
        }
        if (co_fadeout != null)
        {
            StopCoroutine(co_fadeout);
        }
    }

    /// <summary>
    /// fadein
    /// </summary>
    IEnumerator fadein()
    {
        yield return new WaitForSeconds(DelayTimeBeforeShow);

        float time = Time.time;
        float startVal = Value;

        while (true)
        {
            float value = Mathf.Clamp01((Time.time - time) / TotalTime);
            Value = Mathf.Clamp01(startVal + (1 - startVal) * value);

            transitionUpdate(rectTransform, canvasGroup, Value);

            if (AutoBlockRaycasts == true)
            {
                // 完全表示より少し前にレイキャストはONにしておく（ユーザビリティを考えて）
                if (value >= 0.75f)
                {
                    canvasGroup.blocksRaycasts = true;
                }
            }

            if (value >= 1)
            {
                break;
            }
            yield return null;
        }

        OnFadein?.Invoke();
    }

    /// <summary>
    /// fadeout
    /// </summary>
    IEnumerator fadeout()
    {
        yield return new WaitForSeconds(DelayTimeBeforeHide);

        float time = Time.time;
        float startVal = Value;

        while (true)
        {
            float value = Mathf.Clamp01((Time.time - time) / TotalTime);
            Value = Mathf.Clamp01(startVal + (0 - startVal) * value);

            transitionUpdate(rectTransform, canvasGroup, Value);

            if (value >= 1)
            {
                break;
            }
            yield return null;
        }

        if (AutoActivate == true)
        {
            this.SetActive(false);
        }

        OnFadeout?.Invoke();
    }

    /// <summary>
    /// 状態更新
    /// </summary>
    void transitionUpdate(RectTransform rectTrans, CanvasGroup group, float value)
    {
        if (AlphaFade == true)
        {
            group.alpha = EaseValue.Get(value, 1);
        }
        if (VerticalFade == true)
        {
            rectSetY(rectTrans, EaseValue.Get(value, 1, PosY + rectGetHeight(rectTrans) * VerticalRatio, PosY, VerticalEase));
        }
        if (HorizontalFade == true)
        {
            rectSetX(rectTrans, EaseValue.Get(value, 1, PosX + rectGetWidth(rectTrans) * HorizontalRatio, PosX, HorizontalEase));
        }
    }

    /// <summary>
    /// rect control
    /// </summary>
    float rectGetWidth(RectTransform rect)
    {
        return rect.sizeDelta.x;
    }
    float rectGetHeight(RectTransform rect)
    {
        return rect.sizeDelta.y;
    }
    void rectSetX(RectTransform rect, float x)
    {
        Vector3 trans = rect.gameObject.transform.localPosition;
        trans.x = x;
        rect.gameObject.transform.localPosition = trans;
    }
    void rectSetY(RectTransform rect, float y)
    {
        Vector3 trans = rect.gameObject.transform.localPosition;
        trans.y = y;
        rect.gameObject.transform.localPosition = trans;
    }
}

