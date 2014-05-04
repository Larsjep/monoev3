// MonoBrickAddinScpUpload.cs
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

using System;
using System.Threading;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using MonoDevelop.Core;
using Renci.SshNet;

namespace MonoBrickAddin
{
	class ScpUpload : IAsyncOperation
	{
		private string _localPath;
		private string _remotePath;
		private ScpClient _scpClient = null;
		private SshCommandHelper _sshHelper = null;
		private string _fileHash;

		// IAsyncOperation
		public bool IsCompleted { get; private set; }

		public bool Success { get; private set; }

		public bool SuccessWithWarnings { get; private set; }

		public string ErrorMessage { get; private set; }

		public event OperationHandler Completed;

		ManualResetEvent wait = new ManualResetEvent(false);

		public void WaitForCompleted()
		{
			WaitHandle.WaitAll(new WaitHandle [] { wait });
		}

		public void Cancel()
		{
			if (_scpClient.IsConnected)
				_scpClient.Disconnect();

			ErrorMessage = "Upload canceled";
			wait.Set();
		}

		public ScpUpload(string IPAddress, string localPath, string remotePath)
		{
			_localPath = localPath;
			_remotePath = remotePath;
			_scpClient = new ScpClient(IPAddress, "root", "");
			_sshHelper = new SshCommandHelper(IPAddress);
			_fileHash = UserSettings.Instance.LastUploadHash;

			Success = false;
			SuccessWithWarnings = false;
			ErrorMessage = "";
		}

		public void Upload()
		{
			Success = false;
			SuccessWithWarnings = false;
			ErrorMessage = "";

			ThreadPool.QueueUserWorkItem(delegate
			{
				try
				{
					DoUpload();
					Success = true;
				}
				catch (Exception ex)
				{
					Success = false;
					ErrorMessage = ex.Message;
				}
				finally
				{
					try
					{
						if (_scpClient.IsConnected)
							_scpClient.Disconnect();
					}
					catch (Exception ex)
					{
						ErrorMessage = ex.Message;
						Success = false;
					}
				}
				IsCompleted = true;
				wait.Set();
				if (Completed != null)
					Completed(this);
			});
		}

		private void DoUpload()
		{
			string newUploadHash = CreateMd5ForFolder(_localPath);
			if (newUploadHash == _fileHash)
				return; // nothing to do

			_sshHelper.WriteSSHCommand(GetDirectoryCommand());

			_scpClient.Connect();

			DirectoryInfo di = new DirectoryInfo(_localPath);

			_scpClient.Upload(di, _remotePath);

			_scpClient.Disconnect();

			UserSettings.Instance.LastUploadHash = newUploadHash;
			UserSettings.Save();
		}

		private string GetDirectoryCommand()
		{
			string makeDirString = string.Format("mkdir -p {0}", _remotePath);
			return makeDirString;
		}
			
		private static string CreateMd5ForFolder(string path)
		{
			string newHash = "";

			try
			{
				// check the top directory only
				var files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly)
					.OrderBy(p => p).ToList();

				MD5 md5 = MD5.Create();

				for(int i = 0; i < files.Count; i++)
				{
					string file = files[i];

					// hash path
					string relativePath = file.Substring(path.Length + 1);
					byte[] pathBytes = Encoding.UTF8.GetBytes(relativePath.ToLower());
					md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

					// hash contents
					byte[] contentBytes = File.ReadAllBytes(file);
					if (i == files.Count - 1)
						md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
					else
						md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
				}

				newHash = BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();
			}
			catch
			{
			}

			return newHash;
		}
	}
}
