using MenuItems.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace MenuItems.PageObjects
{
    public class MenuPageTests
    {
        public BasketModel Basket;
        private XDocument _xmlDoc;
        private XNamespace _namespace;
        private string _path;
        private double _sum;

        public MenuPageTests()
        {
            Basket = new BasketModel();
            var assembly = Assembly.GetExecutingAssembly().Location;
            _path = Path.GetFullPath(Path.Combine(assembly, @"..\..\..\Resources\Menu.xml"));
        }

        public void LoadXML()
        {
            using (FileStream fs = new FileStream(_path, FileMode.Open, FileAccess.Read))
            {
                _xmlDoc = XDocument.Load(fs);
                _namespace = _xmlDoc.Root.GetDefaultNamespace();
            }
        }

        public void Order(string orderItems)
        {
            var starters = orderItems.Split(':').FirstOrDefault(x => x.Contains("Starters"))?.Split(';').ToList();
            var mains = orderItems.Split(':').FirstOrDefault(x => x.Contains("Mains"))?.Split(';').ToList();
            starters?.RemoveAt(0);
            mains?.RemoveAt(0);

            if (starters != null)
            {
                LoopOverItems("Starters", starters);
            }
            if (mains != null)
            {
                LoopOverItems("Mains", mains);
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

        public double PrintReceipt()
        {
            _sum = 0.00;
            Basket.Basket.All(x =>
            {
                Console.WriteLine($"{x.Key}.............. £{x.Value}");
                _sum += Convert.ToDouble(x.Value);
                return true;
            });

            return _sum;
        }

        public void CheckIfExists(string value, bool expected)
        {
            var itemExists = _xmlDoc.Descendants().FirstOrDefault(i => i.Value.Equals(value));

            if ((itemExists == null) == expected)
            {
                throw new Exception($"The item '{value}' = {expected}");
            }
        }

        private void LoopOverItems(string category, List<string> items)
        {
            var xmlnodes = _xmlDoc.Root.Elements(category);
            var xmlnodesDescendants = xmlnodes.Descendants().ToArray();
            var dict = new Dictionary<string, string>();

            for (int i = 0; i < items.Count(); i++)
            {
                if (xmlnodesDescendants.Any(x => x.ToString().Contains(items[i])))
                {
                    var cost = xmlnodes.Attributes().FirstOrDefault(x => x.Name.LocalName.Equals("price")).Value;
                    dict.Add(items[i], cost);
                }
            }
            Basket.Basket = dict;
        }
    }
}
