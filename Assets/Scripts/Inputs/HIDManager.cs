//using HidLibrary;

//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace HIDOutputDemo_CSharp {
//    class HIDManager {
//        private HidDevice _hidDevice;
//        private UInt16 _deviceVID;
//        private UInt16 _devicePID;
//        private string lastErrorString;
//        public string getLastError() {
//            return lastErrorString;
//        }
//        public HIDManager(UInt16 VID, UInt16 PID) {
//            _deviceVID = VID;
//            _devicePID = PID;
//        }
//        /// <summary>
//        /// Open the specified device
//        /// </summary>
//        /// <returns>Return whether the opening was successful</returns>
//        public bool OpenDevice() {
//            var devices = HidDevices.Enumerate();
//            //Debug.WriteLine(devices);
//            //Filter eligible devices
//            foreach(var device in devices) {
//                //Obtain the VID and PID of the device
//                int deviceVid = device.Attributes.VendorId;
//                int devicePid = device.Attributes.ProductId;
//                //Compare whether the VID and PID of the device match the target
//                if(deviceVid == _deviceVID && devicePid == _devicePID) {
//                    Debug.WriteLine($"Found device: VID = {deviceVid}, PID = {devicePid}");
//                    _hidDevice = device;
//                    //Open the device
//                    _hidDevice.OpenDevice();

//                    Debug.WriteLine($"Device Connected!");
//                    return true;
//                }
//            }
//            return false;
//        }
//        /// <summary>
//        /// Send Output Report
//        /// </summary>
//        ///<param name="reportId">Report ID</param>
//        ///<param name="data">Data to be sent</param>
//        ///<returns>Return whether the write was successful</returns>
//        ///<exception cref="InvalidOperationException">Throwing an exception when the device is not connected</observation>
//        public bool SendOutputReport(byte reportId, byte[] data) {
//            if(_hidDevice == null) {
//                throw new InvalidOperationException("HID device not connected");
//            }

//            //The length of HID report (including report ID) is usually defined by the device
//            int reportLength = _hidDevice.Capabilities.OutputReportByteLength;
//            Debug.Write(reportLength);
//            if(reportLength > 0) {
//                //Create a report to be sent, including the report ID
//                byte[] outputReport = new byte[reportLength];

//                outputReport[0] = reportId; //  Set Report ID

//                //Fill the data into the report, starting from the second byte (the first byte is the report ID)
//                for(int i = 0; i < data.Length && i < reportLength - 1; i++) {
//                    outputReport[i + 1] = data[i];
//                }
//                //Send a report using the Write method of HID library
//                Debug.WriteLine(outputReport[1]);
//                bool result = _hidDevice.Write(outputReport);
//                if(!result) {
//                    // get error code
//                    int errorCode = Marshal.GetLastWin32Error();
//                    string errorMessage = new Win32Exception(errorCode).Message;

//                    lastErrorString = $"Error writing report. Code: {errorCode}, Message: {errorMessage}";

//                }
//                //Check if the write was successful
//                return result;
//            }
//            return false;
//        }
//        public bool IsDeviceOpened() {
//            if(_hidDevice != null && _hidDevice.IsConnected && _hidDevice.IsOpen) {
//                return true;
//            } else {
//                return false;
//            }
//        }
//        public string GetAllDeviceInfo() {
//            if(_hidDevice == null) {
//                throw new InvalidOperationException("HID device not connected");
//            }

//            var capabilities = _hidDevice.Capabilities;
//            string info = "Info:\n";
//            info += $"VID: {_hidDevice.Attributes.VendorHexId}\n";
//            info += $"PID: {_hidDevice.Attributes.ProductHexId}\n";
//            info += $"Usage Page: {capabilities.UsagePage}\n";
//            info += $"Usage: {capabilities.Usage}\n";
//            info += $"Input Report Byte Length: {capabilities.InputReportByteLength}\n";
//            info += $"Output Report Byte Length: {capabilities.OutputReportByteLength}\n";
//            info += $"Feature Report Byte Length: {capabilities.FeatureReportByteLength}\n";
//            info += $"Number of Input Button Caps: {capabilities.NumberInputButtonCaps}\n";
//            info += $"Number of Input Value Caps: {capabilities.NumberInputValueCaps}\n";
//            info += $"Number of Input Data Indices: {capabilities.NumberInputDataIndices}\n";
//            info += $"Number of Output Button Caps: {capabilities.NumberOutputButtonCaps}\n";
//            info += $"Number of Output Value Caps: {capabilities.NumberOutputValueCaps}\n";
//            info += $"Number of Output Data Indices: {capabilities.NumberOutputDataIndices}\n";
//            info += $"Number of Feature Button Caps: {capabilities.NumberFeatureButtonCaps}\n";
//            info += $"Number of Feature Value Caps: {capabilities.NumberFeatureValueCaps}\n";
//            info += $"Number of Feature Data Indices: {capabilities.NumberFeatureDataIndices}\n";

//            return info;
//        }


//    }
//}