using System;
using System.Collections.Generic;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Specialized;

namespace StartupApp
{
	[XmlRoot("ConfigRoot")]
	public class InstallPackage
	{
		public Task Task { get; set; }
		
		public InstallPackage()
		{
			Task = new Task();
			Task.DownloadElement = new ArrayList();
		} 

		public void AddDownloadElement(DownloadElement element)
		{
			this.Task.DownloadElement.Add(element); 
		}
		
		public DownloadElement[] DownloadElementToArray(){
			return (DownloadElement[])  this.Task.DownloadElement.ToArray(typeof(DownloadElement));
		}
		
		public bool SaveToXML(String filepath)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(InstallPackage));
			TextWriter textWriter = new StreamWriter(filepath);
			serializer.Serialize(textWriter, this);
			textWriter.Close();
			return true;
		}
		
		public InstallPackage LoadFromXML(String filepath)
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(InstallPackage));
			TextReader textReader = new StreamReader(filepath);
			Object obj = deserializer.Deserialize(textReader);
			InstallPackage myNewSettings = (InstallPackage)obj;
			textReader.Close();
			return myNewSettings;
		}
	}

	public class DownloadElement
	{
		[XmlElement("Subdir")]
		private string subdir = "";
		[XmlElement("FileName")]
		private string fileName = "";
		public string Subdir
		{ 
			get{return subdir;}
			set{subdir = value;}
		}

		public string FileName
		{ 
			get{return fileName;}
			set{fileName = value;}
		}


		public DownloadElement(string fileName, string subdir = "")
		{
			this.subdir = subdir;
			this.fileName = fileName;
		}


		public DownloadElement():this("","")
		{
			
		}
	}

	public class Task
	{
		[XmlArray("Download")]
		[XmlArrayItem("Element", typeof(DownloadElement))]
		public ArrayList DownloadElement{get;set;}
	}
}

