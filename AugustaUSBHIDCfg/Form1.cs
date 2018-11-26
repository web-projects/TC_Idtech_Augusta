﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HidLibrary;

// Include DeviceConfiguration.dll in the References list
using AugustaHIDCfg.DeviceConfiguration;
using AugustaHIDCfg.CommonInterface;
using System.Collections;

namespace AugustaHIDCfg.MainApp
{
  public partial class Application : Form
  {
    public Panel appPnl;

    private bool formClosing = false;

    public static Button readMSRButton;
    public static Button getConfigButton;

    // AppDomain Artifacts
    private AppDomain appDomainDevice;
    private IDevicePlugIn devicePlugin;
    private const string MODULE_NAME = "DeviceConfiguration";

    public Application()
    {
        InitializeComponent();

        this.Text = "IDTECH Device Discovery Application";

        readMSRButton = this.btnCardRead;

        // Disable User Button(s)
        readMSRButton.Enabled = false;

        // Initialize Device
        InitalizeDevice();
    }

    /********************************************************************************************************/
    // FORMS ELEMENTS
    /********************************************************************************************************/
    #region -- forms elements --

    private void txtPAN_TextChanged(object sender, EventArgs e)
    {
        // Validate INPUT
    }

    #endregion

    /********************************************************************************************************/
    // DELEGATES SECTION
    /********************************************************************************************************/
    #region -- delegates section --

    private void InitalizeDeviceUI(object sender, DeviceEventArgs e)
    {
        InitalizeDevice(true);
    }

    private void EnableFormButtonsUI(object sender, DeviceEventArgs e)
    {
        new Thread(() =>
        {
        Thread.CurrentThread.IsBackground = true;
        EnableFormButtons();
        }).Start();
    }

    private void ProcessCardDataUI(object sender, DeviceEventArgs e)
    {
        ProcessCardData(e.payload[0]);
    }

    private void ProcessCardDataErrorUI(object sender, DeviceEventArgs e)
    {
        ProcessCardDataError(e.payload[0]);
    }

    private void UnloadDeviceConfigurationDomain(object sender, DeviceEventArgs e)
    {
        new Thread(() =>
        {
        Thread.CurrentThread.IsBackground = true;

        ClearUI();

        // Unload The Plugin
        UnloadPlugin(appDomainDevice);

        // wait for a new device to connect
        WaitForDeviceToConnect();

        }).Start();
    }

    private void GetDeviceConfigurationUI(object sender, DeviceEventArgs e)
    {
        GetDeviceConfiguration(e.payload);
    }

    private void SetDeviceConfigurationUI(object sender, DeviceEventArgs e)
    {
        SetDeviceConfiguration(e.payload);
    }

    private void SetDeviceModeUI(object sender, DeviceEventArgs e)
    {
        SetDeviceMode(e.payload);
    }

    private void SetExecuteResultUI(object sender, DeviceEventArgs e)
    {
        SetExecuteResult(e.payload);
    }

    #endregion

    /********************************************************************************************************/
    // GUI - DELEGATE SECTION
    /********************************************************************************************************/
    #region -- gui delegate section --

    private void ClearUI()
    {
        if (InvokeRequired)
        {
        MethodInvoker Callback = new MethodInvoker(ClearUI);
        Invoke(Callback);
        }
        else
        {
            this.lblSerialNumber.Text = "";
            this.lblFirmwareVersion.Text = "";
            this.lblModelName.Text = "";
            this.lblModelNumber.Text = "";
            this.lblPort.Text = "";
            this.txtCardData.Text = "";

            // Disable Buttons
            readMSRButton.Enabled = false;

            // Disable Tab(s)
            this.tabPage1.Enabled = false;
            this.tabPage2.Enabled = false;
            this.tabPage3.Enabled = false;
            this.tabPage4.Enabled = false;
        }
    }

    private void UpdateUI()
    {
        if (InvokeRequired)
        {
        MethodInvoker Callback = new MethodInvoker(UpdateUI);
        Invoke(Callback);
        }
        else
        {
        SetConfiguration();
        }
    }

    public static void EnableFormButtons()
    {
        if (null == readMSRButton)
        {
        return;
        }

        // Make this threadsafe:
        if (readMSRButton.InvokeRequired)
        {
        readMSRButton.Invoke(new MethodInvoker(() =>
        {
            EnableFormButtons();
        }));
        }
        else
        {
        readMSRButton.Enabled = true;
        }
    }

    #endregion

    /********************************************************************************************************/
    // DEVICE ARTIFACTS
    /********************************************************************************************************/
    #region -- device artifacts --

    private void SetConfiguration()
    {
        Debug.WriteLine("\nmain: update GUI elements =========================================================");

        this.lblSerialNumber.Text = "";
        this.lblFirmwareVersion.Text = "";
        this.lblModelName.Text = "";
        this.lblModelNumber.Text = "";
        this.lblPort.Text = "";
        this.txtCardData.Text = "";

        string[] config = devicePlugin.GetConfig();

        if (config != null)
        {
        this.lblSerialNumber.Text = config[0];
        this.lblFirmwareVersion.Text = config[1];
        this.lblModelName.Text = config[2];
        this.lblModelNumber.Text = config[3];
        this.lblPort.Text = config[4];
        }

        // Enable Buttons
        readMSRButton.Enabled = true;

        // Enable Tab(s)
        this.tabPage1.Enabled = true;
        this.tabPage2.Enabled = true;
        this.tabPage3.Enabled = true;
        this.tabPage4.Enabled = true;
    }

    private void InitalizeDevice(bool unload = false)
    {
        // Unload Domain
        if (unload)
        {
            UnloadPlugin(appDomainDevice);

            // Test Unload
            TestIfUnloaded(devicePlugin);
        }

        // AppDomain Interface
        appDomainDevice = CreateAppDomain(MODULE_NAME);

        // Load Interface
        devicePlugin = InstantiatePlugin(MODULE_NAME, appDomainDevice);

        // Initialize interface
        if (devicePlugin != null)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Debug.WriteLine("\nmain: new device detected! +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n");

                // Disable Tab(s)
                this.tabPage1.Enabled = false;
                this.tabPage2.Enabled = false;
                this.tabPage3.Enabled = false;
                this.tabPage4.Enabled = false;

                // Setup DeviceCfg Event Handlers
                devicePlugin.initializeDevice += new DeviceEventHandler(this.InitalizeDeviceUI);
                devicePlugin.enableFormButtons += new DeviceEventHandler(this.EnableFormButtonsUI);
                devicePlugin.unloadDeviceconfigDomain += new DeviceEventHandler(this.UnloadDeviceConfigurationDomain);
                devicePlugin.processCardData += new DeviceEventHandler(this.ProcessCardDataUI);
                devicePlugin.processCardDataError += new DeviceEventHandler(this.ProcessCardDataErrorUI);
                devicePlugin.getDeviceConfiguration += new DeviceEventHandler(this.GetDeviceConfigurationUI);
                devicePlugin.setDeviceConfiguration += new DeviceEventHandler(this.SetDeviceConfigurationUI);
                devicePlugin.setDeviceMode += new DeviceEventHandler(this.SetDeviceModeUI);
                devicePlugin.setExecuteResult += new DeviceEventHandler(this.SetExecuteResultUI);

                // Initialize Device
                devicePlugin.DeviceInit(new AppConfigWrapper());
                Debug.WriteLine("main: loaded plugin={0} ++++++++++++++++++++++++++++++++++++++++++++", (object)devicePlugin.PluginName);

                UpdateUI();

            }).Start();
        }
    }

    private void WaitForDeviceToConnect()
    {
        // Wait for a new device to connect
        new Thread(() =>
        {
        bool foundit = false;
        Thread.CurrentThread.IsBackground = true;

        Debug.Write("Waiting for new device to connect");

        // Wait for a device to attach
        while (!formClosing && !foundit)
        {
            HidDevice device = HidDevices.Enumerate(DeviceCfg.IDTechVendorID).FirstOrDefault();

            if (device != null)
            {
            foundit = true;
            device.CloseDevice();
            }
            else
            {
            Debug.Write(".");
            Thread.Sleep(1000);
            }
        }

        // Initialize Device
        if (!formClosing && foundit)
        {
            Debug.WriteLine("found one!");

            Thread.Sleep(3000);

            // Initialize Device
            InitalizeDeviceUI(this, new DeviceEventArgs());
        }

        }).Start();
    }

    private void ProcessCardData(object payload)
    {
        // Invoker with Parameter(s)
        MethodInvoker mi = () =>
        {
        try
        {
            //string [] data = ((IEnumerable) payload).Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray();
            //this.txtCardData.Text = string.Join("", data[0].ToString());
            this.txtCardData.Text = payload.ToString();
            this.btnCardRead.Enabled = true;

            // Enable Tab(s)
            this.tabPage1.Enabled = true;
            this.tabPage2.Enabled = true;
            this.tabPage3.Enabled = true;
            this.tabPage4.Enabled = true;
        }
        catch (Exception exp)
        {
            Debug.WriteLine("main: ProcessCardData() - exception={0}", (object)exp.Message);
        }
        };

        if (InvokeRequired)
        {
        BeginInvoke(mi);
        }
        else
        {
        Invoke(mi);
        }
    }
    
    private void ProcessCardDataError(object payload)
    {
        // Invoker with Parameter(s)
        MethodInvoker mi = () =>
        {
        string [] data = ((IEnumerable) payload).Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray();
        this.txtCardData.Text = data[0];
        this.btnCardRead.Enabled = true;
        };

        if (InvokeRequired)
        {
        BeginInvoke(mi);
        }
        else
        {
        Invoke(mi);
        }
    }

    private void CardRead_Click(object sender, EventArgs e)
    {
        // Disable Tab(s)
        this.tabPage1.Enabled = false;
        this.tabPage2.Enabled = false;
        this.tabPage3.Enabled = false;
        this.tabPage4.Enabled = false;

        readMSRButton.Enabled = false;

        // Clear field
        this.txtCardData.Text = "";

        // Set Focus to reader input
        this.txtCardData.Focus();

        // MSR Read
        new Thread(() =>
        {
        Thread.CurrentThread.IsBackground = true;
        devicePlugin.GetCardData();
        }).Start();
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        formClosing = true;

        if (devicePlugin != null)
        {
        devicePlugin.SetFormClosing(formClosing);
        }
    }

    private void GetDeviceConfiguration(object payload)
    {
        // Invoker with Parameter(s)
        MethodInvoker mi = () =>
        {
        try
        {
            this.btnReadConfig.Enabled = true;

            string [] data = ((IEnumerable) payload).Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray();

            this.lblExpMask.Text    = data[0];
            this.lblPanDigits.Text  = data[1];
            this.lblSwipeForce.Text = data[2];
            this.lblSwipeMask.Text  = data[3];
            this.lblMsrSetting.Text = data[4];

            // Enable Tabs
            this.tabPage1.Enabled = true;
            this.tabPage2.Enabled = true;
            this.tabPage3.Enabled = true;
            this.tabPage4.Enabled = true;
        }
        catch (Exception exp)
        {
            Debug.WriteLine("main: GetDeviceConfiguration() - exception={0}", (object)exp.Message);
        }
        };

        if (InvokeRequired)
        {
        BeginInvoke(mi);
        }
        else
        {
        Invoke(mi);
        }
    }

    private void SetDeviceConfiguration(object payload)
    {
        // Invoker with Parameter(s)
        MethodInvoker mi = () =>
        {
        try
        {
            // update settings in panel
            string [] data = ((IEnumerable) payload).Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray();

            // Expiration Mask
            this.cBxExpirationMask.Checked = data[0].Equals("Masked", StringComparison.OrdinalIgnoreCase) ? true : false;

            // PAN Clear Digits
            this.txtPAN.Text = data[1];

            // Swipe Force Mask
            string [] values = data[2].Split(',');

            // Process Individual values
            string [] track1 = values[0].Split(':');
            string [] track2 = values[1].Split(':');
            string [] track3 = values[2].Split(':');
            string [] track3Card0 = values[3].Split(':');

            string t1Value = track1[1].Trim();
            string t2Value = track2[1].Trim();
            string t3Value = track3[1].Trim();
            string t3Card0Value = track3Card0[1].Trim();

            bool t1Val = t1Value.Equals("ON", StringComparison.OrdinalIgnoreCase) ? true : false;
            bool t2Val = t2Value.Equals("ON", StringComparison.OrdinalIgnoreCase) ? true : false;
            bool t3Val = t3Value.Equals("ON", StringComparison.OrdinalIgnoreCase) ? true : false;
            bool t3Card0Val = t3Card0Value.Equals("ON", StringComparison.OrdinalIgnoreCase) ? true : false;

            // Compare to existing values
            if(this.cBxTrack1.Checked != t1Val) {
            this.cBxTrack1.Checked = t1Val;
            }

            if(this.cBxTrack2.Checked != t2Val) {
            this.cBxTrack2.Checked = t2Val;
            }

            if(this.cBxTrack3.Checked != t3Val) {
            this.cBxTrack3.Checked = t3Val;
            }

            if(this.cBxTrack3Card0.Checked != t3Card0Val) {
            this.cBxTrack3Card0.Checked = t3Card0Val;
            }

            // Swipe Mask
            values = data[3].Split(',');

            // Process Individual values
            track1 = values[0].Split(':');
            track2 = values[1].Split(':');
            track3 = values[2].Split(':');

            t1Value = track1[1].Trim();
            t2Value = track2[1].Trim();
            t3Value = track3[1].Trim();

            t1Val = t1Value.Equals("ON", StringComparison.OrdinalIgnoreCase) ? true : false;
            t2Val = t2Value.Equals("ON", StringComparison.OrdinalIgnoreCase) ? true : false;
            t3Val = t3Value.Equals("ON", StringComparison.OrdinalIgnoreCase) ? true : false;

            // Compare to existing values
            if(this.cBxSwipeMaskTrack1.Checked != t1Val) {
            this.cBxSwipeMaskTrack1.Checked = t1Val;
            }

            if(this.cBxSwipeMaskTrack2.Checked != t2Val) {
            this.cBxSwipeMaskTrack2.Checked = t2Val;
            }

            if(this.cBxSwipeMaskTrack3.Checked != t3Val) {
            this.cBxSwipeMaskTrack3.Checked = t3Val;
            }

            // Enable Button
            this.btnReadConfig.Enabled = true;

            // Enable Tabs
            this.tabPage1.Enabled = true;
            this.tabPage2.Enabled = true;
            this.tabPage3.Enabled = true;
            this.tabPage4.Enabled = true;
        }
        catch (Exception exp)
        {
            Debug.WriteLine("main: SetDeviceConfiguration() - exception={0}", (object)exp.Message);
        }
        };

        if (InvokeRequired)
        {
        BeginInvoke(mi);
        }
        else
        {
        Invoke(mi);
        }
    }

    private void SetDeviceMode(object payload)
    {
        // Invoker with Parameter(s)
        MethodInvoker mi = () =>
        {
            try
            {
                string [] data = ((IEnumerable) payload).Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray();
                this.btnMode.Text = data[0];
                this.btnMode.Visible = true;
            }
            catch (Exception exp)
            {
                Debug.WriteLine("main: SetDeviceConfiguration() - exception={0}", (object)exp.Message);
            }
        };

        if (InvokeRequired)
        {
            BeginInvoke(mi);
        }
        else
        {
            Invoke(mi);
        }
    }

    private void SetExecuteResult(object payload)
    {
        // Invoker with Parameter(s)
        MethodInvoker mi = () =>
        {
            try
            {
                string [] data = ((IEnumerable) payload).Cast<object>().Select(x => x == null ? "" : x.ToString()).ToArray();
                this.txtCommandResult.Text = "RESPONSE: [" + data[0] + "]";
                this.btnExecute.Enabled = true;
            }
            catch (Exception exp)
            {
                Debug.WriteLine("main: SetDeviceConfiguration() - exception={0}", (object)exp.Message);
            }
        };

        if (InvokeRequired)
        {
            BeginInvoke(mi);
        }
        else
        {
            Invoke(mi);
        }
    }

    #endregion

    /**************************************************************************/
    // APPDOMAIN ARTIFACTS
    /**************************************************************************/
    #region -- appdomain artifacts --

    private AppDomain CreateAppDomain(string dllName)
    {
        AppDomainSetup setup = new AppDomainSetup()
        {
            ApplicationName = dllName,
            ConfigurationFile = dllName + ".dll.config",
            ApplicationBase = AppDomain.CurrentDomain.BaseDirectory
        };

        AppDomain appDomain = AppDomain.CreateDomain(setup.ApplicationName,
                                                    AppDomain.CurrentDomain.Evidence,
                                                    setup);

        // Share App.Config file with all assemblies
        string configFile = System.Reflection.Assembly.GetExecutingAssembly().Location + ".config";
        appDomain.SetData("APP_CONFIG_FILE", configFile);

        return appDomain;
    }

    private IDevicePlugIn InstantiatePlugin(string dllName, AppDomain domain)
    {
        IDevicePlugIn plugIn = null;

        string PLUGIN_NAME = " AugustaHIDCfg." + dllName + ".DeviceCfg";

        try
        {
            plugIn = domain.CreateInstanceAndUnwrap(dllName, PLUGIN_NAME) as IDevicePlugIn;
        }
        catch (Exception e)
        {
            Debug.WriteLine("InsantiatePlugin: exception={0}", (object)e.Message);
        }

        return plugIn;
    }

    private void UnloadPlugin(AppDomain appdomain)
    {
        bool unloaded = false;

        try
        {
        AppDomain.Unload(appdomain);
        unloaded = true;
        }
        catch (CannotUnloadAppDomainException)
        {
        unloaded = true;
        }
        catch (Exception ex)
        {
        Debug.WriteLine(ex.Message);
        }

        if (!unloaded)
        {
        Debug.WriteLine("main: appdomain could not be unloaded.");
        }
    }

    private void TestIfUnloaded(IDevicePlugIn plugin)
    {
        bool unloaded = false;

        try
        {
        Debug.WriteLine(plugin.PluginName);
        }
        catch (AppDomainUnloadedException)
        {
        unloaded = true;
        }
        catch (Exception ex)
        {
        Debug.WriteLine(ex.Message);
        }

        if (!unloaded)
        {
        Debug.WriteLine("It does not appear that the app domain successfully unloaded.");
        }
    }
    #endregion

    /**************************************************************************/
    // SETTINGS TAB
    /**************************************************************************/
    #region -- settings tab --

    private void btnReadConfig_Click(object sender, EventArgs e)
    {
        btnReadConfig.Enabled = false;

        // Clear fields
        this.lblExpMask.Text = "";
        this.lblPanDigits.Text = "";
        this.lblSwipeForce.Text = "";
        this.lblSwipeMask.Text = "";
        this.lblMsrSetting.Text = "";

        // Disable Tabs
        this.tabPage1.Enabled = false;
        this.tabPage2.Enabled = false;
        this.tabPage3.Enabled = false;
        this.tabPage4.Enabled = false;

        // Settings Read
        new Thread(() =>
        {
        Thread.CurrentThread.IsBackground = true;

        try
        {
            devicePlugin.GetDeviceConfiguration();
        }
        catch (Exception exp)
        {
            Debug.WriteLine("main: btnReadConfig_Click() - exception={0}", (object)exp.Message);
        }
        }).Start();
    }

    #endregion

    /**************************************************************************/
    // CONFIGURATION TAB
    /**************************************************************************/
    #region -- configuration tab --

    public List<MsrConfigItem> configExpirationMask;
    public List<MsrConfigItem> configPanDigits;
    public List<MsrConfigItem> configSwipeForceEncryption;
    public List<MsrConfigItem> configSwipeMask;

    private void btnConfigure_Click(object sender, EventArgs e)
    {
        // Disable Tabs
        this.tabPage1.Enabled = false;
        this.tabPage2.Enabled = false;
        this.tabPage3.Enabled = false;
        this.tabPage4.Enabled = false;

        // EXPIRATION MASK
        configExpirationMask = new List<MsrConfigItem>
        {
        { new MsrConfigItem() { Name="expirationmask", Id=(int)EXPIRATION_MASK.MASK, Value=string.Format("{0}", this.cBxExpirationMask.Checked.ToString()) }},
        };

        // PAN DIGITS
        configPanDigits = new List<MsrConfigItem>
        {
        { new MsrConfigItem() { Name="digits", Id=(int)PAN_DIGITS.DIGITS, Value=string.Format("{0}", this.txtPAN.Text) }},
        };

        // SWIPE FORCE
        configSwipeForceEncryption = new List<MsrConfigItem>
        {
        { new MsrConfigItem() { Name="track1",      Id=(int)SWIPE_FORCE_ENCRYPTION.TRACK1, Value=string.Format("{0}",      this.cBxTrack1.Checked.ToString()) }},
        { new MsrConfigItem() { Name="track2",      Id=(int)SWIPE_FORCE_ENCRYPTION.TRACK2, Value=string.Format("{0}",      this.cBxTrack2.Checked.ToString()) }},
        { new MsrConfigItem() { Name="track3",      Id=(int)SWIPE_FORCE_ENCRYPTION.TRACK3, Value=string.Format("{0}",      this.cBxTrack3.Checked.ToString()) }},
        { new MsrConfigItem() { Name="track3Card0", Id=(int)SWIPE_FORCE_ENCRYPTION.TRACK3CARD0, Value=string.Format("{0}", this.cBxTrack3Card0.Checked.ToString()) }}
        };

        // SWIPE MASK
        configSwipeMask = new List<MsrConfigItem>
        {
        { new MsrConfigItem() { Name="track1", Id=(int)SWIPE_MASK.TRACK1, Value=string.Format("{0}", this.cBxSwipeMaskTrack1.Checked.ToString()) }},
        { new MsrConfigItem() { Name="track2", Id=(int)SWIPE_MASK.TRACK2, Value=string.Format("{0}", this.cBxSwipeMaskTrack2.Checked.ToString()) }},
        { new MsrConfigItem() { Name="track3", Id=(int)SWIPE_MASK.TRACK3, Value=string.Format("{0}", this.cBxSwipeMaskTrack3.Checked.ToString()) }}
        };

        // Build Payload Package
        object payload = new object[4] { configExpirationMask, configPanDigits, configSwipeForceEncryption, configSwipeMask };

        // Save to Configuration File
        if(e != null)
        {
        SaveConfiguration();
        }

        // Settings Read
        new Thread(() => SetDeviceConfig(devicePlugin, payload)).Start();
    }

    public static void SetDeviceConfig(IDevicePlugIn devicePlugin, object payload)
    {
        try
        {
            // Make call to DeviceCfg
            devicePlugin.SetDeviceConfiguration(payload);
        }
        catch (Exception exp)
        {
            Debug.WriteLine("main: SetDeviceConfiguration() - exception={0}", (object)exp.Message);
        }
    }

    private void OnConfigurationControlActive(object sender, EventArgs e)
    {
        ConfigSerializer serializer = devicePlugin.GetConfigSerializer();

        // Update settings
        if(serializer != null)
        {
            // EXPIRATION MASK
            this.cBxExpirationMask.Checked = serializer?.user_configuration?.expiration_masking?? false;

            // PAN DIGITS
            this.txtPAN.Text = serializer?.user_configuration?.pan_clear_digits.ToString();

            // SWIPE FORCE
            this.cBxTrack1.Checked = serializer?.user_configuration?.swipe_force_mask.track1?? false;
            this.cBxTrack2.Checked = serializer?.user_configuration?.swipe_force_mask.track2?? false;
            this.cBxTrack3.Checked = serializer?.user_configuration?.swipe_force_mask.track3?? false;
            this.cBxTrack3Card0.Checked = serializer?.user_configuration?.swipe_force_mask.track3card0?? false;

            // SWIPE MASK
            this.cBxSwipeMaskTrack1.Checked = serializer?.user_configuration?.swipe_mask.track1?? false;
            this.cBxSwipeMaskTrack2.Checked = serializer?.user_configuration?.swipe_mask.track2?? false;
            this.cBxSwipeMaskTrack3.Checked = serializer?.user_configuration?.swipe_mask.track3?? false;

            // Invoker without Parameter(s)
            this.Invoke((MethodInvoker)delegate()
            {
                this.btnConfigure_Click(this, null);
            });
        }
    }

    private void SaveConfiguration()
    {
        try
        {
            ConfigSerializer serializer = devicePlugin.GetConfigSerializer();

            // Update Configuration File
            if(serializer != null)
            {
                // Update Data: EXPIRATION MASKING
                serializer.user_configuration.expiration_masking = this.cBxExpirationMask.Checked;
                // PAN Clear Digits
                serializer.user_configuration.pan_clear_digits = Convert.ToInt32(this.txtPAN.Text);
                // Swipe Force Mask
                serializer.user_configuration.swipe_force_mask.track1 = this.cBxTrack1.Checked;
                serializer.user_configuration.swipe_force_mask.track2 = this.cBxTrack2.Checked;
                serializer.user_configuration.swipe_force_mask.track3 = this.cBxTrack3.Checked;
                serializer.user_configuration.swipe_force_mask.track3card0 = this.cBxTrack3Card0.Checked;
                // Swipe Mask
                serializer.user_configuration.swipe_mask.track1 = this.cBxSwipeMaskTrack1.Checked;
                serializer.user_configuration.swipe_mask.track2 = this.cBxSwipeMaskTrack2.Checked;
                serializer.user_configuration.swipe_mask.track3 = this.cBxSwipeMaskTrack3.Checked;

                // WRITE to Config
                serializer.WriteConfig();
            }
        }
        catch (Exception exp)
        {
            Debug.WriteLine("main: SaveConfiguration() - exception={0}", (object)exp.Message);
        }
    }

    private void btnMode_Click(object sender, EventArgs e)
    {
        string mode = this.btnMode.Text;

        new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = true;
            devicePlugin.SetDeviceMode(mode);
        }).Start();

        // Hide MODE Button
        this.btnMode.Visible = false;
    }

    private void OnTextChanged(object sender, EventArgs e)
    {
        if(this.txtCommand.Text.Length > 5)
        {
            this.btnExecute.Visible = true;
        }
        else
        {
            this.btnExecute.Visible = false;
        }
    }

    private void button1_Click(object sender, EventArgs e)
    {
        string command = this.txtCommand.Text;

        new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = true;
            devicePlugin.DeviceCommand(command);
        }).Start();

        this.btnExecute.Enabled = false;
    }

    #endregion
    }
}