using System;
using System.Collections.Generic;
using MonoBrickFirmware.FileSystem;
using MonoBrickFirmware.Display.Dialogs;
using System.Threading;

namespace MonoBrickFirmware.Display.Menus
{
	public class ItemWithProgramList : ItemList
	{
		private bool useEscToStop;
		public ItemWithProgramList (string title, bool useEscToStop = false): base(title, Font.MediumFont, true, "No programs has been uploaded")
		{
			this.useEscToStop = useEscToStop;	 		
		}

		protected override List<IChildItem> OnCreateChildList ()
		{
			List<ProgramInformation> programs = ProgramManager.GetProgramInformationList();
			var childList = new List<IChildItem> ();
			foreach(var program in programs)
			{
				childList.Add (new ProgramItem (program, useEscToStop));
			}
			return childList;
		}
	}

	internal class ProgramItem : ChildItemWithParent
	{
		private ProgramInformation programInformation;
		private bool useEscToStop;
		private ItemWithDialog<SelectDialog<string>> programSelectDialog = null;
		private ItemWithDialog<QuestionDialog> aotQuestionDialog = null;
		private ItemWithDialog<StepDialog> compileDialog = null;
		private ItemWithDialog<QuestionDialog> deleteQuestionDialog = null;
		private ItemWithDialog<ProgressDialog> deleteDialog = null;
		private ItemWithDialog<InfoDialog> compileBeforeExecution = new ItemWithDialog<InfoDialog>( new InfoDialog("Program will be AOT compiled"));
		private readonly string[] selectArray = new string[]{ "Run Program", "Run In AOT", "AOT Compile", "Delete Program" };

		public ProgramItem (ProgramInformation programInformation, bool useEscToStop) : base(programInformation.Name)
		{
			this.programInformation = programInformation;
			this.useEscToStop = useEscToStop;
			programSelectDialog = new ItemWithDialog<SelectDialog<string>>( new SelectDialog<string> (selectArray, "Options", true));
			aotQuestionDialog = new ItemWithDialog<QuestionDialog> (new QuestionDialog ("Progran already compiled. Recompile?", "AOT recompile"));
			compileDialog = new ItemWithDialog<StepDialog>(new StepDialog ("Compiling", new List<IStep> (){ new StepContainer (CompileProgram, "compiling program", "Failed to compile")}));
			deleteQuestionDialog = new ItemWithDialog<QuestionDialog> (new QuestionDialog ("Are you sure?", "Delete"));
			deleteDialog = new ItemWithDialog<ProgressDialog> (new ProgressDialog ("", new StepContainer (DeleteProgram, "Deleting ", "Error deleting program")));
		}

		public override void OnEnterPressed ()
		{
			programSelectDialog.SetFocus (this, OnSelectDialogExit);
		}

		private bool DeleteProgram()
		{
			ProgramManager.DeleteProgram (programInformation); 
			return true;

		}

		private void OnStartProgramCompileExit(StepDialog dialog)
		{
			if (dialog.ExecutedOk)
			{
				var start = new ExecuteProgramDialog (this.programInformation, true, useEscToStop);
				start.Start (Parent);		
			}			
		}
		
		private bool CompileProgram()
		{
			return ProgramManager.AOTCompileProgram(programInformation);	
		}

		private void OnCompileDialogExit(QuestionDialog dialog)
		{
			if (dialog.IsPositiveSelected) 
			{
				compileDialog.SetFocus (Parent);
			} 
			else 
			{
				programSelectDialog.SetFocus (this, OnSelectDialogExit);		
			}
		}

		private void OnDeleteDialogExit(QuestionDialog dialog)
		{
			if (dialog.IsPositiveSelected) 
			{
				deleteDialog.SetFocus (this.Parent);
			} 
			else 
			{
				programSelectDialog.SetFocus (this, OnSelectDialogExit);
			}
		} 

		private void OnCompileInfoDialogExit(InfoDialog dialog)
		{
			compileDialog.SetFocus (Parent, OnStartProgramCompileExit);
		}

		private void OnSelectDialogExit(SelectDialog<string> dialog)
		{
			if (!dialog.EscPressed) {
				switch (dialog.GetSelectionIndex ()) {
				case 0:
					var startDialog = new ExecuteProgramDialog (this.programInformation, false, useEscToStop);
					startDialog.Start (Parent);
					break;
				case 1:
					if (!programInformation.IsAOTCompiled)
					{
						compileBeforeExecution.SetFocus (Parent, OnCompileInfoDialogExit); 
					} 
					else 
					{
						var start = new ExecuteProgramDialog (this.programInformation, true, useEscToStop);
						start.Start (Parent);
					}
					break;
				case 2:
					if (programInformation.IsAOTCompiled)
					{
						aotQuestionDialog.SetFocus (Parent,OnCompileDialogExit);
					} 
					else 
					{
						compileDialog.SetFocus (Parent);
					}
					break;
				case 3:
					deleteQuestionDialog.SetFocus (Parent,OnDeleteDialogExit);
					break;
				}
			}		
		}
	}

	internal class ExecuteProgramDialog : ChildItem
	{
		private ProgramInformation program;
		private ItemWithDialog<InfoDialog> exceptionInfoDialog = new ItemWithDialog<InfoDialog> (new InfoDialog("Exception during execution", "Error"));
		private bool inAot;
		private readonly bool useEscToStop;

		public ExecuteProgramDialog(ProgramInformation programInfo, bool inAot, bool useEscToStop) : base("")
		{
			this.program = programInfo;
			this.useEscToStop = useEscToStop;
			this.inAot = inAot;
		}

		private void StartProgram()
		{
			if (ProgramManager.RunningProgram == null)
			{
				if (!useEscToStop) 
				{
					Parent.SuspendButtonEvents ();
				}
				Lcd.Clear ();
				Lcd.Update ();
				ProgramManager.StartProgram (program, inAot, OnDone);
			} 
			else 
			{
				//Show some dialog
			}
		}


		public void Start(IParentItem parent)
		{
			Parent = parent;
			Parent.SetFocus (this);
			StartProgram ();
		}

		public override void OnEscPressed ()
		{
			ProgramManager.StopProgram (this.program);		
		}

		private void OnDone(Exception e)
		{
			if (!useEscToStop)
			{
				Parent.ResumeButtonEvents ();
			}
			if (e != null) 
			{
				exceptionInfoDialog.SetFocus (Parent);
			} 
			else 
			{
				Parent.RemoveFocus (this);
			}
		}
	}
}

