﻿using AugustaHIDCfg.CommonInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AugustaHIDCfg.MainApp
{
    class AppDomainCfg
    {
        /**************************************************************************/
        // APPDOMAIN ARTIFACTS
        /**************************************************************************/
        #region -- appdomain artifacts --

        public AppDomain CreateAppDomain(string dllName)
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

        public IDevicePlugIn InstantiatePlugin(string dllName, AppDomain domain)
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

        public void UnloadPlugin(AppDomain appdomain)
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

        public void TestIfUnloaded(IDevicePlugIn plugin)
        {
            bool unloaded = false;

            try
            {
            Debug.WriteLine(plugin.PluginName);
            }
            catch (AppDomainUnloadedException)
            {
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
    }
}
