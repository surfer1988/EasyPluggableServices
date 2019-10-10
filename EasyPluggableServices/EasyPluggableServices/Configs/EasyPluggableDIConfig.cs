using System;
using System.Collections.Generic;
using System.Text;

namespace EasyPluggableDI.Core.Configs
{
    public class EasyPluggableDIConfig
    {
        public List<Module> Modules { get; set; }
    }

    public class Module
    {
        public string Location { get; set; }
    }
}