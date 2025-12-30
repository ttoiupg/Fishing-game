using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using System.Linq;

public class VibrationHandler : MonoBehaviour {
    public static VibrationHandler Instance;
    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    internal struct TwoByteCommand : IInputDeviceCommandInfo {
        public static FourCC Type => new FourCC('H', 'I', 'D', 'O');
        public FourCC typeStatic => Type;

        internal const int kSize = InputDeviceCommand.BaseCommandSize + 3; // 8 + 1 + 2

        [FieldOffset(0)]
        public InputDeviceCommand baseCommand;

        // Byte 0: Report ID (必須放在這裡)
        [FieldOffset(InputDeviceCommand.BaseCommandSize)]
        public byte reportId;

        // Byte 1: 第一個數據 (例如: 左馬達強度)
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 1)]
        public byte byte1;

        // Byte 2: 第二個數據 (例如: 右馬達強度)
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 2)]
        public byte byte2;

        public static TwoByteCommand Create(byte id, byte b1, byte b2) {
            return new TwoByteCommand {
                baseCommand = new InputDeviceCommand(Type, kSize),
                reportId = id,
                byte1 = b1,
                byte2 = b2
            };
        }
    }
    public bool vibrationEnabled = false;
    public void ToggleVibration() {
        if(vibrationEnabled) {
            //SendBytes(0, 0);
        } else {
            //SendBytes(80, 80);
        }
        vibrationEnabled = !vibrationEnabled;
    }
    public float Remap(float value, float fromSource, float toSource, float fromTarget, float toTarget) {
        return fromTarget + (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget);
    }
    public void SetMotorSpeed(float lowFreq, float highFreq) {

        if(Gamepad.current == null) return;
        if(Gamepad.current.name != "DualShock4GamepadHID" && Gamepad.current.name != "FishingRodGamepad") {
            Gamepad.current.SetMotorSpeeds(lowFreq, highFreq);
        } else if(Gamepad.current.name == "FishingRodGamepad") {
            SetVibration(lowFreq, highFreq);
        }
    }
    //void Update() {
    //    // 按下 T 鍵測試發送
    //    if(Keyboard.current.tKey.wasPressedThisFrame) {
    //        // 範例：發送 128 (約50%強度) 和 255 (100%強度)
    //        SendBytes(0x80, 0xFF);
    //    }
    //}

    void SendBytes(byte data1, byte data2) {
        // 1. 尋找你的 ESP32 裝置 (確保它是 HID 類型)
        var device = InputSystem.devices.FirstOrDefault(x => x is Gamepad && x.name.Contains("FishingRodGamepad"));

        if(device == null) {
            Debug.LogError("找不到 ESP32 裝置！請確認藍牙已連接且名稱正確。");
            return;
        }

        // 2. 建立指令
        // [重要]: Report ID 這裡先設為 0x00 或 0x01
        // 如果你的 ESP32 使用預設庫設定，通常 ID 是 0x00 或 0x01。
        // 如果 Result 回傳 -1，請將這裡的 0x00 改成 0x01 試試看。
        byte targetReportId = 0x05;

        var command = TwoByteCommand.Create(targetReportId, data1, data2);

        // 3. 執行指令
        long result = device.ExecuteCommand(ref command);

        //if(result >= 0)
            //Debug.Log($"發送成功! ID: {targetReportId}, Data: {data1}, {data2} (Result: {result})");
        //else
            //Debug.LogError($"發送失敗! Result: {result} (請嘗試更改 Report ID 或確認藍牙已重新配對)");
    }

    private void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    public void SetVibration(float LowFrequency, float HighFrequency) {
        var lremap = Remap(LowFrequency,0,1,59,150);
        if(lremap < 60) lremap = 0;
        var hremap = Remap(HighFrequency, 0, 1, 59, 180);
        if(hremap < 60) hremap = 0;
        byte lowFreq = (byte)Mathf.Clamp(lremap, 0, 150);
        byte highFreq = (byte)Mathf.Clamp(hremap, 0, 180);
        SendBytes(lowFreq, highFreq);
    }
    private void OnApplicationQuit() {
        SetMotorSpeed(0, 0);
    }
}