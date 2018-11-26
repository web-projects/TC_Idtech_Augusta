using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AugustaHIDCfg.CommonInterface
{
  [Serializable]
  public class DeviceEventArgs : System.EventArgs
  {
    // Provide one or more constructors, as well as fields and
    // accessors for the arguments.
    public object [] payload { get; set; }

    public DeviceEventArgs()
    {
      // Upto 10 Parameters to process
      payload = new object[10];
    }
  }

  public delegate void DeviceEventHandler(object sender, DeviceEventArgs e);

  public interface IDevicePlugIn
  {
    // Device Events back to Main Form
    event DeviceEventHandler initializeDevice;
    event DeviceEventHandler unloadDeviceconfigDomain;
    event DeviceEventHandler enableFormButtons;
    event DeviceEventHandler processCardData;
    event DeviceEventHandler processCardDataError;
    event DeviceEventHandler getDeviceConfiguration;
    event DeviceEventHandler setDeviceConfiguration;
    event DeviceEventHandler setDeviceMode;
    event DeviceEventHandler setExecuteResult;

    // INITIALIZATION
    string PluginName { get; }
    void DeviceInit(IConfigurationWrapper wrapper);
    ConfigSerializer GetConfigSerializer();
    // GUI UPDATE
    string [] GetConfig();
    // NOTIFICATION
    void SetFormClosing(bool state);
    // MSR READER
    void GetCardData();
    // Settings
    void GetDeviceConfiguration();
    // Configuration
    void SetDeviceConfiguration(object data);
    void SetDeviceMode(string mode);
    void DeviceCommand(string command);
  }
}
