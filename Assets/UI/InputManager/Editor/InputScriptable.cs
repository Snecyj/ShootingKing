using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Input
{
	[CreateAssetMenu(fileName = "InputAsset", menuName = "Input Manager/Create Input", order = 1)]
	public class InputScriptable : ScriptableObject
	{
		public InputManager.InputAxis[] Axes;

#if UNITY_EDITOR
		[ContextMenu("Load Default")]
		void LoadDefault()
		{

			List<InputManager.InputAxis> inputAxes = new List<InputManager.InputAxis>();

			SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
			SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");
			SerializedProperty axisProperty;
			int id = 0;
			InputManager.InputAxis axis;
			while (id < axesProperty.arraySize)
			{
				axisProperty = axesProperty.GetArrayElementAtIndex(id);
				axis = new InputManager.InputAxis();
				axis.name = InputManager.GetChildProperty(axisProperty, "m_Name").stringValue;
				axis.descriptiveName = InputManager.GetChildProperty(axisProperty, "descriptiveName").stringValue;
				axis.descriptiveNegativeName = InputManager.GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue;
				axis.negativeButton = InputManager.GetChildProperty(axisProperty, "negativeButton").stringValue;
				axis.positiveButton = InputManager.GetChildProperty(axisProperty, "positiveButton").stringValue;
				axis.altNegativeButton = InputManager.GetChildProperty(axisProperty, "altNegativeButton").stringValue;
				axis.altPositiveButton = InputManager.GetChildProperty(axisProperty, "altPositiveButton").stringValue;
				axis.gravity = InputManager.GetChildProperty(axisProperty, "gravity").floatValue;
				axis.dead = InputManager.GetChildProperty(axisProperty, "dead").floatValue;
				axis.sensitivity = InputManager.GetChildProperty(axisProperty, "sensitivity").floatValue;
				axis.snap = InputManager.GetChildProperty(axisProperty, "snap").boolValue;
				axis.invert = InputManager.GetChildProperty(axisProperty, "invert").boolValue;
				axis.type = (InputManager.AxisType)InputManager.GetChildProperty(axisProperty, "type").intValue;
				axis.axis = InputManager.GetChildProperty(axisProperty, "axis").intValue;
				axis.joyNum = InputManager.GetChildProperty(axisProperty, "joyNum").intValue;
				inputAxes.Add(axis);
				id++;
			}
			Axes = inputAxes.ToArray();
		}
#endif
	}
}