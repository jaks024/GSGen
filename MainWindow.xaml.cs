using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
namespace GSGen
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		private string lastSelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

		public MainWindow()
		{
			InitializeComponent();

			SSCParser.LoadTags();
			lbTags.ItemsSource = SSCParser.allTags;
		}

		private void LbTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (lbTags.SelectedItem == null)
				return;

			SSCTag tag = (SSCTag)lbTags.SelectedItem;

			if (tag == null)
				return;

			tbSscTag.Text = tag.tag;
			tbHtmlTag.Text = tag.htmlTag;
			tbCssClasses.Text = tag.cssClasses;
		}

		private void BtnChange_Click(object sender, RoutedEventArgs e)
		{
			if (lbTags.SelectedItem == null)
				return;

			SSCTag tag = (SSCTag)lbTags.SelectedItem;
			tag.tag = tbSscTag.Text;
			tag.htmlTag = tbHtmlTag.Text;
			tag.cssClasses = tbCssClasses.Text;
			SSCParser.allTags[lbTags.SelectedIndex].NotifyChanges();
			SSCParser.SerializeTag();
		}

		private void BtnDelete_Click(object sender, RoutedEventArgs e)
		{
			if (lbTags.SelectedItem == null)
				return;

			SSCTag tag = (SSCTag)lbTags.SelectedItem;
			SSCParser.allTags.Remove(tag);
			SSCParser.SerializeTag();
		}

		private void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			if (tbAddSscTag.Text.Length == 0 ||
			tbAddHtmlTag.Text.Length == 0 ||
			tbAddCssClasses.Text.Length == 0)
			{
				MessageBox.Show("New tag cannot have empty fields", "All field must be filled", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}

			SSCParser.allTags.Add(new SSCTag(tbAddSscTag.Text, tbAddHtmlTag.Text, tbAddCssClasses.Text));
			SSCParser.SerializeTag();
			ClearAddFields();
		}

		private void BtnAddClear_Click(object sender, RoutedEventArgs e)
		{
			ClearAddFields();
		}

		private void ClearAddFields()
		{
			tbAddSscTag.Text = "";
			tbAddHtmlTag.Text = "";
			tbAddCssClasses.Text = "";
		}

		private void SelectSSCOnClick(object sender, RoutedEventArgs e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog();
			dialog.InitialDirectory = lastSelectedPath;
			dialog.Filters.Add(new CommonFileDialogFilter("Static Stylized Content (*.ssc)", ".ssc"));
			if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
			{
				lastSelectedPath = System.IO.Path.GetDirectoryName(dialog.FileName);
				tbSelectedSSC.Text = dialog.FileName;
			}
		}

		private void SelectInjectOnClick(object sender, RoutedEventArgs e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog();
			dialog.InitialDirectory = lastSelectedPath;
			dialog.Filters.Add(new CommonFileDialogFilter("Hyper Text Markup Language (*.html)", ".html"));
			if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
			{
				lastSelectedPath = System.IO.Path.GetDirectoryName(dialog.FileName);
				tbInjectPath.Text = dialog.FileName;
			}
		}

		private void BtnInjectConfirm_Click(object sender, RoutedEventArgs e)
		{
			SSCParser.InjectToHtml(tbSelectedSSC.Text, tbInjectPath.Text);
		}
	}
}
