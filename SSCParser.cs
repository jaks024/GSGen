using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
namespace GSGen
{
	public class SSCParser
	{
		private static string userTagPath { get => $"{AppDomain.CurrentDomain.BaseDirectory}\\userTags.ssc"; }
		public static ObservableCollection<SSCTag> allTags { get; private set; } = new ObservableCollection<SSCTag>()
		//{
		//	new SSCTag("##HEADER", "div", "sg-head sg-div sg-span sg-custom"),
		//	new SSCTag("##SUBHEAD", "div", "sg-subhead"),
		//	new SSCTag("##PARA",   "div", "sg-para"),
		//	new SSCTag("##PARA1",  "div", "sg-para1")
		//}
		;

		public static void LoadTags()
		{
			string input;
			try
			{
				input = File.ReadAllText(userTagPath);
			}
			catch (Exception)
			{
				MessageBox.Show("File not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			allTags = new ObservableCollection<SSCTag>(JsonConvert.DeserializeObject<SSCTag[]>(input));
		}

		public static void SerializeTag()
		{
			string content = JsonConvert.SerializeObject(allTags, Formatting.Indented);
			File.WriteAllText(userTagPath, content);
		}

		private static string ParseFile(string path)
		{
			string[] input;
			try
			{
				input = File.ReadAllLines(path);
			}
			catch (Exception)
			{
				MessageBox.Show("File not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return "";
			}

			StringBuilder generated = new StringBuilder();

			int prevIndex = -1;
			int prevTagIndex = -1;
			int tagIndex;
			SSCTag tempTag = new SSCTag();
			for(int i = 0; i < input.Length; i++)
			{
				tempTag.tag = input[i];
				tagIndex = allTags.IndexOf(tempTag);
				if (tagIndex != -1)
				{
					if (i != 0)
					{
						generated.Append(BlockPatcher(prevIndex, i, prevTagIndex, input));
					}
					prevTagIndex = tagIndex;
					prevIndex = i;
				}
			}
			generated.Append(BlockPatcher(prevIndex, input.Length, prevTagIndex, input));
			return generated.ToString();
		}

		private static string BlockPatcher(int start, int end, int tagIndex, string[] input)
		{
			StringBuilder content = new StringBuilder();
			for (int x = start + 1; x < end; x++)
			{
				if (input[x].Length == 0)
				{
					content.AppendLine("<br>");
					continue;
				}
				content.AppendLine(input[x]).Append("<br>\n");
			}

			if (tagIndex >= allTags.Count || tagIndex < 0)
				return "";
			SSCTag tag = allTags[tagIndex];

			string braceStart = $"<{tag.htmlTag} class=\"{string.Join(" ", tag.cssClasses)}\">";
			string braceEnd = $"</{tag.htmlTag}>";

			return $"{braceStart}\n{content}{braceEnd}\n";
		}

		public static void InjectToHtml(string sscPath, string injectPath)
		{
			MessageBox.Show("The file to be injected must have ##INJECT marked for this operation to work. Are you ready?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

			string content;
			try
			{
				content = File.ReadAllText(injectPath);
			}
			catch (Exception)
			{
				MessageBox.Show("File not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			string generated = ParseFile(sscPath);
			content = content.Replace("##INJECT", generated);
			File.WriteAllText(injectPath, content);
			MessageBox.Show("Generated texts has been injected", "Operation Successful", MessageBoxButton.OK, MessageBoxImage.Information);
		}
	}

}
