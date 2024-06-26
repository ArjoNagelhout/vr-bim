﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Net.Sockets;


namespace revit_to_vr_common
{
    // common configuration parameters for the program to be used by both the VR app and the Revit plugin
    public static class Configuration
    {
        public static JsonSerializerOptions jsonSerializerOptions { get; } = new JsonSerializerOptions()
        {
            WriteIndented = true,
            IncludeFields = true
        };

        static Configuration()
        {
            jsonSerializerOptions.Converters.Add(new VRBIM_Vector3Converter());
            jsonSerializerOptions.Converters.Add(new VRBIM_Vector3ListConverter());
        }

        public static int invalidElementId = -1;

        public static string protocolPrefix = "ws://";

        public static string uri => protocolPrefix + GetLocalIpAddress(NetworkInterfaceType.Ethernet);

        public static string mainPath = "/main";

        // only to be used by the server as it retrieves the local ip address. 
        public static string GetLocalIpAddress(NetworkInterfaceType _type)
        {
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
            return output;
        }

        public static int temporaryMeshIndex = int.MinValue;
    }
}
