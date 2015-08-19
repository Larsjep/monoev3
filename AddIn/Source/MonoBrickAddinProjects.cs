// MonoBrickAddinProject.cs
//
// Author:
//       Bernhard Straub
// 
// Copyright (c) 2014 Bernhard Straub
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// 	The above copyright notice and this permission notice shall be included in
// 	all copies or substantial portions of the Software.
// 
// 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// 	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// 	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// 	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// 	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// 	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// 	THE SOFTWARE.

using System.Xml;
using System.Reflection;
using MonoDevelop.Projects;
using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using MonoDevelop.Core.ProgressMonitoring;
using System.IO;
using System.Diagnostics;

namespace MonoBrickAddin
{
	public class MonoBrickProject : DotNetAssemblyProject
	{
		FilePath referencePath = "";
		#region Constructors

		public MonoBrickProject()
		{
			Init();
		}

		public MonoBrickProject(string language) : base(language)
		{
			Init();
		}

		public MonoBrickProject(string languageName, ProjectCreateInformation info, XmlElement projectOptions)
			: base(languageName, info, projectOptions)
		{
			referencePath = info.ProjectBasePath;
			Init();
			//versionString = FileVersionInfo.GetVersionInfo("MonoBrickFirmware.dll").FileVersion;

		}

		void Init()
		{
			if (!referencePath.IsEmpty)
				CreateReference(referencePath);
		}

		public override SolutionItemConfiguration CreateConfiguration(string name)
		{
			var conf = new MonoBrickProjectConfiguration(name);
			conf.CopyFrom(base.CreateConfiguration(name));
			return conf;
		}

		#endregion

		#region Execution
		protected override bool OnGetCanExecute (MonoDevelop.Projects.ExecutionContext context, ConfigurationSelector config)
		{
			var configuration = (MonoBrickProjectConfiguration) GetConfiguration (config);
			var cmd = CreateExecutionCommand (config, configuration);
			return context.ExecutionHandler.CanExecute (cmd);
		}

		protected override ExecutionCommand CreateExecutionCommand(ConfigurationSelector configSel,
		                                                            DotNetProjectConfiguration configuration)
		{
			var conf = (MonoBrickProjectConfiguration)configuration;
			return new MonoBrickExecutionCommand(conf) {
				UserAssemblyPaths = GetUserAssemblyPaths(configSel),
			};
		}

		protected void CreateReference(FilePath sPath)
		{
			// Get the resource and write it out?
			FilePath sFileName = sPath.Combine("MonoBrickFirmware.dll");

			FileStream OutputStream = new FileStream(sFileName, FileMode.Create);
			Assembly ass = Assembly.GetExecutingAssembly();

			Stream DBStream = ass.GetManifestResourceStream("MonoBrick.MonoBrickFirmware.dll");
			for (int l = 0; l < DBStream.Length; l++)
			{
				OutputStream.WriteByte((byte)DBStream.ReadByte());
			}
			OutputStream.Close();
		}

		protected override void OnExecute(IProgressMonitor monitor, ExecutionContext context, ConfigurationSelector configSel)
		{
			var conf = (MonoBrickProjectConfiguration)GetConfiguration(configSel);
			var cmd = (MonoBrickExecutionCommand)CreateExecutionCommand(configSel, conf);

			bool debugExecution = !(context.ExecutionHandler is MonoBrickExecutionHandler);

			var runtime = conf.TargetRuntime;
		
			if (runtime.RuntimeId != "Mono" && debugExecution)
			{
				monitor.ReportError("You must use the Mono runtime for debugging!", null);
				return;
			}

			using (var opMon = new AggregatedOperationMonitor(monitor))
			{
				IConsole console = null;

				try
				{
					console = conf.ExternalConsole
						? context.ExternalConsoleFactory.CreateConsole(!conf.PauseConsoleOutput)
						: context.ConsoleFactory.CreateConsole(!conf.PauseConsoleOutput);

					cmd.Console = console;

					if (!MonoBrickUtility.GetEV3Configuration(console))
					{
						string configError = "Invalid EV3 configuration. Check Preferences->MonoBrick for correct settings!";

						console.Log.WriteLine(configError);
						monitor.ReportError(configError, null);
						return;
					}
					string EV3IPAddress = UserSettings.Instance.IPAddress;

					MonoBrickUtility.ShowMonoBrickLogo(EV3IPAddress);
					
					console.Log.WriteLine("Upload program to brick...");
					var uploadOp = MonoBrickUtility.Upload(EV3IPAddress, cmd);
					opMon.AddOperation(uploadOp);
				
					uploadOp.WaitForCompleted();

					if (!uploadOp.Success)
					{
						console.Log.WriteLine(uploadOp.ErrorMessage);
						monitor.ReportError(uploadOp.ErrorMessage, null);
						return;
					}

					console.Log.WriteLine("Suspending firmware execution");
					MonoBrickUtility.SuspendFirmware(EV3IPAddress);

					console.Log.WriteLine("Executing program on EV3 brick...");

					var ex = context.ExecutionHandler.Execute(cmd, console);
					opMon.AddOperation(ex);
					ex.WaitForCompleted();
				
					console.Log.WriteLine("");
					console.Log.WriteLine("Done executing program!");

					console.Log.WriteLine("Resuming firmware execution");
					MonoBrickUtility.ResumeFirmware(EV3IPAddress);

				}
				finally
				{
					if (console != null)
						console.Dispose();
				}
			}
		}

		#endregion
	}
}

