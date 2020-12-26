using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApplication2
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly Regex _pattern = new Regex("[^0-9.-]+");
        public MainWindow()
        {
            InitializeComponent();
            Enum.GetNames(typeof(Rates)).ToList().ForEach(item => RightList.Items.Add(item));
            Enum.GetNames(typeof(Rates)).ToList().ForEach(item => LeftList.Items.Add(item));
            //this.Background = new ImageBrush(new BitmapImage(new Uri(@"Img/dollars.png")));
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            
           string tmp = LeftList.SelectionBoxItem.ToString();
           LeftList.Text = RightList.Text;
           RightList.Text = tmp;
        }

        private static bool IsTextHasOnlyNums(string text)
        {
            return !_pattern.ToString().Equals("") && !_pattern.IsMatch(text);
        }
        private void PreviewTextInputHandler(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextHasOnlyNums(e.Text); 
        }

        private void Calculate(object sender, RoutedEventArgs e)
        {
            string left = LeftList.Text;
            string right = RightList.Text;
            if(left.Equals("") || right.Equals("")) return;
            
            string urlPattern = "http://rate-exchange-1.appspot.com/currency?from={0}&to={1}";
            string url = string.Format(urlPattern, left, right);
            string answer;
            using (var wc = new WebClient())
            {
                var json = wc.DownloadString(url);

                Newtonsoft.Json.Linq.JToken token = Newtonsoft.Json.Linq.JObject.Parse(json);
                decimal exchangeRate = (decimal)token.SelectToken("rate");

                answer = (Convert.ToDecimal(FromText.Text) * exchangeRate).ToString(CultureInfo.CurrentCulture);
            }

            ToText.Text = answer;
        }
    }
}