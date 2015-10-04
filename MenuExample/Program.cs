using System;
using MonoBrickFirmware.Display.Dialogs;
using MonoBrickFirmware.Display.Menus;

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

			subSubMenu.AddItem( new ItemWithCheckBoxStep("Turned on", false, "Device", new CheckBoxStep(turnOnStep, turnOffStep), OnTurnOnChanged  ));
			subSubMenu.AddItem (new MonoBrickQuestion());
			subSubMenu.AddItem( new ItemWithBrickInfo());

			subMenu.AddItem (subSubMenu);
			subMenu.AddItem( new ItemWithCharacterInput("Name", "Enter Name", "Anders", OnNameChanged, false, true, true));
			subMenu.AddItem( new ItemWithNumericInput("Age", 29, OnAgeChanged, 0, 100));
			subMenu.AddItem( new ItemWithCheckBox("Loves C#", true, OnCheckedChanged));

			mainMenu.AddItem (subMenu);
			mainMenu.AddItem (new ItemWithProgramList ("Programs"));
			
			container = new MenuContainer(mainMenu);
			container.Show (true);
		}

		private static void OnTurnOnChanged(bool isOn)
		{
			Console.WriteLine ("Turn on set to  " + isOn);	
		}

		private static void OnCheckedChanged(bool isChecked)
		{
			Console.WriteLine ("Loves C# is set to " + isChecked);
		}

		private static void OnNameChanged(string newName)
		{
			Console.WriteLine ("Name was changed to" + newName);
		}

		private static void OnAgeChanged(int newAge)
		{
			Console.WriteLine ("Your new age is " + newAge);
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

	public class MonoBrickQuestion : ChildItemWithParent
	{
		private ItemWithDialog<QuestionDialog> questionDialog = new ItemWithDialog<QuestionDialog>(new QuestionDialog ("Do you Like MonoBrick?", "MonoBrick"));
		public MonoBrickQuestion() : base("Question")
		{

		}

		public override void OnEnterPressed ()
		{
			questionDialog.SetFocus(this, OnExit);	
		}

		public void OnExit (QuestionDialog dialog)
		{
			Console.WriteLine ("Do you like MonoBrick is set to " + dialog.IsPositiveSelected);
		}

	}

}
