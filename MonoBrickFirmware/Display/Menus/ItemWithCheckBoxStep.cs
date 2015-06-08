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
		public void OnEnterPressed ()
		{
			itemStep.Checked = this.Checked;
			dialogItem.SetFocus(this);	
		}

		public void OnHideContent ()
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
		public string StepText{get {return Checked ? checkedStep.StepText : unCheckStep.StepText;}}
		public string ErrorText{get {return Checked ? checkedStep.ErrorText : unCheckStep.ErrorText;}}
		public string OkText{get {return Checked ? checkedStep.OkText : unCheckStep.OkText;}}
		public bool ShowOkText{get {return Checked ? checkedStep.ShowOkText : unCheckStep.ShowOkText;}}
		public bool Execute ()
		{
			return Checked ? checkedStep.Execute () : unCheckStep.Execute ();
		}
	}

	internal class ItemWithProgressDialog : ItemWithDialog<ProgressDialog>
	{
		public ItemWithProgressDialog(ProgressDialog dialog): base(dialog)
		{

		}

		public override void OnExit (ProgressDialog dialog)
		{
			((ItemWithCheckBox)Parent).Checked = dialog.Ok;
			Parent.RemoveFocus (this);
		}
	}
}

