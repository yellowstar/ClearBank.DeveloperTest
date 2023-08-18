using System;
using System.Collections.Generic;
using System.Text;

namespace ClearBank.DeveloperTest.Configuration
{
    public class ConfigurationSettings : IConfigurationSettings
    {
        // Populated on startup from suitable configuration settings
        public string DataStoreType { get; set; }
    }
}