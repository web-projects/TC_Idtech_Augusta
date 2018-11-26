﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AugustaHIDCfg.CommonInterface
{
    public interface IConfigurationWrapper
    {
        bool HasKey(string key);
        string GetAppSetting(string key);
    }
}
