using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.spacepuppy.UserInput.Unity;

namespace com.spacepuppy.UserInput.SPInput
{

    public static class SPInputFactory
    {

        public static IButtonInputSignature CreateButtonSignature(string id, SPInputButton button, SPJoystick joystick = SPJoystick.All)
        {
            return new ButtonInputSignature(id, GetButtonInputId(button, joystick));
        }

        public static IButtonInputSignature CreateButtonSignature(string id, int hash, SPInputButton button, SPJoystick joystick = SPJoystick.All)
        {
            return new ButtonInputSignature(id, hash, GetButtonInputId(button, joystick));
        }

        public static IButtonInputSignature CreateAxleButtonSignature(string id, SPInputAxis axis, SPJoystick joystick = SPJoystick.All)
        {
            return new AxleButtonInputSignature(id, GetAxisInputId(axis, joystick));
        }

        public static IButtonInputSignature CreateAxleButtonSignature(string id, int hash, SPInputAxis axis, SPJoystick joystick = SPJoystick.All)
        {
            return new AxleButtonInputSignature(id, hash, GetAxisInputId(axis, joystick));
        }

        public static IAxleInputSignature CreateAxisSignature(string id, SPInputAxis axis, SPJoystick joystick = SPJoystick.All)
        {
            return new AxleInputSignature(id, GetAxisInputId(axis, joystick));
        }

        public static IAxleInputSignature CreateAxisSignature(string id, int hash, SPInputAxis axis, SPJoystick joystick = SPJoystick.All)
        {
            return new AxleInputSignature(id, hash, GetAxisInputId(axis, joystick));
        }

        public static IDualAxleInputSignature CreateDualAxisSignature(string id, SPInputAxis axisX, SPInputAxis axisY, SPJoystick joystick = SPJoystick.All)
        {
            return new DualAxleInputSignature(id, GetAxisInputId(axisX, joystick), GetAxisInputId(axisY, joystick));
        }

        public static IDualAxleInputSignature CreateDualAxisSignature(string id, int hash, SPInputAxis axisX, SPInputAxis axisY, SPJoystick joystick = SPJoystick.All)
        {
            return new DualAxleInputSignature(id, GetAxisInputId(axisX, joystick), GetAxisInputId(axisY, joystick));
        }
        


        public static string GetButtonInputId(SPInputButton button, SPJoystick joystick = SPJoystick.All)
        {
            if (joystick == SPJoystick.All)
            {
                if (button <= SPInputButton.Button19)
                    return string.Format("JoyAll-Button{0:00}", (int)button);
                else
                    return string.Format("MouseButton{0:0}", (int)button - (int)SPInputButton.MouseButton0);
            }
            else
            {
                if(button <= SPInputButton.Button19)
                    return string.Format("Joy{0:0}-Button{0:00}", (int)joystick, (int)button);
                else
                    return string.Format("MouseButton{0:0}", (int)button - (int)SPInputButton.MouseButton0);
            }
        }

        public static string GetAxisInputId(SPInputAxis axis, SPJoystick joystick = SPJoystick.All)
        {
            if (joystick == SPJoystick.All)
            {
                if (axis <= SPInputAxis.Axis28)
                    return string.Format("JoyAll-Axis{0:00}", (int)axis + 1);
                else
                    return string.Format("MouseAxis{0:0}", (int)axis - (int)SPInputAxis.MouseAxis1 + 1);
            }
            else
            {
                if (axis <= SPInputAxis.Axis28)
                    return string.Format("Joy{0:0}-Axis{0:00}", (int)joystick, (int)axis + 1);
                else
                    return string.Format("MouseAxis{0:0}", (int)axis - (int)SPInputAxis.MouseAxis1 + 1);
            }
        }
        

    }

}
