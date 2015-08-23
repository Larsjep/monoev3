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

			IStep turnOnStep = new StepContainer (TurnOn, "Turning On", "Error turning on", "Turned on sucesfully");
			IStep turnOffStep = new StepContainer (TurnOff, "Turning Off", "Error turning off");


			subSubMenu.AddItem( new ItemWithCheckBoxStep("Turned on", false, "Device", new CheckBoxStep(turnOnStep, turnOffStep ));
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
		
		private static bool TurnOn()
		{
			System.Threading.Thread.Sleep (2000);
			return true; //return false if it fails
		}
		
		private static bool TurnOff()
		{
			System.Threading.Thread.Sleep (500);
			return true; //return false if it fails
		}
		

	}
}
