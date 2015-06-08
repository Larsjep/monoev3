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
		public static MenuContainer container;
		public static Menu mainMenu;
		public static Menu subMenu;
		public static Menu subSubMenu;



		public static void Main (string[] args)
		{
			subSubMenu = new Menu ("sub Sub Menu", Font.MediumFont);
			subSubMenu.AddMenuItem( new ItemWithCharacterInput("Name", "Enter Name", "Anders"));
			subSubMenu.AddMenuItem( new ItemWithNumericInput("Age", 29, 0, 100));
			subSubMenu.AddMenuItem( new ItemWithOptions<string>("Option", new string[]{"Male","Female"}));
			subSubMenu.AddMenuItem( new ItemWithCheckBox("Loves C#", true));


			subMenu = new Menu ("Sub Menu", Font.MediumFont);
			subMenu.AddMenuItem (subSubMenu);
			subMenu.AddMenuItem( new ItemWithCharacterInput("Name", "Enter Name", "Anders"));
			subMenu.AddMenuItem( new ItemWithNumericInput("Age", 29, 0, 100));
			subMenu.AddMenuItem( new ItemWithCheckBox("Loves C#", true));

			mainMenu = new Menu ("Main menu", Font.MediumFont);
			mainMenu.AddMenuItem (subMenu);
			mainMenu.AddMenuItem (new ItemWithProgramList ("Programs"));
			
			container = new MenuContainer(mainMenu);
			container.Show();
			
		}

	}
}
