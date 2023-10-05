using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//TODO:分开长按和点击
//TODO:添加各个事件的AddListener方法
public class SGButton : Selectable, IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    #region Attributes
    [SerializeField]
    [Tooltip("取消勾选此项后，按钮点击后将跳过'选中'状态")]
    [Header("是否要有选中状态")]
    private bool needSelect = true;

    [SerializeField]
    [Tooltip("鼠标在按钮上停留超过设定时间后，会调用'鼠标悬停事件'")]
    [Header("最短悬停时间")]
    private float minHoverTime = 0.2f;

    [SerializeField]
    [Tooltip("鼠标在按钮上长按超过设定时间后，会每FixedUpdate调用'鼠标长按事件'")]
    [Header("最短长按时间")]
    private float minHoldTime = 0.3f;

    [SerializeField]
    [Tooltip("启用后，可以设置鼠标移入,点击等事件的按钮大小变换")]
    [Header("是否应用Scale变换")]
    private bool useScaleTrainsion = false;

    [SerializeField]
    [Header("鼠标进入时的缩放")]
    private Vector3 onMouseEnterScale = new Vector3(1,1,1);

    [SerializeField]
    [Header("鼠标悬停时的缩放")]
    private Vector3 onMouseHoverScale = new Vector3(1,1,1);

    [SerializeField]
    [Header("鼠标长按时的缩放")]
    private Vector3 onMouseHoldScale = new Vector3(1,1,1);

    private bool isHovered = false;
    private bool isHolding = false;
    private Coroutine holdHandler;
    #endregion

    #region Events
    [SerializeField]
    [Header("鼠标点击事件")]
    private UnityEvent onButtonClicked;

    [SerializeField]
    [Header("鼠标悬停事件")]
    [Header("在开始悬停时，会以True调用一次事件")]
    [Header("在结束悬停时，会以False调用一次事件")]
    private UnityEvent<bool> onButtonHovered;

    [SerializeField]
    [Header("鼠标长按事件")]
    [Header("在鼠标长按设定时间后，若仍保持长按，则每FixedUpdate调用一次事件")]
    private UnityEvent onButtonHolded;

    [SerializeField]
    [Header("鼠标进入事件")]
    private UnityEvent onMouseEntered;

    [SerializeField]
    [Header("鼠标离开事件")]
    private UnityEvent onMouseLeaved;
    #endregion

    public void OnPointerClick(PointerEventData eventData)
    {
        if(interactable)
        {
            //调用点击事件
            onButtonClicked?.Invoke();
            //如果不需要选中状态，则跳过
            if (!needSelect)
            {
                DoStateTransition(SelectionState.Highlighted, true);
            }
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if(interactable)
        {
            //调用鼠标进入事件
            onMouseEntered?.Invoke();
            //如果现在是非悬停状态，则开启处理悬停协程
            if(!isHovered)
            {
                StartCoroutine(HandleHover());
            }
            //如果不需要选中状态，鼠标进入时自动转为Highlighted
            if(!needSelect)
            {
                DoStateTransition(SelectionState.Highlighted, true);
            }
            if (useScaleTrainsion)
            {
                transform.localScale = onMouseEnterScale;
            }
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if(interactable)
        {
            //调用鼠标离开事件
            onMouseLeaved?.Invoke();
            //鼠标离开时，停止所有协程
            StopAllCoroutines();
            if(isHovered)
            {
                //如果当前状态是悬停状态，则鼠标离开时以调用False鼠标悬停事件
                onButtonHovered?.Invoke(false);
            }
            //将所有状态置为false
            isHovered = false;
            isHolding = false;
            //如果不需要选中状态，则鼠标离开时将状态置为Normal(而非Selected)
            if (!needSelect)
            {
                DoStateTransition(SelectionState.Normal, true);
            }
            if (useScaleTrainsion)
            {
                transform.localScale = Vector3.one;
            }
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if(interactable)
        {
            //如果现在非长按状态，则开始处理长按协程
            if(!isHolding)
            {
                holdHandler = StartCoroutine(HandleHold());
            }
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if(interactable)
        {
            //如果长按协程开启中,则停止长按协程
            if(!holdHandler.Equals(null))
            {
                StopCoroutine(holdHandler);
            }
            //长按状态设为false
            isHolding = false;
            if(useScaleTrainsion)
            {
                transform.localScale = onMouseHoverScale;
            }
        }
    }

    IEnumerator HandleHover()
    {
        yield return minHoverTime;
        isHovered = true;
        if(useScaleTrainsion)
        {
            transform.localScale = onMouseHoverScale;
        }
        onButtonHovered?.Invoke(true);
    }

    IEnumerator HandleHold()
    {
        yield return new WaitForSeconds(minHoldTime);
        isHolding = true;
        if(useScaleTrainsion)
        {
            transform.localScale = onMouseHoldScale;
        }
        while(isHolding)
        {
            onButtonHolded?.Invoke();
            yield return new WaitForFixedUpdate();
        }
    }
}
