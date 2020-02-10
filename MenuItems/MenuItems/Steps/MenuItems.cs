using MenuItems.Dto;
using MenuItems.PageObjects;
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
        private readonly ScenarioContext context;
        private MenuPageTests menuPageTests;

        public MenuItems(ScenarioContext injectedContext)
        {
            context = injectedContext;
            menuPageTests = new MenuPageTests();
        }

        [Given(@"I load the menu")]
        public void GivenILoadTheMenu()
        {
            menuPageTests.LoadXML();
        }

        [When(@"I update '(.*)' in '(.*)' menu item to '(.*)'")]
        public void WhenIUpdateInMenuItemTo(string sourceData, string category, string newValue)
        {
            menuPageTests.ModifyXML(category, sourceData, newValue);
        }

        [When(@"I create  '(.*)' in '(.*)' menu item")]
        public void WhenICreateInMenuItem(string sourceData, string category)
        {
            menuPageTests.CreateNewMenuItem(category, sourceData);
        }

        [When(@"I delete '(.*)' in '(.*)' menu item")]
        public void WhenIDeleteInMenuItem(string sourceData, string category)
        {
            menuPageTests.DeleteMenuItem(category, sourceData);
        }

        [When(@"I order the following items '(.*)'")]
        public void WhenIOrderTheFollowingItems(string orderItems)
        {
            menuPageTests.Order(orderItems);
        }

        [Then(@"the menu total should be '(.*)'")]
        public void ThenTheMenuTotalShouldBe(double expectedAmount)
        {
            var sum = menuPageTests.PrintReceipt();
            
            if (sum != expectedAmount)
            {
                throw new Exception($"Expected Amount '{expectedAmount}'. Actual Amount '{sum}'.");
            }
            Console.WriteLine($"Total Due:...........£{sum.ToString("F2")}");
        }

        [Then(@"the menu shall now reflect to contain the item '(.*)'")]
        [Then(@"the menu shall now reflect to contain the updated item '(.*)'")]
        public void ThenTheMenuShallNowReflectToContainTheUpdatedItem(string updatedValue)
        {
            menuPageTests.CheckIfExists(updatedValue, true);
        }

        [Then(@"the menu should not contain the item '(.*)'")]
        public void ThenTheMenuShouldNotContainTheItem(string value)
        {
            menuPageTests.CheckIfExists(value, false);
        }
    }
}
