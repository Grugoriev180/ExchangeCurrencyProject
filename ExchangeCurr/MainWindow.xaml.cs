using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;


namespace ExchangeCurr
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
	
		public MainWindow()
		{
			InitializeComponent();
		}

		private string selectedCurrency;
		private Currency selectedCurrencyObj;
		
		async Task DownloadCurr()
		{
			try
			{
				await DownloadJsonAsync();

				if (selectedCurrency != "Російський рубль")
					rateOutput.Text = ($"Курс валюти завантажено з офційного сайту НБУ:\n" +
								$"{selectedCurrencyObj.Txt}, {selectedCurrencyObj.cc}, курс - {selectedCurrencyObj.Rate}, дата {selectedCurrencyObj.Exchangedate}");
				else
					rateOutput.Text = ("Виберіть нормальну валюту;)");
			}
			catch (HttpRequestException)
			{
				rateOutput.Text = "Не вдалося завантажити курс валюти. Перевірте підключення до Інтернету.";
			}
			catch (OperationCanceledException)
			{
				rateOutput.Text = "Операцію скасовано.";
			}
		}
		private async Task DownloadJsonAsync()
		{
			var httpClient = new HttpClient();
			var baseAddress = new Uri("https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json");
			var response = await httpClient.GetStringAsync(baseAddress);

			var currencies = JsonConvert.DeserializeObject<List<Currency>>(response);

			selectedCurrencyObj = currencies.FirstOrDefault(c => c.Txt == selectedCurrency);
		}
		async Task CalculateCurr()
		{
			try
			{
				await DownloadJsonAsync();

				if (selectedCurrency != "Російський рубль")
					calculationOutput.Text = $"{decimal.Round(Convert.ToDecimal(inputUAH.Text) / Decimal.Parse(selectedCurrencyObj.Rate, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture), 4, MidpointRounding.ToEven)} {selectedCurrencyObj.cc}";
				else
					rateOutput.Text = ("Виберіть нормальну валюту;)");
			}
			catch (HttpRequestException)
			{
				calculationOutput.Text = "Не вдалося завантажити курс валюти. Перевірте підключення до Інтернету.";
			}
			catch (FormatException)
			{
				calculationOutput.Text = "Введіть коректну суму в гривнях.";
			}
			catch (OperationCanceledException)
			{
				calculationOutput.Text = "Операцію скасовано.";
			}
		}

		#region UI Elements
		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			var comboBox = sender as ComboBox;

			selectedCurrency = comboBox.SelectedItem as string;
		}

		private void ComboBox_Loader(object sender, RoutedEventArgs e)
		{
			List<string> currencyName = new List<string>()
			{
				"Австралійський долар", "Азербайджанський манат", "Алжирський динар", "Бат", "Білоруський рубль", "Болгарський лев",
				"Бразильський реал", "Вірменський драм", "Данська крона", "Дирхам ОАЕ", "Долар США", "Домініканське песо", "Донг",
				"Євро", "Єгипетський фунт", "Єна", "Злотий", "Золото", "Індійська рупія", "Іракський динар", "Іранський ріал",
				"Канадський долар", "Ларі", "Ліванський фунт", "Лівійський динар", "Малайзійський ринггіт", "Марокканський дирхам",
				"Мексиканське песо", "Молдовський лей", "Новий ізраїльський шекель", "Новий тайванський долар", "Новозеландський долар",
				"Норвезька крона", "Пакистанська рупія", "Паладій", "Платина", "Ренд", "Російський рубль", "Румунський лей", "Саудівський ріял",
				"Сербський динар", "Сінгапурський долар", "Сом", "Сомоні", "СПЗ (спеціальні права запозичення)", "Срібло", "Така", "Теньге",
				"Туніський динар", "Турецька ліра", "Туркменський новий манат", "Узбецький сум", "Форинт", "Фунт стерлінгів", "Хонгконгівський долар",
				"Шведська крона", "Швейцарський франк", "Юань Женьміньбі", "Японська єна"
			};

			var comboBox = sender as ComboBox;
			comboBox.ItemsSource = currencyName;
			comboBox.SelectedIndex = 0;
		}
		private void downloadButton_Click(object sender, RoutedEventArgs e)
		{
			DownloadCurr();
		}

		private void calculateButton_Click(object sender, RoutedEventArgs e)
		{
			CalculateCurr();
		}

		private void closeApp_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
		#endregion
	}
}