using Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEditor;

[StructLayout(LayoutKind.Explicit, Size = 224)]
struct FishingRodEsp32GamepadHIDInputReport : IInputStateTypeInfo
{
    // Because all HID input reports are tagged with the 'HID ' FourCC,
    // this is the format we need to use for this state struct.
    public FourCC format => new FourCC('H', 'I', 'D');

    // HID input reports can start with an 8-bit report ID. It depends on the device
    // whether this is present or not. On the PS4 DualShock controller, it is
    // present. We don't really need to add the field, but let's do so for the sake of
    // completeness. This can also help with debugging.
    [FieldOffset(0)] public byte reportId;

    // The InputControl annotations here probably look a little scary, but what we do
    // here is relatively straightforward. The fields we add we annotate with
    // [FieldOffset] to force them to the right location, and then we add InputControl
    // to attach controls to the fields. Each InputControl attribute can only do one of
    // two things: either it adds a new control or it modifies an existing control.
    // Given that our layout is based on Gamepad, almost all the controls here are
    // inherited from Gamepad, and we just modify settings on them.

    [InputControl(name = "leftStick", layout = "Stick", format = "VC2S")]
    [InputControl(name = "leftStick/x", offset = 4, format = "SHRT", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "leftStick/left", offset = 4, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "leftStick/right", offset = 4, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1")]
    [InputControl(name = "leftStick/y", offset = 2, format = "SHRT", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "leftStick/up", offset = 2, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "leftStick/down", offset = 2, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1,invert=false")]
    [FieldOffset(0)] public byte leftStickX;
     [FieldOffset(3)] public byte leftStickY;

    [InputControl(name = "rightStick", layout = "Stick", format = "VC2S")]
    [InputControl(name = "rightStick/x", offset = 4, format = "SHRT", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "rightStick/left", offset = 4, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "rightStick/right", offset = 4, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1")]
    [InputControl(name = "rightStick/y", offset = 2, format = "SHRT", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "rightStick/up", offset = 2, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "rightStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1,invert=false")]
    [FieldOffset(0)] public byte rightStickX;
    [FieldOffset(3)] public byte rightStickY;

    //[InputControl(name = "dpad", format = "BIT", layout = "Dpad", sizeInBits = 4, defaultState = 8)]
    //[InputControl(name = "dpad/up", format = "BIT", layout = "DiscreteButton", parameters = "minValue=7,maxValue=1,nullValue=8,wrapAtValue=7", bit = 0, sizeInBits = 4)]
    //[InputControl(name = "dpad/right", format = "BIT", layout = "DiscreteButton", parameters = "minValue=1,maxValue=3", bit = 0, sizeInBits = 4)]
    //[InputControl(name = "dpad/down", format = "BIT", layout = "DiscreteButton", parameters = "minValue=3,maxValue=5", bit = 0, sizeInBits = 4)]
    //[InputControl(name = "dpad/left", format = "BIT", layout = "DiscreteButton", parameters = "minValue=5, maxValue=7", bit = 0, sizeInBits = 4)]
    [InputControl(name = "buttonWest", displayName = "Square", bit = 9)]
    [InputControl(name = "buttonSouth", displayName = "Cross", bit = 8)]
    [InputControl(name = "buttonEast", displayName = "Circle", bit = 10)]
    [InputControl(name = "buttonNorth", displayName = "Triangle", bit = 11)]
    [FieldOffset(0)] public byte buttons1;

    //[InputControl(name = "leftShoulder", bit = 0)]
    //[InputControl(name = "rightShoulder", bit = 1)]
    [InputControl(name = "rightTrigger", offset = 12, format = "BYTE")]
    [FieldOffset(6)] public byte rightTrigger;
    //[InputControl(name = "select", displayName = "Share", bit = 4)]
    //[InputControl(name = "start", displayName = "Options", bit = 5)]
    //[InputControl(name = "leftStickPress", bit = 6)]
    //[InputControl(name = "rightStickPress", bit = 7)]

    //[InputControl(name = "systemButton", layout = "Button", displayName = "System", bit = 0)]
    //[InputControl(name = "touchpadButton", layout = "Button", displayName = "Touchpad Press", bit = 1)]
    //[FieldOffset(7)] public byte buttons3;

    //[InputControl(name = "leftTrigger", format = "BYTE")]
    //[FieldOffset(8)] public byte leftTrigger;

    //[InputControl(name = "rightTrigger", format = "BYTE")]
    //[FieldOffset(9)] public byte rightTrigger;

    [FieldOffset(30)] public byte batteryLevel;
}

[InputControlLayout(stateType = typeof(FishingRodEsp32GamepadHIDInputReport))]
#if UNITY_EDITOR
[InitializeOnLoad] // Make sure static constructor is called during startup.
#endif

public class FishingRodGamepad : Gamepad
{
    static FishingRodGamepad()
    {
        // This is one way to match the Device.
         InputSystem.RegisterLayout<FishingRodGamepad>(
           matches: new InputDeviceMatcher()
                .WithInterface("HID")
                .WithCapability("vendorId", 0xe502) // Sony Entertainment.
                .WithCapability("productId", 0xabcd)); // Wireless controller.
    }

    // In the Player, to trigger the calling of the static constructor,
    // create an empty method annotated with RuntimeInitializeOnLoadMethod.
    [RuntimeInitializeOnLoadMethod]
    static void Init() {}
}