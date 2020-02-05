using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using TechTalk.SpecFlow;

namespace MenuItems.Hook
{
    [Binding]
    public sealed class Hooks
    {
        // For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks

        [BeforeScenario]
        public void BeforeScenario()
        {
            //TODO: implement logic that has to run before executing each scenario
        }

        [AfterScenario]
        public void AfterScenario()
        {
            var assembly = Assembly.GetExecutingAssembly().Location;
            var _path = Path.GetFullPath(Path.Combine(assembly, @"..\..\..\Resources\"));
            using (FileStream fs = new FileStream(_path + "MenuBackup.xml", FileMode.Open, FileAccess.Read))
            {
                var masterXdoc = XDocument.Load(fs);
                using (FileStream fs2 = new FileStream(_path + "Menu.xml", FileMode.Truncate, FileAccess.Write))
                {
                    masterXdoc.Save(fs2);
                }
            }
        }
    }
}
