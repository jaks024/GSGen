using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GSGen
{
	public class SSCTag : INotifyPropertyChanged
	{
		public string tag { get; set; }
		public string cssClasses { get; set; }
		public string htmlTag { get; set; }
		[Newtonsoft.Json.JsonIgnore]
		public string fullLine { get => $"tag: {tag}\thtml tag: {htmlTag}\tcss classes: {cssClasses}"; }
		public SSCTag() { }
		public SSCTag(string tag, string htmlTag, string cssClasses)
		{
			this.tag = tag;
			this.htmlTag = htmlTag;
			this.cssClasses = cssClasses;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public void NotifyChanges()
		{
			NotifyPropertyChanged("fullLine");
		}

		public override bool Equals(object obj)
		{
			if ((obj == null) || !GetType().Equals(obj.GetType()))
				return false;

			SSCTag o = (SSCTag)obj;
			return tag.Equals(o.tag);
		}

		public override int GetHashCode()
		{
			return tag.GetHashCode();
		}
	}

}
