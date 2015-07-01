using System;
using MonoBrickFirmware.Display.Dialogs;

namespace MonoBrickFirmware.Display.Menus
{
	public class ItemWithCheckBoxStep : ItemWithCheckBox, IParentItem
	{
		private CheckBoxStep itemStep = null;
		private string stepTitle;
		private ItemWithProgressDialog dialogItem;
		public ItemWithCheckBoxStep (string text, bool checkedAtStart, string stepTitle, CheckBoxStep step) : base(text, checkedAtStart)
		{
			this.stepTitle = stepTitle;
			this.itemStep = step;
			dialogItem = new ItemWithProgressDialog (new ProgressDialog(stepTitle, itemStep));
		}

		public override void OnEnterPressed ()
		{
			itemStep.Checked = this.Checked;
			dialogItem.SetFocus(this);	
		}

		public override void OnHideContent ()
		{
			dialogItem.OnHideContent ();
		}

		public void OnUpPressed ()
		{

		}

		public void OnDownPressed ()
		{

		}

		public void OnEscPressed ()
		{

		}

		#region IParentItem implementation

		public void SetFocus (IChildItem item)
		{
			Parent.SetFocus (item);
		}

		public virtual void RemoveFocus (IChildItem item)
		{
			Parent.SetFocus(item);
		}

		public void SuspendEvents (IChildItem item)
		{
			Parent.SuspendEvents (item);
		}

		public void ResumeEvents (IChildItem item)
		{
			Parent.SuspendEvents (item);
		}

		#endregion

	}

	public class CheckBoxStep: IStep
	{
		private IStep checkedStep;
		private IStep unCheckStep;

		public CheckBoxStep(IStep checkedStep, IStep unCheckedStep)
		{
			this.checkedStep = checkedStep;
			this.unCheckStep = unCheckedStep;
		}


		public bool Checked{get; set;}
		public string StepText{get {return Checked ? unCheckStep.StepText : checkedStep.StepText;}}
		public string ErrorText{get {return Checked ? unCheckStep.ErrorText : checkedStep.ErrorText;}}
		public string OkText{get {return Checked ? unCheckStep.OkText : checkedStep.OkText;}}
		public bool ShowOkText{get {return Checked ? unCheckStep.ShowOkText : checkedStep.ShowOkText;}}
		public bool Execute ()
		{
			return Checked ? unCheckStep.Execute () : checkedStep.Execute ();
		}
	}

	internal class ItemWithProgressDialog : ItemWithDialog<ProgressDialog>
	{
		public ItemWithProgressDialog(ProgressDialog dialog): base(dialog)
		{

		}

		public override void OnExit (ProgressDialog dialog)
		{
			if (dialog.Ok)
			{
				((ItemWithCheckBox)Parent).Checked	 = !((ItemWithCheckBox)Parent).Checked;
			}
			Parent.RemoveFocus (this);
		}
	}
}

