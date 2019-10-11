using System;
using Examples.Interfaces;

namespace Examples.Plugin
{
    public class TestImpl : ITest
    {
        public string Get()
        {
            return "ciao :-)";
        }
    }
}
