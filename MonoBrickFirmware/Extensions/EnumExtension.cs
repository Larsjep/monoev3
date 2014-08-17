using System;

namespace MonoBrickFirmware.Extensions
{
	/// <summary>
	/// Extensions to get next or previous enum
	/// </summary>
	public static class EnumExtensions
	{

	    public static T Next<T>(this T src) where T : struct
	    {
	        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argumnent {0} is not an Enum", typeof(T).FullName));
	
	        T[] Arr = (T[])Enum.GetValues(src.GetType());
	        int j = Array.IndexOf<T>(Arr, src) + 1;
	        return (Arr.Length==j) ? Arr[0] : Arr[j];            
	    }
	    
	    public static T Previous<T>(this T src) where T : struct
	    {
	        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argumnent {0} is not an Enum", typeof(T).FullName));
	
	        T[] Arr = (T[])Enum.GetValues(src.GetType());
	        int j = Array.IndexOf<T>(Arr, src) -1;
	        return (j < 0) ? Arr[Arr.Length-1] : Arr[j];            
	    }
	}
}

