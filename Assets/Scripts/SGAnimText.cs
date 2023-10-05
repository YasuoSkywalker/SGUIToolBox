using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//TODO:字体放大效果改为使用textInfo.characterInfo[i].pointSize修改
//TODO:添加更多的效果
[RequireComponent(typeof(TextMeshProUGUI))]
public class SGAnimText : MonoBehaviour
{
    //TextMeshProUGUI组件
    private TextMeshProUGUI tmpUGUI;
    //TextMeshProUGUI的文本
    private string tmpUGUIText;

    #region 打字效果
    [SerializeField]
    [Header("使用打字效果")]
    [Tooltip("勾选后，将会以打字的效果逐个展示字符")]
    private bool useTypeEffect = false;
    [SerializeField]
    [Header("是否对全部文本生效")]
    private bool applyTypeToAllText = false;
    [SerializeField]
    [Header("打字效果生效的起始位置")]
    private int typeStartIndex = 0;
    [SerializeField]
    [Header("打字效果生效的结束位置")]
    private int typeEndIndex = 0;
    [SerializeField]
    [Header("打字间隔")]
    private float typeSpace = 0.2f;
    #endregion

    #region 放大效果
    [SerializeField]
    [Header("使用放大效果")]
    [Tooltip("勾选后，会将设定范围内的字体放大")]
    private bool useBiggerEffect = false;
    [SerializeField]
    [Header("是否对全部文本生效")]
    private bool applyBiggerToAllText = false;
    [SerializeField]
    [Header("打字效果生效的起始位置")]
    private int biggerStartIndex = 0;
    [SerializeField]
    [Header("打字效果生效的结束位置")]
    private int biggerEndIndex = 0;
    [SerializeField]
    [Header("放大倍率")]
    [Min(0f)]
    private float biggerRate = 1.2f;
    #endregion

    private void Awake()
    {
        tmpUGUI = GetComponent<TextMeshProUGUI>();
        tmpUGUIText = tmpUGUI.text;
        tmpUGUI.ForceMeshUpdate();
        HandleTypeEffect();
        HandleBiggerEffect();
    }

    /// <summary>
    /// 处理打字效果
    /// </summary>
    private void HandleTypeEffect()
    {
        if (!useTypeEffect) return;
        //如果勾选了对所有文字生效
        if(applyTypeToAllText)
        {
            //先清空TMP的text
            tmpUGUI.text = "";
            //将整段文字传入处理打字效果的协程函数中
            StartCoroutine(TypeEffectCoroutine(tmpUGUIText));
        }
        else
        {
            tmpUGUI.text = "";
            //数据正确性检查
            if(typeStartIndex >= tmpUGUIText.Length || typeStartIndex < 0)
            {
                Debug.LogError("typeStartIndex Out Of Range!");
                return;
            }
            if(typeEndIndex >= tmpUGUIText.Length || typeEndIndex < 0)
            {
                Debug.LogWarning("typeEndIndex Out Of Range!");
                typeEndIndex = tmpUGUIText.Length - 1;
            }
            if (typeEndIndex < typeStartIndex)
            {
                Debug.LogError("typeEndIndex must larger than typeStartIndex");
                return;
            }
            //打字效果之前部分
            tmpUGUI.text += tmpUGUIText.Substring(0, typeStartIndex);
            //打字效果，onFinish传入打字效果之后部分
            StartCoroutine(TypeEffectCoroutine(tmpUGUIText.Substring(typeStartIndex, typeEndIndex - typeStartIndex + 1),
                ()=> {
                    if(typeEndIndex+1 < tmpUGUIText.Length)
                        tmpUGUI.text += tmpUGUIText.Substring(typeEndIndex + 1, tmpUGUIText.Length - typeEndIndex-1);
                }));
        }
    }

    /// <summary>
    /// 处理打字效果协程
    /// </summary>
    /// <param name="text"></param>
    /// <param name="onFinish"></param>
    /// <returns></returns>
    IEnumerator TypeEffectCoroutine(string text,Action onFinish = null)
    {
        int index = 0;
        //逐个输出文字
        while(index < text.Length)
        {
            tmpUGUI.text += text[index++];
            yield return new WaitForSeconds(typeSpace);
        }
        //打字效果结束后，调用onFinish Action，记得判空
        onFinish?.Invoke();
    }

    /// <summary>
    /// 处理放大效果
    /// </summary>
    private void HandleBiggerEffect()
    {
        if (!useBiggerEffect) return;
        if(applyBiggerToAllText)
        {
            tmpUGUI.fontSize = tmpUGUI.fontSize * biggerRate;
        }
        else
        {
            //数据正确性检查
            if (biggerStartIndex >= tmpUGUIText.Length || biggerStartIndex < 0)
            {
                Debug.LogError("biggerStartIndex Out Of Range!");
                return;
            }
            if (biggerEndIndex >= tmpUGUIText.Length || biggerEndIndex < 0)
            {
                Debug.LogWarning("biggerEndIndex Out Of Range!");
                typeEndIndex = tmpUGUIText.Length - 1;
            }
            if (biggerEndIndex < biggerStartIndex)
            {
                Debug.LogError("biggerEndIndex must larger than biggerStartIndex");
                return;
            }
            /*for(int i=biggerStartIndex;i<=biggerEndIndex;i++)
            {
                tmpUGUI.textInfo.characterInfo[i].pointSize = tmpUGUI.textInfo.characterInfo[i].pointSize*biggerRate;
                tmpUGUI.ForceMeshUpdate();
            }*/
            //根据规则生成带有富文本的字符串
            float size = tmpUGUI.fontSize * biggerRate;
            string effectText = "";
            for(int i=0;i<tmpUGUIText.Length;i++)
            {
                if(i == biggerStartIndex)
                {
                    effectText += ("<size=" + size + ">");
                }
                effectText += tmpUGUIText[i];
                if (i == biggerEndIndex)
                {
                    effectText += "</size>";
                }
            }
            //应用字符串
            tmpUGUIText = effectText;
            tmpUGUI.text = effectText;
        }
    }
}
