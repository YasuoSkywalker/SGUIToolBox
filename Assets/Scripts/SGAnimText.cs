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
    private bool isExecuted = false;

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
    [Header("放大效果生效的起始位置")]
    private int biggerStartIndex = 0;
    [SerializeField]
    [Header("放大效果生效的结束位置")]
    private int biggerEndIndex = 0;
    [SerializeField]
    [Header("放大倍率")]
    [Min(0f)]
    private float biggerRate = 1.2f;
    #endregion

    #region 绕中心点摆动效果
    [SerializeField]
    [Header("使用旋转效果")]
    [Tooltip("勾选后，会将设定范围内的字符绕字符中心点摆动")]
    private bool useRotateEffect = false;
    [SerializeField]
    [Header("是否对全部文本生效")]
    private bool applyRotateToAllText = false;
    [SerializeField]
    [Header("摆动效果生效的起始位置")]
    private int rotateStartIndex = 0;
    [SerializeField]
    [Header("摆动效果生效的结束位置")]
    private int rotateEndIndex = 0;
    [SerializeField]
    [Header("摆动角度")]
    [Range(0f,360f)]
    private float rotationAngle = 0f;
    [SerializeField]
    [Header("摆动速度")]
    [Range(0f, 1f)]
    private float rotationSpeed = 1f;
    #endregion

    private void Awake()
    {
        tmpUGUI = GetComponent<TextMeshProUGUI>();
        tmpUGUIText = tmpUGUI.text;
        tmpUGUI.ForceMeshUpdate();
    }

    private void Update()
    {
        if (!isExecuted)
        {
            HandleTypeEffect();
            HandleBiggerEffect();
            HandleRotateEffect();
            isExecuted = true;
        }
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
                biggerEndIndex = tmpUGUIText.Length - 1;
            }
            if (biggerEndIndex < biggerStartIndex)
            {
                Debug.LogError("biggerEndIndex must larger than biggerStartIndex");
                return;
            }
            /*tmpUGUI.ForceMeshUpdate();
            for (int i = biggerStartIndex; i <= biggerEndIndex; i++)
            {
                tmpUGUI.textInfo.characterInfo[i].pointSize = tmpUGUI.textInfo.characterInfo[i].pointSize * biggerRate;
            }
            string originalText = tmpUGUI.text;
            tmpUGUI.text = "";
            tmpUGUI.text = originalText;
            Debug.Log(tmpUGUI.textInfo.characterInfo[0].pointSize);
            tmpUGUI.ForceMeshUpdate();
            Debug.Log(tmpUGUI.textInfo.characterInfo[0].pointSize);*/
            //根据规则生成带有富文本的字符串
            float size = tmpUGUI.fontSize * biggerRate;
            string effectText = "";
            for (int i = 0; i < tmpUGUIText.Length; i++)
            {
                if (i == biggerStartIndex)
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
            Debug.Log(tmpUGUI.textInfo.characterInfo[0].pointSize);
        }
    }

    /// <summary>
    /// 处理旋转效果
    /// </summary>
    private void HandleRotateEffect()
    {
        if (!useRotateEffect) return;
        if(applyRotateToAllText)
        {
            StartCoroutine(RotateEffectCoroutine(0, tmpUGUIText.Length - 1));
        }
        else
        {
            //数据正确性检查
            if (rotateStartIndex >= tmpUGUIText.Length || rotateStartIndex < 0)
            {
                Debug.LogError("biggerStartIndex Out Of Range!");
                return;
            }
            if (rotateEndIndex >= tmpUGUIText.Length || rotateEndIndex < 0)
            {
                Debug.LogWarning("biggerEndIndex Out Of Range!");
                rotateEndIndex = tmpUGUIText.Length - 1;
            }
            if (rotateEndIndex < rotateStartIndex)
            {
                Debug.LogError("biggerEndIndex must larger than biggerStartIndex");
                return;
            }
            StartCoroutine(RotateEffectCoroutine(rotateStartIndex, rotateEndIndex));
        }
    }

    /// <summary>
    /// 处理摆动效果协程
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="endIndex"></param>
    /// <returns></returns>
    IEnumerator RotateEffectCoroutine(int startIndex,int endIndex)
    {
        float tempAngle = rotationAngle*rotationSpeed; //每次摆动的角度
        float currentRotate = 0;                       //当前累计摆动的角度
        int rotateDir = 1;                             //摆动的方向

        TMP_TextInfo textInfo = tmpUGUI.textInfo;  //获取textInfo引用
        Matrix4x4 matrix;

        while(true)
        {
            TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();  //获取MeshInfo缓存
            //对于选定区域的字符应用旋转
            for (int i = startIndex; i <= endIndex; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                // 如果字符不可见，跳过
                if (!charInfo.isVisible) continue;

                // 获取当前字符的materialIndex
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                // 获取当前字符的vertexIndex
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                // 缓存当前字符的vertices
                Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
                // 字符vertices的直接引用
                Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                //计算出每个字符的中点
                Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

                //计算出字符到中心点的平移量
                Vector3 offset = charMidBasline;

                //先平移到中点，应用变换，然后平移回来
                destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
                destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
                destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
                destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;
                //创建T，R，S矩阵
                matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, tempAngle*rotateDir), Vector3.one);
                //应用变换
                destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
                destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
                destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
                destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);
                //平移回来
                destinationVertices[vertexIndex + 0] += offset;
                destinationVertices[vertexIndex + 1] += offset;
                destinationVertices[vertexIndex + 2] += offset;
                destinationVertices[vertexIndex + 3] += offset;
                //更新TMP的mesh
                for (int j = 0; j < textInfo.meshInfo.Length; j++)
                {
                    textInfo.meshInfo[j].mesh.vertices = textInfo.meshInfo[j].vertices;
                    tmpUGUI.UpdateGeometry(textInfo.meshInfo[j].mesh, j);
                }
            }
            //更新累计旋转角度，如果累计旋转角度到达设定值，变换方向
            currentRotate += tempAngle*rotateDir;
            if (currentRotate >= rotationAngle || currentRotate <= -rotationAngle) rotateDir *= -1;
            
            yield return new WaitForFixedUpdate();
        }
    }
}
