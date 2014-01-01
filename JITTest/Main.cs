using System;
using System.IO;

namespace JITTest
{
	class MainClass
	{
		struct JitResult
		{
			public TimeSpan first;
			public TimeSpan second;
			public void print()
			{
				Console.WriteLine("  First run : {0}ms", first.Milliseconds);
				Console.WriteLine("  Second run: {0}ms", second.Milliseconds); 
			}
		}
		
		static JitResult JitTest(Action functionToTest)
		{
			
			DateTime start = DateTime.Now;
			functionToTest();
			DateTime afterFirst = DateTime.Now;
			functionToTest();
			DateTime afterSecond = DateTime.Now;						
			return new JitResult() { first = afterFirst-start, second = afterSecond-afterFirst };
		}
		
		static void FileTest()
		{
			using (StreamWriter sw = new StreamWriter("JITTestFile"))
			{
			  sw.WriteLine("Test data");
			}			
		}
		
		public static void Main (string[] args)
		{
			JitResult consoleTest = JitTest(() => Console.WriteLine("Testing console output...") );			
			JitResult fileAccessTest = JitTest(() => FileTest() );			
			Console.WriteLine("JIT Test");
			Console.WriteLine("Results for console output:");
			consoleTest.print();
			Console.WriteLine("Results for file writing test:");
			fileAccessTest.print();
		}
		
	}
}
