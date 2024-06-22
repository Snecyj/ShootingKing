using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIDrag : Selectable, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	[SerializeField] UnityEvent BeginDrag, Drag, EndDrag;
	public void OnBeginDrag(PointerEventData eventData)
	{
		BeginDrag.Invoke();
	}

	public void OnDrag(PointerEventData eventData)
	{
		Drag.Invoke();
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		EndDrag.Invoke();
	}

}
