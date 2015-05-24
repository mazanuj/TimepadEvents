using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Awesomium.Core;
using TimepadEvents.Annotations;
using TimepadEvents.Http;

namespace TimepadEvents
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public sealed partial class MainWindow : INotifyPropertyChanged
	{
		public MainWindow()
		{
			//var session = WebCore.CreateWebSession("Sessions", WebPreferences.Default);
			StaticSettings.View = WebCore.CreateWebView(0, 0, WebViewType.Offscreen);
			DataItemsList = new ObservableCollection<DataItem>();
			InitializeComponent();

			#region Menu

			MenuItemsList = new List<MenuItem>
			{
				new MenuItem
				{
					Key = "Все события",
					Value = "https://my.timepad.ru/all/events?approved=true&date=&mode=&online=true&paid=true&page="
				},
				new MenuItem
				{
					Key = "Бизнес",
					Value =
						"https://my.timepad.ru/all/categories/biznes/events?approved=true&date=&mode=&online=true&paid=true&page="
				},
				new MenuItem
				{
					Key = "ИТ и интернет",
					Value =
						"https://my.timepad.ru/all/categories/it-i-internet/events?approved=true&date=&mode=&online=true&paid=true&page="
				},
				new MenuItem
				{
					Key = "Иностранные языки",
					Value =
						"https://my.timepad.ru/all/categories/inostrannye-yazyki/events?approved=true&date=&mode=&online=true&paid=true&page="
				},
				new MenuItem
				{
					Key = "Выставки",
					Value =
						"https://my.timepad.ru/all/categories/vystavki/events?approved=true&date=&mode=&online=true&paid=true&page="
				},
				new MenuItem
				{
					Key = "Концерты",
					Value =
						"https://my.timepad.ru/all/categories/kontserty/events?approved=true&date=&mode=&online=true&paid=true&page="
				},
				new MenuItem
				{
					Key = "Театры",
					Value =
						"https://my.timepad.ru/all/categories/teatry/events?approved=true&date=&mode=&online=true&paid=true&page="
				},
				new MenuItem
				{
					Key = "Вечеринки",
					Value =
						"https://my.timepad.ru/all/categories/vecherinki/events?approved=true&date=&mode=&online=true&paid=true&page="
				},
				new MenuItem
				{
					Key = "Экскурсии и путешествия",
					Value =
						"https://my.timepad.ru/all/categories/ekskursii-i-puteshestviya/events?approved=true&date=&mode=&online=true&paid=true&page="
				},
				new MenuItem
				{
					Key = "Еда",
					Value =
						"https://my.timepad.ru/all/categories/eda/events?approved=true&date=&mode=&online=true&paid=true&page="
				},
				new MenuItem
				{
					Key = "Красота и здоровье",
					Value =
						"https://my.timepad.ru/all/categories/krasota-i-zdorovie/events?approved=true&date=&mode=&online=true&paid=true&page="
				},
				new MenuItem
				{
					Key = "Для детей",
					Value =
						"https://my.timepad.ru/all/categories/dlya-detey/events?approved=true&date=&mode=&online=true&paid=true&page="
				},
				new MenuItem
				{
					Key = "Психология и самопознание",
					Value =
						"https://my.timepad.ru/all/categories/psihologiya-i-samopoznanie/events?approved=true&date=&mode=&online=true&paid=true&page="
				},
				new MenuItem
				{
					Key = "Кино",
					Value =
						"https://my.timepad.ru/all/categories/kino/events?approved=true&date=&mode=&online=true&paid=true&page="
				},
				new MenuItem
				{
					Key = "Хобби и творчество",
					Value =
						"https://my.timepad.ru/all/categories/hobbi-i-tvorchestvo/events?approved=true&date=&mode=&online=true&paid=true&page="
				},
				new MenuItem
				{
					Key = "Искусство и культура",
					Value =
						"https://my.timepad.ru/all/categories/iskusstvo-i-kultura/events?approved=true&date=&mode=&online=true&paid=true&page="
				},
				new MenuItem
				{
					Key = "Другие развлечения",
					Value =
						"https://my.timepad.ru/all/categories/drugie-razvlecheniya/events?approved=true&date=&mode=&online=true&paid=true&page="
				},
				new MenuItem
				{
					Key = "Другие события",
					Value =
						"https://my.timepad.ru/all/categories/drugie-sobytiya/events?approved=true&date=&mode=&online=true&paid=true&page="
				}
			};

			#endregion

			ComBoxEvents.ItemsSource = MenuItemsList;
			ComBoxEvents.DisplayMemberPath = "Key";
			DataGrid.ItemsSource = DataItemsList;
		}

		public ObservableCollection<DataItem> DataItemsList { get; set; }

		private static List<MenuItem> MenuItemsList { get; set; }

		private void LaunchOnlineTranslationsOnGitHub(object sender, RoutedEventArgs e)
		{
			Process.Start("https://github.com/mazanuj/TimepadEvents");
		}

		private async Task AddItemToDataGrid(DataItem item)
		{
			await Application.Current.Dispatcher.BeginInvoke(
				new Action(
					() => DataItemsList.Insert(0, item)));
		}

		private async Task MainCyrcle(FormSettings formSettings)
		{
			var eventsLinks = await HttpHelper.GetEventsLinks(formSettings.Event.Value, formSettings.Depth);

			//await HttpHelper.PostFeedback(eventsLinks[0], formSettings);
			foreach (var eventsLink in eventsLinks)
				await AddItemToDataGrid(await HttpHelper.PostFeedback(eventsLink, formSettings));
		}

		private async void ButtonStart_OnClick(object sender, RoutedEventArgs e)
		{
			ControlsIsEnabled(false);
			await MainCyrcle(GetFormSettings());
			ControlsIsEnabled(true);
		}

		private void ControlsIsEnabled(bool value)
		{
			TextBoxAntigate.IsEnabled =
				TextBoxEmail.IsEnabled =
					TextBoxMessage.IsEnabled =
						TextBoxName.IsEnabled =
							ButtonStart.IsEnabled =
								NumericUpDown.IsEnabled =
									ComBoxEvents.IsEnabled = value;
		}

		private FormSettings GetFormSettings()
		{
			return new FormSettings
			{
				AntigateKey = TextBoxAntigate.Text,
				Depth = NumericUpDown.Value == null ? 0 : (int) NumericUpDown.Value,
				Name = TextBoxName.Text,
				Email = TextBoxEmail.Text,
				Message = TextBoxMessage.Text,
				Event = ComBoxEvents.SelectedItem as MenuItem
			};
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public sealed class FormSettings
	{
		public string AntigateKey { get; set; }
		public MenuItem Event { get; set; }
		public int Depth { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Message { get; set; }
	}

	public sealed class MenuItem
	{
		public string Key { get; set; }
		public string Value { get; set; }
	}

	public sealed class DataItem : INotifyPropertyChanged
	{
		public string ID { get; set; }
		public DateTime Date { get; set; }
		public string Name { get; set; }
		private bool status;

		public bool Status
		{
			get { return status; }
			set
			{
				status = value;
				NotifyOfPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		private void NotifyOfPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public static class StaticSettings
	{
		public static WebView View { get; set; }
	}
}