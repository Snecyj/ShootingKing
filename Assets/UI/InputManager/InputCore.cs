#if UNITY_PSP2
using UnityEngine;
using System.Collections;

namespace UnityEngine.PSVita{
	public enum Key{
		Cross=KeyCode.JoystickButton0,
		Circle=KeyCode.Joystick1Button1,
		Square=KeyCode.Joystick1Button2,
		Triangle=KeyCode.Joystick1Button3,
		LTrigger=KeyCode.Joystick1Button4,
		RTrigger=KeyCode.Joystick1Button5,
		Select=KeyCode.Joystick1Button6,
		Start=KeyCode.Joystick1Button7,
		Dup=KeyCode.Joystick1Button8,
		Dright=KeyCode.Joystick1Button9,
		Ddown=KeyCode.Joystick1Button10,
		Dleft=KeyCode.Joystick1Button11	
	}
	public enum Axis{
		LX,
		LY,
		LB,
		RX,
		RY,
		RB
	}
	public static class VitaInput
	{
		public static bool GetButton(Key key){
			return Input.GetKey((KeyCode)key);
		}
		public static bool GetButtonDown(Key key){
			return Input.GetKeyDown((KeyCode)key);
		}
		public static bool GetButtonUp(Key key){
			return Input.GetKeyUp((KeyCode)key);
		}
		public static float GetAxis(Axis axis){
			return Input.GetAxis(axis.ToString());
		}
	}
}
#endif
