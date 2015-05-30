using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Collections.Specialized;
using StartupApp;
using System.Linq;
namespace InstallCreator
{
	class MainClass
	{
		
		private static List<DownloadElement> downloadList = new List<DownloadElement>();
		private static List<string> extensionExclude = new List<string>{"mdb", "DS_Store"};
		private static List<string> fileExclude = new List<string>{"package.xml", "InstallCreator.exe"};

		private static string currentDir;
		private static string installFileName = "package.xml";

	    static public string NormalizeFilepath(string filepath)
	    {
	        string result = System.IO.Path.GetFullPath(filepath);
			var removeChars = new char[] { '\\'};

	        result = result.TrimEnd(removeChars);
			return result;
	    }

	    public static string GetRelativePath(string rootPath, string fullPath)
	    {
	        rootPath = NormalizeFilepath(rootPath);
	        fullPath = NormalizeFilepath(fullPath);

	        if (!fullPath.StartsWith(rootPath))
	            throw new Exception("Error when calculating relative path.");

	        if(fullPath.Length != rootPath.Length)
	        	return fullPath.Substring(rootPath.Length+1);
	        return "";
	    }


		private static void DirSearch(string sDir) 
		{
		    try 
		    {
		        foreach (string f in Directory.GetFiles(sDir)) 
		        {
					string fileName = Path.GetFileName(f);
					bool exclude = extensionExclude.Any(item => fileName.EndsWith(item)) || fileExclude.Any(item => fileName == item);
					if(!exclude)
					{
						string subdir =  GetRelativePath(currentDir, Path.GetDirectoryName(f));
						DownloadElement element = new DownloadElement(fileName, subdir);
						downloadList.Add(element);
					}
		        }

		        foreach (string d in Directory.GetDirectories(sDir)) 
		        {
		            DirSearch(d);
		        }
		    }
		    catch
		    {
		        Console.WriteLine("Failed during dir seach");
		    }
		}

		public static void Main (string[] args)
		{
			currentDir = Directory.GetCurrentDirectory ();
			DirSearch (currentDir);
			InstallPackage installSettings = new InstallPackage();
			foreach (var element in downloadList) 
			{
				installSettings.AddDownloadElement(element);
			}
			installSettings.SaveToXML(Path.Combine(currentDir, installFileName));
		}
	}
}
