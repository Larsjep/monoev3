using System;
using System.Collections;
using MonoBrickFirmware.FileSystem;
using System.Linq;

namespace MonoBrickWebServer.Models
{
	public class ProgramModelList : IEnumerable
	{
		private System.Collections.Generic.List<IProgramModel> programs = new System.Collections.Generic.List<IProgramModel>();
		private bool useDummy;
		public ProgramModelList(bool useDummy)
		{
			this.useDummy = useDummy;
			Update(useDummy);
		}

		public IProgramModel this [int programIndex] {
			get 
			{
				return programs[programIndex];
			}
		}

		public IProgramModel this [string programName] {
			get 
			{
				return programs.First(p => p.Name == programName);
			}
		}

		public IEnumerator GetEnumerator()
		{
			//return programs.GetEnumerator();

			foreach (object o in programs)
			{
				if(o == null)
				{
					break;
				}
				yield return o;
			}
		}

		public void Update()
		{
			Update(useDummy);	
		}

		private void Update(bool useDummy)
		{
			if(useDummy)
			{
				programs = new System.Collections.Generic.List<IProgramModel> ();
				programs.Add(new DummyProgramModel("Gyro Test"));
				programs.Add(new DummyProgramModel("Motor Test"));
				programs.Add(new DummyProgramModel("HT compass"));
				programs.Add(new DummyProgramModel("Rubiks Cube Solver"));
			}
			else
			{
				var programInfoList = ProgramManager.Instance.GetProgramInformationList ();
				programs = new System.Collections.Generic.List<IProgramModel> ();
				foreach (var program in programInfoList) 
				{
					programs.Add(new ProgramModel(program));
				}

			}

		}

	}
}

