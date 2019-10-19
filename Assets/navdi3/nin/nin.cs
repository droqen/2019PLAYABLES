using UnityEngine;
using System.Collections;

namespace navdi3
{

    public static class nin // 'nin' for 'navdi input'
    {
        public static bool JoyButt(int joy, int butt)
        {
            return Input.GetKey(KeyCode.Joystick1Button0 + butt + (joy-1) * (KeyCode.Joystick1Button0 - KeyCode.Joystick2Button0));
        }
        public static bool JoyButtDown(int joy, int butt)
        {
            return Input.GetKeyDown(KeyCode.Joystick1Button0 + butt + (joy-1) * (KeyCode.Joystick1Button0 - KeyCode.Joystick2Button0));
        }
        public static float JoyAxis(int joy, int axis)
        {
            return Input.GetAxisRaw("joy" + joy + "axis" + axis);
        }
    }

}