using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputAsset", menuName = "Input Manager/Create Virtual Inputs", order = 2)]
public class VirtualsInputs : ScriptableObject
{
	public VirtualAxis[] Virtuals;

	public bool GetVirtualInput<T>(string name, out VirtualAxis.AxisInput<T> axis)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			axis = null;
			return false;
		}
		foreach (VirtualAxis item in Virtuals)
		{
			if (item.name == name)
			{
				if (item.Get(out axis))
				{
					return true;
				}
			}
		}
		Debug.LogError("Axis \"" + name + "\" not found");
		axis = null;
		return false;
	}

	public void AddListener<T>(string name, System.Action<T, AxisState> onValue)
	{
		VirtualAxis.AxisInput<T> _t;
		if (GetVirtualInput(name, out _t))
		{
			_t.onAction += onValue;
		}
	}
	public void RemoveListener<T>(string name, System.Action<T,AxisState> onValue)
	{
		VirtualAxis.AxisInput<T> _t;
		if (GetVirtualInput(name, out _t))
		{
			_t.onAction -= onValue;
		}
	}


	public enum AxisState
	{
		Begin,
		Update,
		End
	};
	public enum AxisInputType
	{
		_float,
		_int,
		_long,
		_string,
		_vector2,
		_vector3,
		_vector4,
		_color,
		_color32
	};
	[System.Serializable]
	public class VirtualAxis
	{

		[SerializeField]
		string _name;
		[SerializeField]
		AxisInputType type;
		public string name { get { return _name; } }
		AxisInvoke onValue;
		public void Init()
		{
			switch (type)
			{
				case AxisInputType._float:
					onValue = new AxisInput<float>();
					break;
				case AxisInputType._int:
					onValue = new AxisInput<int>();
					break;
				case AxisInputType._long:
					onValue = new AxisInput<long>();
					break;
				case AxisInputType._string:
					onValue = new AxisInput<string>();
					break;
				case AxisInputType._vector2:
					onValue = new AxisInput<Vector2>();
					break;
				case AxisInputType._vector3:
					onValue = new AxisInput<Vector3>();
					break;
				case AxisInputType._vector4:
					onValue = new AxisInput<Vector4>();
					break;
				case AxisInputType._color:
					onValue = new AxisInput<Color>();
					break;
				case AxisInputType._color32:
					onValue = new AxisInput<Color32>();
					break;
			}
		}
		public void Invoke<T>(T _object, AxisState state)
		{
			onValue.Invoke(_object, state);
		}

		internal bool Get<T>(out AxisInput<T> axis)
		{
			if (onValue == null){
				Init();
			}
			axis = (AxisInput<T>)onValue;
			return axis != null;
		}

		public class AxisInput<T> : AxisInvoke
		{
			public System.Action<T, AxisState> onAction;
		}
		public abstract class AxisInvoke
		{
			public AxisInvoke Construct<T>() where T : new()
			{
				return new AxisInput<T>();
			}

			public void Invoke<T>(T _object, AxisState state)
			{
				AxisInput<T> _t = (AxisInput<T>)this;
				_t.onAction.Invoke(_object, state);
			}
		}
	}
}
