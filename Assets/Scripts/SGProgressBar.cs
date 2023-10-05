using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SGProgressBar : MonoBehaviour
{
    [Header("数值形式")]
    public ProgressValueType valueType;

    public enum ProgressValueType
    {
        Integer,
        Float,
        Rate
    }

    [Header("最小值")]
    public float minValue = 0;
    [Header("最大值")]
    public float maxValue = 100;
    [Header("当前值")]
    public float currentValue = 50;

    [Header("进度文本")]
    public TMPro.TextMeshProUGUI progressText;

    [Header("进度条图片")]
    public GameObject progressImage;

    [Header("进度条变化速度")]
    [Min(0.1f)]
    public float changeTime = 0.5f;

    [Tooltip("是否为不同的进度作颜色区分,进度值高于分界线后会设置为对应的颜色")]
    [Header("是否启用颜色区分")]
    public bool enableDistinguish = false;
    [Header("分界线1")]
    public float startValue1 = 0.0f;
    public Color startColor1 = Color.red;
    [Header("分界线2")]
    public float startValue2 = 45.0f;
    public Color startColor2 = Color.yellow;
    [Header("分界线3")]
    public float startValue3 = 90.0f;
    public Color startColor3 = Color.green;

    private void Awake()
    {
        UpDateProgressBar();
    }

    /// <summary>
    /// 将value约束到[minValue,maxValue]之中
    /// </summary>
    void ClampValue(ref float value)
    {
        if (value > maxValue)
            value = maxValue;
        else if (value < minValue)
            value = minValue;
    }

    /// <summary>
    /// 设置进度条的当前值
    /// </summary>
    /// <param name="value"></param>
    public void SetCurrentValue(float targetValue)
    {
        ClampValue(ref targetValue);
        StopAllCoroutines();
        StartCoroutine(ValueChangeIEnumerator(targetValue));
    }

    void UpDateProgressBar()
    {
        //约束currentValue
        ClampValue(ref currentValue);
        //计算进度条的长度百分比
        float temp = (currentValue - minValue) / (maxValue - minValue);
        //修改进度条长度
        progressImage.transform.localScale = new Vector3(temp, 1, 1);
        //应用颜色
        if(enableDistinguish)
        {
            if (currentValue >= startValue3)
            {
                progressImage.GetComponent<UnityEngine.UI.Image>().color = startColor3;
            }
            else if (currentValue >= startValue2)
            {
                progressImage.GetComponent<UnityEngine.UI.Image>().color = startColor2;
            }
            else if (currentValue >= startValue1)
            {
                progressImage.GetComponent<UnityEngine.UI.Image>().color = startColor1;
            }
        }
        //修改文本
        switch (valueType)
        {
            case ProgressValueType.Integer:
                progressText.text = ((int)currentValue).ToString();
                break;
            case ProgressValueType.Float:
                progressText.text = currentValue.ToString("F2");
                break;
            case ProgressValueType.Rate:
                progressText.text = ((temp * 100f).ToString("F2") + "%");
                break;
            default:
                break;
        }
    }

    IEnumerator ValueChangeIEnumerator(float targetValue)
    {
        while(currentValue != targetValue)
        {
            currentValue = Mathf.MoveTowards(currentValue,targetValue,changeTime);
            UpDateProgressBar();
            yield return new WaitForFixedUpdate();
        }
    }
}

