using IDTechSDK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AugustaHIDCfg.CommonInterface
{
    [Serializable]
    public class TerminalData
    {
        /*
            TAG 5F36 - TLV information for "Transaction Currency Exponent"
            TAG 9F1A - "Terminal Country Code"
            TAG 9F35 - "Terminal Type"
            TAG 9F33 - "Terminal Capabilities" 
            TAG 9F40 - "Additional Terminal Capabilities"
            TAG 9F1E - "Interface Device Serial Number"
            TAG 50   - "Tag Application Label"
            TAG 73   - "Directory Discretionary Template"
            TAG 86   - "Issuer Script Command" 
        */
        
        [JsonProperty(PropertyName = "terminal_data", Order = 1)]
        public string terminal_data { get; set; }
        [JsonIgnore]
        private byte [] tlv;

        public TerminalData(byte [] param)
        {
            tlv = param;
        }

        public string ConvertTLVToString()
        {
            byte[] test = tlv.Skip(0).Take(tlv.Length).ToArray();
            terminal_data = BitConverter.ToString(test).Replace("-", string.Empty);
            return terminal_data;
        }

        public string ConvertTLVToValuePairs()
        {
            string text = "";
            try
            {
                Dictionary<string, string> dict = Common.processTLVUnencrypted(tlv);
                Debug.WriteLine("==================== TLV DUMP ====================");
                foreach (KeyValuePair<string, string> kvp in dict)
                {
                    text += kvp.Key + ": " + kvp.Value + "\r\n";
                    Debug.WriteLine("{0} : {1}", kvp.Key , kvp.Value);
                }
            }
            catch(Exception exp)
            {
                Debug.WriteLine("TerminalDatga::ConvertTLVToValuePairs(): - exception={0}", (object)exp.Message);
            }
            return text;
        }
    }
}
