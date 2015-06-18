using System;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace MonoBrickFirmware.Tools
{
	public static class XmlHelper
	{
		public static XmlSerializer CreateSerializer(Type t)
		{
			string XmlAssembly = Path.GetFileNameWithoutExtension(t.Assembly.Location) + ".XmlSerializers";
			try
			{
				var asm = Assembly.Load(XmlAssembly);
				XmlSerializerImplementation imp =  (XmlSerializerImplementation)asm.CreateInstance("Mono.GeneratedSerializers.Literal.XmlSerializerContract");		
				XmlSerializer serializer = imp.GetSerializer(t);				
				return serializer;
			} 
			catch (Exception)
			{
				Console.WriteLine("Error loading Xml serializer from {0}.dll", XmlAssembly);

				return new XmlSerializer(t);
			}
		}
		
	}
}

