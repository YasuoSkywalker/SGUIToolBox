using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramingUIComponentTest : MonoBehaviour
{
    private float rate = 0.0f;
    [SerializeField]
    GameObject testHoldBar;

    //测试点击函数
    public void TestClick(int arg)
    {
        Debug.Log("Button has been clicked,arg is:" + arg);
    }

    //测试悬停函数
    public void TestHover(bool isHover)
    {
        if(isHover)
        {
            Debug.Log("Button is hovered");
        }
        else
        {
            Debug.Log("Button is not hovered");
        }
    }

    //测试长按函数
    public void TestHold()
    {
        if (rate < 1)
        {
            rate += Time.fixedDeltaTime;
            testHoldBar.transform.localScale = new Vector3(rate, 1, 1);
        }
        else
            Debug.Log("Rate is 100% now!");
    }
}
