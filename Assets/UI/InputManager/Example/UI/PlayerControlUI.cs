using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerControlUI : Selectable, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	[System.Flags]
	public enum Mode{
		IsStatic=1,
		IsDelta=2,

	}

	public Mode flags;

	public string axisName = "PlayerAxis";
	VirtualsInputs.VirtualAxis.AxisInput<Vector2> axis;
	PointerEventData pointer;
	[Range(0.01f,1f)]
	public float screenPercentFill = 0.2f;
	Vector2 deltaPoint;
	public Image joyBack,joyPoint;

	[SerializeField] UnityEvent BeginDrag, Drag, EndDrag, FailDrag, SmallDrag;

#if UNITY_EDITOR
	protected override void OnValidate()
	{
		RectTransform rT = (RectTransform)transform;
		if (rT){ 
			if (joyBack)
			{
				joyBack.transform.SetParent(transform, false);
				if (joyPoint)
				{
					joyPoint.transform.SetParent(joyBack.transform, false);
				}
				joyBack.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rT.rect.width * 2 * screenPercentFill);
				joyBack.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rT.rect.width * 2 * screenPercentFill);
			}
		}
	}
	#endif

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (pointer == null){
			BeginDrag.Invoke();
			pointer = eventData;
			deltaPoint = Vector2.zero;
			RectTransform rT = (RectTransform)transform;
			if (joyBack && ((flags & Mode.IsStatic) == Mode.IsStatic || (flags & Mode.IsDelta) != Mode.IsDelta))
			{
				joyBack.gameObject.SetActive(true);
				joyBack.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rT.rect.width * 2 * screenPercentFill);
				joyBack.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rT.rect.width * 2 * screenPercentFill);
			}
			UpdateAxisByVector2(deltaPoint, VirtualsInputs.AxisState.Begin);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (eventData == pointer){
			if ((flags & Mode.IsStatic) == Mode.IsStatic)
			{
				deltaPoint = Vector2.ClampMagnitude((eventData.position - eventData.pressPosition) / Screen.width / screenPercentFill, 1f);
				if ((flags & Mode.IsDelta) == Mode.IsDelta)
				{
					if (joyBack)
					{
						joyBack.rectTransform.position = eventData.position - deltaPoint * Screen.width * screenPercentFill;
						if (joyPoint)
							joyPoint.rectTransform.position = eventData.position;
					}
				}
				else
				{
					if (joyBack)
					{
						joyBack.rectTransform.position = eventData.pressPosition;
						if (joyPoint)
							joyPoint.rectTransform.position = eventData.pressPosition + deltaPoint * Screen.width * screenPercentFill;
					}
				}
			}
			else
			{
				if ((flags & Mode.IsDelta) == Mode.IsDelta)
				{
					deltaPoint = eventData.delta / Screen.width / screenPercentFill;
				}
				else
				{
					deltaPoint = Vector2.ClampMagnitude(deltaPoint + eventData.delta / Screen.width / screenPercentFill, 1f);
					if (joyBack)
					{
						joyBack.rectTransform.position = eventData.position - deltaPoint * Screen.width * screenPercentFill;
						if (joyPoint)
							joyPoint.rectTransform.position = eventData.position;
					}
				}
			}
				Drag.Invoke();
			UpdateAxisByVector2(deltaPoint, VirtualsInputs.AxisState.Update);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (eventData == pointer)
		{
			if (deltaPoint.magnitude > 0.5f)
			{
				EndDrag.Invoke();
			}
			else
            {
				FailDrag.Invoke();
            }
				UpdateAxisByVector2(deltaPoint, VirtualsInputs.AxisState.End);
			pointer = null;
			if (joyBack)
				joyBack.gameObject.SetActive(false);
		}
	}
	protected override void OnEnable()
	{
		base.OnEnable();
		if (joyBack)
			joyBack.gameObject.SetActive(false);

		if (VirtualInputManager.instance != null && VirtualInputManager.instance.inputs.GetVirtualInput(axisName, out axis)) { }
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		UpdateAxisByVector2(deltaPoint, VirtualsInputs.AxisState.End);
		if (joyBack)
			joyBack.gameObject.SetActive(false);
		pointer = null;
	}
	protected override void OnDestroy()
	{
		base.OnDestroy();
		UpdateAxisByVector2(deltaPoint, VirtualsInputs.AxisState.End);
		if (joyBack)
			joyBack.gameObject.SetActive(false);
	}
	void UpdateAxisByVector2(Vector2 vector2, VirtualsInputs.AxisState state)
	{
		if (axis!=null && axis.onAction!=null)
			axis.onAction.Invoke(vector2, state);
	}

	protected override void Start()
	{
		if (joyBack)
			joyBack.gameObject.SetActive(false);
		base.Start();
		if (VirtualInputManager.instance!=null && VirtualInputManager.instance.inputs.GetVirtualInput(axisName, out axis)) { }
	}
}
