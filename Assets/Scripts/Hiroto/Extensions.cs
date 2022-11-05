using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// game object extensions
/// </summary>
public static class ComponentExtensions
{
    /// <summary>
    /// コンポーネントを返す。なければ作る
    /// </summary>
    public static void SafeGetComponent<T>(this Component self, out T component) where T : Component
    {
        component = self.GetComponent<T>();
        if (component == null)
        {
            component = self.gameObject.AddComponent<T>();
        }
    }

    /// <summary>
    /// コンポーネントを取得
    /// </summary>
    public static bool CheckGetComponent<T>(this Component self, out T component) where T : Component
    {
        component = self.GetComponent<T>();
        if (component == null)
        {
            Debug.LogError("Missing Component");
            return false;
        }
        return true;
    }

    /// <summary>
    /// コンポーネントを取得
    /// </summary>
    public static bool CheckGetComponentInChildren<T>(this Component self, out T component, bool includeInactive = true) where T : Component
    {
        component = self.GetComponentInChildren<T>(includeInactive);
        if (component == null)
        {
            Debug.LogError("Missing Component");
            return false;
        }
        return true;
    }

    /// <summary>
    /// コンポーネントに紐づく GameObject を破棄
    /// </summary>
    public static void Destroy(this Component self)
    {
        Object.Destroy(self.gameObject);
    }

    /// <summary>
    /// コンポーネントに紐づく GameObject を即時破棄
    /// </summary>
    public static void DestroyImmediate(this Component self)
    {
        Object.DestroyImmediate(self.gameObject);
    }

    /// <summary>
    /// コンポーネントに紐づく GameObject のアクティベート
    /// </summary>
    public static void SetActive(this Component self, bool value)
    {
        self.gameObject.SetActive(value);
    }

    /// <summary>
    /// 座標を取得します
    /// </summary>
    public static float GetX(this Component self)
    {
        return self.gameObject.transform.localPosition.x;
    }

    /// <summary>
    /// 座標を取得します
    /// </summary>
    public static float GetY(this Component self)
    {
        return self.gameObject.transform.localPosition.y;
    }

    /// <summary>
    /// 座標を設定します
    /// </summary>
    public static void SetX(this Component self, float x)
    {
        Vector3 trans = self.gameObject.transform.localPosition;
        trans.x = x;
        self.gameObject.transform.localPosition = trans;
    }

    /// <summary>
    /// 座標を設定します
    /// </summary>
    public static void SetY(this Component self, float y)
    {
        Vector3 trans = self.gameObject.transform.localPosition;
        trans.y = y;
        self.gameObject.transform.localPosition = trans;
    }

    /// <summary>
    /// 座標を設定します
    /// </summary>
    public static void SetXY(this Component self, float x, float y)
    {
        Vector3 trans = self.gameObject.transform.localPosition;
        trans.x = x;
        trans.y = y;
        self.gameObject.transform.localPosition = trans;
    }

    /// <summary>
    /// 回転を設定します
    /// </summary>
    public static void SetRotateXY(this Component self, float x, float y)
    {
        Vector3 trans = self.gameObject.transform.localEulerAngles;
        trans.x = x;
        trans.y = y;
        self.gameObject.transform.localEulerAngles = trans;
    }

    /// <summary>
    /// 回転を設定します
    /// </summary>
    public static void SetRotateX(this Component self, float x)
    {
        Vector3 trans = self.gameObject.transform.localEulerAngles;
        trans.x = x;
        self.gameObject.transform.localEulerAngles = trans;
    }

    /// <summary>
    /// 回転を設定します
    /// </summary>
    public static void SetRotateY(this Component self, float y)
    {
        Vector3 trans = self.gameObject.transform.localEulerAngles;
        trans.y = y;
        self.gameObject.transform.localEulerAngles = trans;
    }

    /// <summary>
    /// 回転を設定します
    /// </summary>
    public static void SetRotateZ(this Component self, float z)
    {
        Vector3 trans = self.gameObject.transform.localEulerAngles;
        trans.z = z;
        self.gameObject.transform.localEulerAngles = trans;
    }

    /// <summary>
    /// スケールを設定します
    /// </summary>
    public static void SetScaleXY(this Component self, float x, float y)
    {
        Vector3 trans = self.gameObject.transform.localScale;
        trans.x = x;
        trans.y = y;
        self.gameObject.transform.localScale = trans;
    }
}

/// <summary>
/// image extensions
/// </summary>
public static class ImageExtensions
{
    /// <summary>
    /// サイズを設定します
    /// </summary>
    public static void SetNativeSize(this Image self)
    {
        if (self.sprite != null)
        {
            SetSize(self, self.sprite.rect.width, self.sprite.rect.height);
        }
        else
        {
            SetSize(self, 100, 100);
        }
    }

    /// <summary>
    /// サイズを設定します
    /// </summary>
    public static void SetSize(this Image self, float width, float height)
    {
        self.rectTransform.SetSize(width, height);
    }
    

}

/// <summary>
/// RectTransform 型の拡張メソッドを管理するクラス
/// </summary>
public static class RectTransformExtensions
{
    /// <summary>
    /// 幅を返します
    /// </summary>
    public static float GetWidth(this RectTransform self)
    {
        return self.sizeDelta.x;
    }
    
    /// <summary>
    /// 高さを返します
    /// </summary>
    public static float GetHeight(this RectTransform self)
    {
        return self.sizeDelta.y;
    }

    /// <summary>
    /// 幅を設定します
    /// </summary>
    public static void SetWidth(this RectTransform self, float width)
    {
        var size = self.sizeDelta;
        size.x = width;
        self.sizeDelta = size;
    }
    
    /// <summary>
    /// 高さを設定します
    /// </summary>
    public static void SetHeight(this RectTransform self, float height)
    {
        var size = self.sizeDelta;
        size.y = height;
        self.sizeDelta = size;
    }
    
    /// <summary>
    /// サイズを取得します
    /// </summary>
    public static Vector2 GetSize(this RectTransform self)
    {
        return self.sizeDelta;
    }
    
    /// <summary>
    /// サイズを設定します
    /// </summary>
    public static void SetSize(this RectTransform self, float width, float height)
    {
        self.sizeDelta = new Vector2(width, height);
    }
}

