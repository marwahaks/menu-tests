using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using TechTalk.SpecFlow;

namespace MenuItems.Steps
{
    [Binding]
    public sealed class MenuItems
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private readonly ScenarioContext context;
        private XDocument _xmlDoc;
        private XNamespace _namespace;
        private string _path;
        private double _sum;
        public MenuItems(ScenarioContext injectedContext)
        {
            context = injectedContext;
            var assembly = Assembly.GetExecutingAssembly().Location;
            _path = Path.GetFullPath(Path.Combine(assembly, @"..\..\..\Resources\Menu.xml"));
        }

        [Given(@"I load the menu")]
        public void GivenILoadTheMenu()
        {
            LoadXML();
        }


        [When(@"I have added the relavent menu items")]
        public void WhenIHaveAddedTheRelaventMenuItems()
        {
            WhenICreateInMenuItem("Saag","Mains");
            WhenICreateInMenuItem("Bread","Starters");
        }


        [When(@"I update '(.*)' in '(.*)' menu item to '(.*)'")]
        public void WhenIUpdateInMenuItemTo(string sourceData, string category, string newValue)
        {
            ModifyXML(category, sourceData, newValue);
        }

        [When(@"I create  '(.*)' in '(.*)' menu item")]
        public void WhenICreateInMenuItem(string sourceData, string category)
        {
            CreateNewMenuItem(category, sourceData);
        }

        [When(@"I delete '(.*)' in '(.*)' menu item")]
        public void WhenIDeleteInMenuItem(string sourceData, string category)
        {
            DeleteMenuItem(category, sourceData);
        }


        [When(@"I order the following items '(.*)'")]
        public void WhenIOrderTheFollowingItems(string orderItems)
        {

            var starters = orderItems.Split(':').FirstOrDefault(x => x.Contains("Starters"))?.Split(';').ToList();
            var mains = orderItems.Split(':').FirstOrDefault(x => x.Contains("Mains"))?.Split(';').ToList();
            starters?.RemoveAt(0);
            mains?.RemoveAt(0);

            var dictionary = new List<(int index, string name, string price)>();//[starters.Count() + mains.Count()];
            if (starters != null)
            {
                dictionary.AddRange(LoopOverItems("Starters", starters));
            }
            if (mains != null)
            {
                dictionary.AddRange(LoopOverItems("Mains", mains));
            }

            _sum = 0.00;
            dictionary.All(x =>
            {
                Console.WriteLine($"{x.name}......  £{x.price}");
                _sum += Convert.ToDouble(x.price);
                return true;
            });

        }
        [Then(@"the menu total should be '(.*)'")]
        public void ThenTheMenuTotalShouldBe(double expectedAmount)
        {
            if (_sum != expectedAmount)
            {
                throw new Exception($"Expected Amount '{expectedAmount}'. Actual Amount '{_sum}'.");
            }
        }



        private (int index, string name, string price)[] LoopOverItems(string category, List<string> items)
        {
            var xmlnodes = _xmlDoc.Root.Elements(category);
            var xmlnodesDescendants = xmlnodes.Descendants().ToArray();
            var dictionary = new (int index, string name, string price)[items.Count()];

            for (int i = 0; i < items.Count(); i++)
            {
                if (items[i].Contains(xmlnodesDescendants[i].Value))
                {
                    var cost = xmlnodes.Attributes().FirstOrDefault(x => x.Name.LocalName.Equals("price")).Value;
                    dictionary[i] = ((i, items[i], cost));
                }
            }
            return dictionary;
        }


        [Then(@"the menu shall now reflect to contain the item '(.*)'")]
        [Then(@"the menu shall now reflect to contain the updated item '(.*)'")]
        public void ThenTheMenuShallNowReflectToContainTheUpdatedItem(string updatedValue)
        {
            var itemExists = _xmlDoc.Descendants().FirstOrDefault(i => i.Value.Equals(updatedValue));
            if (itemExists == null)
            {
                throw new Exception($"The item '{updatedValue}' does not exist within the menu");
            }
        }

        [Then(@"the menu should not contain the item '(.*)'")]
        public void ThenTheMenuShouldNotContainTheItem(string value)
        {
            var itemExists = _xmlDoc.Descendants().FirstOrDefault(i => i.Value.Equals(value));
            if (itemExists != null)
            {
                throw new Exception($"The item '{value}' does not exist within the menu");
            }
        }

        public void LoadXML()
        {
            using (FileStream fs = new FileStream(_path, FileMode.Open, FileAccess.Read))
            {
                _xmlDoc = XDocument.Load(fs);
                _namespace = _xmlDoc.Root.GetDefaultNamespace();
            }
        }

        public void ModifyXML(string menuType, string sourceData, string newValue)
        {
            if (_xmlDoc == null)
            {
                LoadXML();
            }
            using (FileStream fs = new FileStream(_path, FileMode.Truncate, FileAccess.Write))
            {
                var source = _xmlDoc.Root.Elements(_namespace + menuType).Descendants("Item").FirstOrDefault(x => x.Value.Equals(sourceData));
                source.SetValue(newValue);
                _xmlDoc.Save(fs);
            }
        }

        public void CreateNewMenuItem(string menuType, string newItem)
        {
            if (_xmlDoc == null)
            {
                LoadXML();
            }
            using (FileStream fs = new FileStream(_path, FileMode.Truncate, FileAccess.Write))
            {
                var source = _xmlDoc.Root.Elements(_namespace + menuType).FirstOrDefault();
                if (source != null)
                {
                    source.Add(new XElement(_namespace + "Item")
                    {
                        Value = newItem
                    });
                }
                else
                {
                    throw new Exception($"Cannot find the menu '{menuType}'");
                }
                _xmlDoc.Save(fs);
            }
        }

        public void DeleteMenuItem(string menuType, string menuItem)
        {
            if (_xmlDoc == null)
            {
                LoadXML();
            }
            using (FileStream fs = new FileStream(_path, FileMode.Truncate, FileAccess.Write))
            {
                var source = _xmlDoc.Root.Elements(_namespace + menuType).Descendants("Item").FirstOrDefault(x => x.Value.Equals(menuItem));
                if (source != null)
                {
                    source.Remove();
                }
                else
                {
                    throw new Exception($"Cannot find the menu item'{menuItem}'");
                }
                _xmlDoc.Save(fs);
            }
        }
    }
}
