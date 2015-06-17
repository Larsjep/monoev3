using System;
using System.Collections.Generic;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Display.Dialogs;
using MonoBrickFirmware.Display.Menus;
using MonoBrickFirmware.UserInput;

namespace MenuExample
{
	class MainClass
	{
		


		public static void Main (string[] args)
		{
			MenuContainer container;
			Menu mainMenu = new Menu("Main menu");
			Menu subMenu = new Menu("Sub menu");
			Menu subSubMenu = new Menu ("sub sub Menu");

			subSubMenu.AddItem( new ItemWithCharacterInput("Name", "Enter Name", "Anders"));
			subSubMenu.AddItem( new ItemWithNumericInput("Age", 29, 0, 100));
			subSubMenu.AddItem( new ItemWithOptions<string>("Option", new string[]{"Male","Female"}));
			subSubMenu.AddItem( new ItemWithCheckBox("Loves C#", true));

			subMenu.AddItem (subSubMenu);
			subMenu.AddItem( new ItemWithCharacterInput("Name", "Enter Name", "Anders"));
			subMenu.AddItem( new ItemWithNumericInput("Age", 29, 0, 100));
			subMenu.AddItem( new ItemWithCheckBox("Loves C#", true));

			mainMenu.AddItem (subMenu);
			mainMenu.AddItem (new ItemWithProgramList ("Programs"));
			
			container = new MenuContainer(mainMenu);
			container.Show();
			
		}

	}
}
