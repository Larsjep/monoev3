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
			List<IMenuItem> items = new List<IMenuItem>();
			items.Add (new MenuItemWithAction("Personal details", () => ShowSubMenu(),MenuItemSymbole.RightArrow));
			items.Add (new MenuItemWithAction("Execute steps", () => ExecuteSteps()));
			Menu m = new Menu("Main menu", items);
			m.Show();	
		}
		
		public static bool ExecuteSteps ()
		{
			List<IStep> steps = new List<IStep>();
			steps.Add( new StepContainer(DummyStep, "Dummy step 1", "Error executing step 1") );
			steps.Add( new StepContainer(DummyStep, "Dummy step 2", "Error executing step 2") );
			Dialog stepDialog = new StepDialog("Doing steps", steps);
			stepDialog.Show();
			return false;
		}
		
		
		
		public static bool ShowSubMenu ()
		{
			List<IMenuItem> items = new List<IMenuItem> ();
			var nameItem = new MenuItemWithCharacterInput("Name", "Enter Name", "Anders");
			var ageItem = new MenuItemWithNumericInput("Age", 29, 0, 100);
			var genderItem = new MenuItemWithOptions<string>("Option", new string[]{"Male","Female"});
			var programItem = new MenuItemWithCheckBox("Loves C#", true);
			var checkBoxWithActionItem = new MenuItemWithCheckBox("Execute", true, TurnCheckBoxOnOff);
			
			items.Add(nameItem);
			items.Add(ageItem);
			items.Add(genderItem);
			items.Add(programItem);
			items.Add(checkBoxWithActionItem);
			
			//Show the menu
			Menu m = new Menu ("Sub Menu", items);
			m.Show ();
			Console.WriteLine("Your name is " + nameItem.Text);
			Console.WriteLine("Your genger is " + genderItem.GetSelection().ToString());
			Console.WriteLine("Your age is " + ageItem.Value);
			if(programItem.Checked)
				Console.WriteLine(nameItem.Text + " loves C#");
			Console.WriteLine("Is checked: " + checkBoxWithActionItem.Checked);			
			return false;
		}
		
		public static bool TurnCheckBoxOnOff (bool isChecked)
		{
			if (isChecked) {
				var step = new StepContainer (DummyStep, "Turning off", "Failed to turn off");
				var progressDialog = new ProgressDialog ("Information", step);
				progressDialog.Show ();
				return false;
			} 
			else 
			{
				var dialog = new InfoDialog("Turning On", true);
				dialog.Show();
				return true;
			}
		}
		
		public static bool DummyStep ()
		{
			System.Threading.Thread.Sleep(5000);
			return true;
		}
		
	}
}
