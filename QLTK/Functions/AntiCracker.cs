using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using LitJSON;
using LitJson;

namespace QLTK.Functions
{
    internal class AntiCracker
    {
        #region
        static AntiCracker instance;
        public static AntiCracker gI()
        {
            if (instance == null)
            {
                instance = new AntiCracker();
                instance.init();
            }
            return instance;
        }
        #endregion


        const string HOST_IP = "103.200.22.212";
        const string TOOL_NAME = "autotask";

       
        RSACryptoServiceProvider private_key_client;
        RSACryptoServiceProvider public_key_server;

        void init()
        {
            var key = "-----BEGIN RSA PRIVATE KEY-----\r\nMIICXgIBAAKBgQDCpwiv2NTJWSnOWuyHR7B+wA+zugLDsA90yV87AaVpUDCl3Day\r\njbktLkxcxxNcDfn0KYjRcPtQvp1PRRk9u0X8sSoLr1yeIPPyNoloKLxaC2jowwMt\r\nBw8LrE9/EhBTrYrwbxs4HzIa8VUu4hGsX6Tjmglk+CRX0gK97Y6VhfkZuQIDAQAB\r\nAoGBAL242oGywOujaVkFTEcqu78+6c8OfUVIpDf7UkOL/jJgg5oVwoZUFmoQF0PS\r\nkW8G/Fpb5UvmaD2yHFe4DLsbfkDzMKZw12q9TsQy4dU/0rSq8DSqb7ro9rC3UF/Q\r\n7hMdDGbokurv6ai7jBiunDV9Cw2fKUXtznYgPXOdP7DFTJDFAkEA3+kJwMeBpHXd\r\nr4vw2uEGZRJ7BtW2sMVxeCYEOxBzRiK3/owu5E3dg3A5qAn/O1Ms3BjaWKkTSLNt\r\nJ1glWgcaLwJBAN6MkPT1lA6ks0w/w9IQIVm7G5QffnhMaLZumaROeLEuzml5S9Ww\r\n0IXs3DUwags9pxIrf5n6NTx0sBxWQl4f2JcCQDwPOzF9kjW18+fBhrJ/R4LW6S8V\r\nJjLWUzBiKiJZsEHw0B+0JGPSywcVlDGRtOeJ0O+crvy3JVgL4Mx75VD3tLsCQQCD\r\nGBBni6Xcfl5z4vfp7MCIh/R165tnJ7NjC4GLzQHBdY33iZEHXrFmwqDw4AUHfBjH\r\nVkJDJKVpaF85QgK0SVQfAkEAqur26r3JNJ8HDkZHkKq54u/x4RzqensACwEDpS3M\r\nJIyic3Suhj8QtCNzAzKyH5BKshcBZXZFO5pmH92oOnDbEQ==\r\n-----END RSA PRIVATE KEY-----";
            private_key_client = new RSACryptoServiceProvider();
            private_key_client.ImportParameters(DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)((AsymmetricCipherKeyPair)(new PemReader(new StringReader(key))).ReadObject()).Private));

            key = "-----BEGIN PUBLIC KEY-----\r\nMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCvd/A3ZM7rZ+vCR2nRx33amPrj\r\n1IHxIb5yxzoLVbI03JjhaUR+xelYtn6hwDVhptshIr1OGT1jpqY8CnAde8MG3LTe\r\n47xrMPN7MHO/+UlYzvEXou2xDwZT9zxzvUFC85Ooqwz49sGPyPjJe8iN97KhKKTJ\r\nPyxMh3e8us4zEqPQoQIDAQAB\r\n-----END PUBLIC KEY-----";
            public_key_server = new RSACryptoServiceProvider();
            public_key_server.ImportParameters(DotNetUtilities.ToRSAParameters((RsaKeyParameters)((AsymmetricKeyParameter)new PemReader(new StringReader(key)).ReadObject())));
        }

        internal bool check_key_license()
        {
            if (!IsConnectedToInternet())
            {
                MessageBox.Show("Không có mạng dùng tool kiểu gì :v", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            if (check_host_ip(HOST_IP))
            {
                MessageBox.Show("Crack kon kặk =))", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            try
            {
                using (WebClient w = new WebClient())
                {

                    var dataUp = encrypt(JsonMapper.ToJson(new
                    {
                        name = TOOL_NAME,
                        licensekey = GetRequestLicenseCode()
                    }));
                    var respone = w.UploadValues("http://licensekey.123tool.pro/", "POST", new NameValueCollection()
                    {
                        { "data", dataUp }
                    });
                    var r = Encoding.UTF8.GetString(respone);
                    r = decrypt(r);
                    if (r == "no")
                    {
                        MessageBox.Show("Key không tồn tại", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    var data = JsonMapper.ToObject(r);
                    var time_out = TimeHelper.gI().CheckTimeOut((string)data["check_time"]);
                    if (time_out > 5000)
                    {
                        Utils.notification("Time out!", MessageBoxIcon.Error);
                        return false;
                    }
                    mainForm.gI.time_expired = TimeHelper.gI().DateTimeFromString((string)data["time_expired"]);
                    //mainForm.gI.lbVersion.Text = (string)data["time_expired"];

                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public string decrypt(string str)
        {
            return Encoding.UTF8.GetString(private_key_client.Decrypt(Convert.FromBase64String(str), false));
        }
        public string encrypt(string str)
        {
            return Convert.ToBase64String(public_key_server.Encrypt(Encoding.UTF8.GetBytes(str), false));
        }

        string GetMacAddress()
        {
            NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            for (int i = 0; i < allNetworkInterfaces.Length; i++)
            {
                PhysicalAddress physicalAddress = allNetworkInterfaces[i].GetPhysicalAddress();
                bool flag = physicalAddress.ToString() != string.Empty;
                bool flag2 = flag;
                if (flag2)
                {
                    return physicalAddress.ToString();
                }
            }
            return string.Empty;
        }

        string GetMD5(string txt)
        {
            string text = "";
            byte[] array = Encoding.UTF8.GetBytes(txt);
            array = new MD5CryptoServiceProvider().ComputeHash(array);
            foreach (byte b in array)
            {
                text += b.ToString("X2");
            }
            return text;
        }

        internal string GetRequestLicenseCode()
        {
            return GetMD5(GetMacAddress() + "TranVinh");
        }

        bool check_host_ip(string host)
        {
            bool result = true;
            using (Ping p = new Ping())
            {
                try
                {
                    var pr = p.Send(host, 2000);
                    if (pr.Status == IPStatus.Success && pr.Address.ToString() == HOST_IP && pr.Address.MapToIPv4().ToString() == HOST_IP)
                    {
                        result = false;
                    }
                }
                catch (Exception)
                {
                    result = true;
                }
            }
            return result;
        }

        [Flags] 
        enum ConnectionInternetState : int 
        {
            INTERNET_CONNECTION_MODEM = 0x1,
            INTERNET_CONNECTION_LAN = 0x2,
            INTERNET_CONNECTION_PROXY = 0x4,
            INTERNET_RAS_INSTALLED = 0x10,
            INTERNET_CONNECTION_OFFLINE = 0x20,
            INTERNET_CONNECTION_CONFIGURED = 0x40 
        }
        [DllImport("wininet.dll", CharSet = CharSet.Auto)] 
        static extern bool InternetGetConnectedState(ref ConnectionInternetState lpdwFlags, int dwReserved);

        public static bool IsConnectedToInternet()
        {
            try
            {
                ConnectionInternetState Description = 0;
                bool conn = InternetGetConnectedState(ref Description, 0);
                return conn;
            }
            catch
            {
                return false;
            }
        }
    }
}
