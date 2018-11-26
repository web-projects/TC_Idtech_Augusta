using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AugustaHIDCfg.DeviceConfiguration
{
  public enum DeviceManufacturer
  {
      Unknown = 0,
      IDTech = 1,
      Ingenico = 2
  }

  public enum IDTECH_DevicePID
  {
    AUGUSTA_KYB = 3810,
    AUGUSTA_USB = 3820,
  }

 public enum FuncID
 {
  DefaultConfig = 0x18,  
  MSR = 0x1A,            // 0131 - Enable MSR
  DecodingMethod = 0x1D, // 0131 - Decode in both direction
  USBHIDFmtID = 0x23,    // 0130 - HSB HID ID Tech format, 0138 - USB-HID-KB 
  KeyType = 0x3E,        // 0100 - data key
  EncrytionType = 0x4C,  // 0131 - TDES
  EncryptStr = 0x85,     // 0131 - enhanced (for SecureKey device it only applies > FW v1.14)
  KeyedOptions = 0x8F,   // 0101 - enhanced format
  PrePANID = 0x49,       // 0104: leading 4 digits to display
  PostPANID = 0x4A,      // 0104: trailing 4 digits to display
  SerialNumber = 0x4E,   // 10-bytes serial number starting at byte 3 from the response bytes
  DeviceFormat = 0x77,   // for IDT/XML format values
  SecurityLevel = 0x7E   // Reader's encryption level - 1: no encryption, 2: key loaded, 3: encrypted reader.
 }

  public static class CommandTokens
  {
      public static byte[] SetDefaultConfig      = { 0x02, 0x53, 0x18, 0x03 };
      public static byte[] ReadConfiguration     = { 0x02, 0x52, 0x1F, 0x03 };
      public static byte[] ReadFirmwareVersion   = { 0x02, 0x52, 0x22, 0x03 };
      public static byte[] GetSerialNumber       = { 0x02, 0x52, 0x4E, 0x03 };
      public static byte[] DeviceReset           = { 0x02, 0x46, 0x49, 0x03 };
      public static byte[] SetKeyedInOption      = { 0x02, 0x53, 0x8F, 0x01, 0x00, 0x03 };
      public static byte[] SetKeyedInCVV         = { 0x02, 0x53, 0x8F, 0x01, 0x02, 0x03 };
      public static byte[] EnableAdminKey        = { 0x02, 0x30, 0x8F, 0x01, 0x20, 0x03 };
      public static byte[] DisableAdminKey       = { 0x02, 0x31, 0x8F, 0x01, 0x20, 0x03 };
      public static byte[] SetUSBHIDMode         = { 0x02, 0x53, 0x23, 0x01, 0x30, 0x03 };
      public static byte[] SetTDES               = { 0x02, 0x53, 0x4C, 0x01, 0x31, 0x03 };
      public static byte[] SetKeyedOption        = { 0x02, 0x53, 0x8F, 0x01, 0x01, 0x03 };
      public static byte[] SetPANMask            = { 0x02, 0x53, 0x49, 0x01, 0x06, 0x03 };
  }

  public enum Token
  {
      STK = 0x02,
      ETK = 0X03,
      R   = 0X52,
      S   = 0X53,
      ACK = 0x06,
      NAK = 0X15
  }

  public enum EntryModeStatus
  {
      [System.ComponentModel.Description("Payment canceled by customer.")]
      Canceled = 150,
      [System.ComponentModel.Description("Signature was not captured. Signature was canceled at device.")]
      SignatureCanceled = 151,
      [System.ComponentModel.Description("Error reading card. Maximum number of swipes or inserts has been reached. Transaction canceled.")]
      CardNotRead = 155,
      [System.ComponentModel.Description("Device has timed out due to inactivity.")]
      Timeout = 160,
      [System.ComponentModel.Description("Do not need")]
      Error = 165,
      [System.ComponentModel.Description("Card Blocked, please call card issuer for assistance. Swipe another card.")]
      cardblocked = 166,
      [System.ComponentModel.Description("This card is not supported, please provide another card.")]
      Unsupported = 170,
      [System.ComponentModel.Description("DAL no DAL")]
      NoTCIPADAL = 175,
      [System.ComponentModel.Description("Device not found.")]
      NoDevice = 177,
      [System.ComponentModel.Description("Error encountered during pin entry.")]
      ErrorPinEntry = 178,
      [System.ComponentModel.Description("Pin try limit exceeded.")]
      PinEntryExceed = 179,
      [System.ComponentModel.Description("Successful")]
      Success = 180,
      [System.ComponentModel.Description("User retry the card.")]
      Retry = 1000,
      [System.ComponentModel.Description("Not able to process request. The Cust ID used has no access to the ServiceURL reached, please contact your Administrator.")]
      InvalidServiceURL,
      [System.ComponentModel.Description("Multiple devices connected.")]
      MultipleDevice,
      [System.ComponentModel.Description("DAL not ready.")]
      NotReady = 1003
  }

  public enum ConfigTypeEnum
  {
    MsrTimeout = 100
  }

  public enum TimerType
   {
     [Description("MsrTimeout")]
     MSR = ConfigTypeEnum.MsrTimeout
  }

  public static class FeatureResponses
  {
      public static byte[] USBHIDResponse = { 0x06, 0x02, 0x23, 0x01, 0x30, 0x03 };
      public static byte[] USBKBResponse  = { 0x06, 0x02, 0x23, 0x01, 0x38, 0x03 };
  }

  class DeviceData
  {
  }

  public enum SecureKeyModelFormat
  {
      M100IDT = 0x01,
      M130IDT = 0x05,
      M100XML = 0x09,
      M130XML = 0x0D
  }

  public class DeviceVersion
  {
      public const double V100 = 1.00;
      public const double V114 = 1.14;
      public const double V126 = 1.26;
      public const double V130 = 1.30;
  }

  public class DeviceModelType
  {
      public const string SecureKey = "ID TECH TM3 SecureKey";
      public const string SecureMag = "ID TECH TM3 SecureMag";
      public const string SRedKey = "ID TECH SREDKey";
      public const string SecuRED = "ID TECH TM3 SecuRED";
  }

  public class DeviceModelNumber
  {
      public const string SRedKey = "IDSK-534833TEB";
      public const string SecureKeyM100Xml = "IDKE-504800BL";
      public const string SecureKeyM100Enhanced = "IDKE-504800BM";
      public const string SecureKeyM130Xml = "IDKE-534833BL";
      public const string SecureKeyM130Enhanced = "IDKE-534833BE";
      public const string SecureKeyM130NewFormat = "IDKE-534833BEM";
      public const string SecureMag = "IDRE-33X133B";
      public const string SecuRED = "IDSR-334133TEB";
  }
}
