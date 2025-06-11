using Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEditor;

[StructLayout(LayoutKind.Explicit, Size = 32)]
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

    // // The InputControl annotations here probably look a little scary, but what we do
    // // here is relatively straightforward. The fields we add we annotate with
    // // [FieldOffset] to force them to the right location, and then we add InputControl
    // // to attach controls to the fields. Each InputControl attribute can only do one of
    // // two things: either it adds a new control or it modifies an existing control.
    // // Given that our layout is based on Gamepad, almost all the controls here are
    // inherited from Gamepad, and we just modify settings on them.

    [InputControl(name = "leftStick", layout = "Stick", format = "VC2B")]
    [InputControl(name = "leftStick/x", offset = 0, format = "BYTE",
        parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "leftStick/y", offset = 1, format = "BYTE",
        parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [FieldOffset(1)] public byte leftStickX;
    [FieldOffset(2)] public byte leftStickY;

    [InputControl(name = "button_1", displayName = "buttonSouth", bit = 5)]
    [FieldOffset(5)] public byte buttons1;
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
        Debug.Log("instancing...");
        // This is one way to match the Device.
         InputSystem.RegisterLayout<FishingRodGamepad>(
           matches: new InputDeviceMatcher()
                .WithInterface("HID")
                .WithCapability("vendorId", 0x2DC8) // Sony Entertainment.
                .WithCapability("productId", 0xAB12)); // Wireless controller.
    }

    // In the Player, to trigger the calling of the static constructor,
    // create an empty method annotated with RuntimeInitializeOnLoadMethod.
    [RuntimeInitializeOnLoadMethod]
    static void Init() {}
}