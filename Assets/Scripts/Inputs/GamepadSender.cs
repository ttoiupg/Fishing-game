using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

public class GamepadSender : MonoBehaviour {
    // 定義 HID Output Command 結構
    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    internal struct OutputReportCommand : IInputDeviceCommandInfo {
        public static FourCC Type => new FourCC('H', 'I', 'D', 'O'); // HID Output 的標識符
        public FourCC typeStatic => Type;
        internal const int id = 5;

        internal const int kSize = InputDeviceCommand.BaseCommandSize + 8; // BaseSize + 你的數據長度 (這裡範例送 8 bytes)

        [FieldOffset(0)]
        public InputDeviceCommand baseCommand;
        [FieldOffset(InputDeviceCommand.BaseCommandSize)]
        public byte reportId;
        // 從這裡開始是你的自定義數據
        [FieldOffset(InputDeviceCommand.BaseCommandSize)]
        public byte byte1;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 1)]
        public byte byte2;
        // ... 你可以繼續定義更多 byte

        public static OutputReportCommand Create(byte b1, byte b2) {
            return new OutputReportCommand {
                baseCommand = new InputDeviceCommand(Type, kSize),
                reportId = id,
                byte1 = b1,
                byte2 = b2
            };
        }
    }

    void Update() {
        // 範例：按下 Space 鍵發送數據
        if(Keyboard.current.spaceKey.wasPressedThisFrame) {
            SendToESP32(0xFF, 0xAA); // 發送測試數據
        }
    }

    void SendToESP32(byte data1, byte data2) {
        var gamepad = Gamepad.current;
        if(gamepad == null) {
            Debug.LogWarning("找不到 Gamepad！");
            return;
        }

        // 建立指令封包
        var command = OutputReportCommand.Create(data1, data2);

        // 發送指令
        long result = gamepad.ExecuteCommand(ref command);

        if(result >= 0)
            Debug.Log($"數據發送成功！ Result: {result}");
        else
            Debug.LogError($"發送失敗。 Result: {result}");
    }
}