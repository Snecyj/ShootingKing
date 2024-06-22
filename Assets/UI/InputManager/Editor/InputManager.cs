using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor;

namespace Input
{
	public class InputManager : EditorWindow, IPreprocessBuild
	{

		static InputManager inst;

		[System.Serializable]
		public class PlatformInput
		{
			public string name;
			public BuildTarget[] platform;
			public InputScriptable inputAxes;
		}

		static PlatformInput[] s_platforms;
		static InputScriptable s_DefaultInput;

		public PlatformInput[] platforms;
		public InputScriptable DefaultInput;


		[MenuItem("Window/Pre-Build InputManager")]
		static void Init()
		{
			GetWindow(typeof(InputManager));
		}

		void OnEnable()
		{
			var data = EditorPrefs.GetString("InputManager", JsonUtility.ToJson(this, false));
			JsonUtility.FromJsonOverwrite(data, this);
		}
		void OnDisable()
		{
			// We get the Json data
			var data = JsonUtility.ToJson(this, false);
			// And we save it
			EditorPrefs.SetString("InputManager", data);
		}

		void OnGUI()
		{
			if (DefaultInput != s_DefaultInput)
			{
				SetupInputManager(DefaultInput);
				s_DefaultInput = DefaultInput;
			}
			ScriptableObject target = this;
			SerializedObject so = new SerializedObject(target);
			EditorGUILayout.PropertyField(so.FindProperty("DefaultInput"), false); // True means show children
			EditorGUILayout.PropertyField(so.FindProperty("platforms"), true); // True means show children
			so.ApplyModifiedProperties(); // Remember to apply modified properties
		}

		public int callbackOrder { get { return 0; } }
		public void OnPreprocessBuild(BuildTarget target, string path)
		{
			s_platforms = platforms;
			s_DefaultInput = DefaultInput;
			if (s_platforms != null && s_platforms.Length > 0)
			{
				foreach (var item in s_platforms)
				{
					foreach (var plat in item.platform)
					{
						if (plat == target)
						{
							SetupInputManager(item.inputAxes);
							return;
						}
					}
				}
			}
			else
			{
				if (s_DefaultInput != null)
					SetupInputManager(s_DefaultInput);
			}
		}

		private static void Clear()
		{
			SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
			SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");
			axesProperty.ClearArray();
			serializedObject.ApplyModifiedProperties();
		}

		internal static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
		{
			SerializedProperty child = parent.Copy();
			child.Next(true);
			do
			{
				if (child.name == name) return child;
			}
			while (child.Next(false));
			return null;
		}
		private static bool AxisDefined(string axisName)
		{
			SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
			SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

			axesProperty.Next(true);
			axesProperty.Next(true);
			while (axesProperty.Next(false))
			{
				SerializedProperty axis = axesProperty.Copy();
				axis.Next(true);
				if (axis.stringValue == axisName) return true;
			}
			return false;
		}

		private static void AddAxis(InputAxis axis)
		{
			if (AxisDefined(axis.name)) return;

			SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
			SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

			axesProperty.arraySize++;
			serializedObject.ApplyModifiedProperties();

			SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

			GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
			GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
			GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
			GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
			GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
			GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
			GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
			GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
			GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
			GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
			GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
			GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
			GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
			GetChildProperty(axisProperty, "axis").intValue = axis.axis;
			GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;

			serializedObject.ApplyModifiedProperties();
		}
		public static void SetupInputManager(InputScriptable input)
		{
			Clear();
			if (input == null)
				return;
			foreach (var item in input.Axes)
			{
				AddAxis(item);
			}
		}
		public enum AxisType
		{
			KeyOrMouseButton = 0,
			MouseMovement = 1,
			JoystickAxis = 2
		};

		[System.Serializable]
		public struct InputAxis
		{
			public string name;
			public string descriptiveName;
			public string descriptiveNegativeName;
			public string negativeButton;
			public string positiveButton;
			public string altNegativeButton;
			public string altPositiveButton;

			public float gravity;
			public float dead;
			public float sensitivity;

			public bool snap;
			public bool invert;

			public AxisType type;

			public int axis;
			public int joyNum;
		}
	}
}