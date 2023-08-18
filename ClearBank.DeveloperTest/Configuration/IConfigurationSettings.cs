using System;
using System.Collections.Generic;
using System.Text;

namespace ClearBank.DeveloperTest.Configuration
{
    public interface IConfigurationSettings
    {
        string DataStoreType { get; set; }
    }
}