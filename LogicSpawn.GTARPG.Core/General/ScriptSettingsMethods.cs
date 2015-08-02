using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GTA;

namespace LogicSpawn.GTARPG.Core.General
{
    public static class ScriptSettingsMethods
    {
         public static bool Save (this ScriptSettings settings)
         {
             Dictionary<String, List<Tuple<String, String> > >  result = new Dictionary<String  , List<Tuple<String, String>>>();

             var mValues = (Dictionary<String, String>)typeof (ScriptSettings).GetField("mValues", BindingFlags.NonPublic).GetValue(settings);

		foreach (KeyValuePair<String  , String  > data in mValues)
		{
			String  key = data.Key.Substring(data.Key.IndexOf("]", StringComparison.Ordinal) + 1);
			String  section = data.Key.Remove(data.Key.IndexOf("]", StringComparison.Ordinal)).Substring(1);

			if (!result.ContainsKey(section))
			{
				List<Tuple<String,String>>  values = new List<Tuple<String,String>>();
				values.Add(new Tuple<String  , String  >(key, data.Value));
				result.Add(section, values);
			}
			else
			{
				result[section].Add(new Tuple<String  , String  >(key, data.Value));
			}
		}

		StreamWriter writer = null;

		try
		{
            var mFileName = (string)typeof (ScriptSettings).GetField("mFileName", BindingFlags.NonPublic).GetValue(settings);
			writer = File.CreateText(mFileName);
		}
		catch (IOException)
		{
			return false;
		}

		try
		{
			foreach (KeyValuePair<String  , List<Tuple<String  , String  >  >  > section in result)
			{
				writer.WriteLine("[" + section.Key + "]");

				foreach (Tuple<String  , String  >  value in section.Value)
				{
					writer.WriteLine(value.Item1 + " = " + value.Item2);
				}

				writer.WriteLine();
			}
		}
		catch (IOException  )
		{
			return false;
		}
		finally
		{
			writer.Close();
		}

		return true;
         }
    }
}