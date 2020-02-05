Feature: MenuCrudTests


Scenario Outline: Update Menu Items
Given I load the menu
When I update '<item>' in '<category>' menu item to '<updateValue>'
Then the menu shall now reflect to contain the updated item '<updateValue>'
Examples: 
| item | category | updateValue   |
| Soup | Starters  | VeryNice Soup |

Scenario Outline: Create New Menu Items
Given I load the menu
When I create  '<item>' in '<category>' menu item 
Then the menu shall now reflect to contain the item '<item>'
Examples: 
| item | category |
| Saag | Mains  | 
| Bread | Starters  | 

Scenario Outline: Delete Menu Items
Given I load the menu
When I delete '<item>' in '<category>' menu item 
Then the menu should not contain the item '<item>'
Examples: 
| item | category |
| Pizza | Mains  | 

Scenario Outline: Order Menu Items
Given I load the menu
When I have added the relavent menu items
When I order the following items '<items>'
Then the menu total should be '<total>'
Examples: 
| items                                | total |
| Starters;Soup;Bread:Mains;Pizza;Saag | 22.8  |