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
		public static Lcd lcd;
		public static Buttons btns;
		
		public static void Main (string[] args)
		{
			lcd = new Lcd();
			btns = new Buttons();
			List<IMenuItem> items = new List<IMenuItem>();
			items.Add (new MenuItemWithAction(lcd, "Personal details", () => ShowSubMenu(),MenuItemSymbole.RightArrow));
			items.Add (new MenuItemWithAction(lcd, "Execute steps", () => ExecuteSteps()));
			Menu m = new Menu(Font.MediumFont, lcd, btns ,"Main menu", items);
			m.Show();	
		}
		
		public static bool ExecuteSteps ()
		{
			List<IStep> steps = new List<IStep>();
			steps.Add( new StepContainer(DummyStep, "Dummy step 1", "Error executing step 1") );
			steps.Add( new StepContainer(DummyStep, "Dummy step 2", "Error executing step 2") );
			Dialog stepDialog = new StepDialog(lcd, btns, "Doing steps", steps);
			stepDialog.Show();
			return false;
		}
		
		
		
		public static bool ShowSubMenu ()
		{
			List<IMenuItem> items = new List<IMenuItem> ();
			var nameItem = new MenuItemWithCharacterInput(lcd,btns, "Name", "Enter Name", "Anders");
			var ageItem = new MenuItemWithNumericInput(lcd,"Age", 29, 0, 100);
			var genderItem = new MenuItemWithOptions<string>(lcd,"Option", new string[]{"Male","Female"});
			var programItem = new MenuItemWithCheckBox(lcd,"Loves C#", true);
			var checkBoxWithActionItem = new MenuItemWithCheckBox(lcd, "Execute", true, TurnCheckBoxOnOff);
			
			items.Add(nameItem);
			items.Add(ageItem);
			items.Add(genderItem);
			items.Add(programItem);
			items.Add(checkBoxWithActionItem);
			
			//Show the menu
			Menu m = new Menu (Font.MediumFont, lcd, btns, "Sub Menu", items);
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
			Dialog dialog;
			if(isChecked)
			{
				dialog = new InfoDialog(Font.MediumFont,lcd,btns,"Turning off. Please wait...", false);
				dialog.Show();
				System.Threading.Thread.Sleep(2000);
				return false;
			}
			
			dialog = new InfoDialog(Font.MediumFont,lcd,btns,"Turning On. Please wait...", false);
			dialog.Show();
			System.Threading.Thread.Sleep(2000);
			return true;
		}
		
		public static bool DummyStep ()
		{
			System.Threading.Thread.Sleep(10000);
			return true;
		}
		
	}
}
