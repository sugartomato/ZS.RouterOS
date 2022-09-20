using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;

namespace ZS.RouterOS.Test
{
    [TestClass]
    public class HelperTest
    {
        private ZS.RouterOS.Session m_Session = null;
        public HelperTest()
        {
            m_Session = new ZS.RouterOS.Session()
            {
                HostNameOrIPAddress = "10.90.0.1",
                UserName = "",
                Password = "",
            };
        }


        #region "DHCP"

        [TestMethod]
        public void DHCPServer_Leases_Add()
        {
            ZS.RouterOS.Helper.DHCPServer.Leases_Model mod = new Helper.DHCPServer.Leases_Model()
            {
                Address = new _IPAddress("1.1.1.1"),
                AlwaysBroadcast = new _Disabled(true),
                Disabled = new _Disabled(false),
                MacAddress = new _MACAddress("00:00:00:00:00:01"),
                Comment = @"D5C5B4",

            };

            string exMessage = string.Empty;
            ZS.RouterOS.Helper.Helper.Instance(m_Session).DHCPServer.Leases_Add(mod, ref exMessage);

            Console.WriteLine(exMessage);
        }

        [TestMethod]
        public void DHCPServer_Leases_Print()
        {

            List<string> l = new List<string>();
            l = ZS.RouterOS.Helper.Helper.Instance(m_Session).DHCPServer.Leases_Print();
            Console.Write(l);
        }


        [TestMethod]
        public void DHCPServer_Leases_RemoveAllDynamic()
        {
            string exMessage = string.Empty;
            Boolean blnR = ZS.RouterOS.Helper.Helper.Instance(m_Session).DHCPServer.Leases_RemoveAllDynamic(ref exMessage);

            Console.Write(blnR);
        }

        [TestMethod]
        public void DHCPServer_Lease_ModelToAddScriptString()
        {
            Helper.DHCPServer.Leases_Model model = new Helper.DHCPServer.Leases_Model();
            model.Address = new _IPAddress("10.92.3.2");
            model.MacAddress = new _MACAddress("EC-08-6B-C2-C3-4A ");
            model.Server = "Floor2";
            model.Comment = "203打印服务器。设备固定IP";
            Console.WriteLine(model.ToAddScriptString());
        }

        #endregion

        [TestMethod]
        public void HotSpot_IPBinding_IsIpExists()
        {
            string exMessage = string.Empty;
            ZS.RouterOS._IPAddress ip = new _IPAddress("10.90.0.2");
            Boolean blnR = ZS.RouterOS.Helper.Helper.Instance(m_Session).Hotspot.IPBindings_IsIpExists(ip, ref exMessage);
            Console.Write(blnR);
        }
        
        [TestMethod]
        public void HotSpot_IPBinding_Print()
        {
            List<string> l = new List<string>();
            l = ZS.RouterOS.Helper.Helper.Instance(m_Session).Hotspot.IPBindings_Print();
            Console.Write(l);
        }

        [TestMethod]
        public void MetadataTest()
        {
            ZS.RouterOS.ROSObjects.RMetadata mm = ZS.RouterOS.ROSObjects.RMetadataCache.GetMetadata<ZS.RouterOS.ROSObjects.IP.Address>();

        }


    }
}
