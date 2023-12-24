using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace jkouh
{
    public partial class Form1 : Form
    {
        private readonly SeriesCollection _forecastSeries;
        public Form1()
        {
            InitializeComponent();
            _forecastSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>(),
                    PointGeometrySize = 15
                }
            };
            cartesianChart1.Series = _forecastSeries;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public class MeasureModel
        {
            public System.DateTime DateTime { get; set; }
            public double Value { get; set; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double latitude, longitude;
            if (!double.TryParse(textBox1.Text, out latitude) || !double.TryParse(textBox2.Text, out longitude))
            {
                MessageBox.Show("Please enter valid latitude and longitude.");
                return;
            }
            FetchWeatherForecast(latitude, longitude);
        }
        private void FetchWeatherForecast(double latitude, double longitude)
        {
            try
            {
                string apiUrl = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&cnt=48&units=metric&appid=522939462acdeb28a5f79f10077fc4b8";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.Default))
                {
                    string result = streamReader.ReadToEnd();
                    Root forecastData = JsonConvert.DeserializeObject<Root>(result);
                    UpdateChartWithForecast(forecastData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching weather forecast: " + ex.Message);
            }
        }
        private void UpdateChartWithForecast(Root forecastData)
        {
            _forecastSeries[0].Values.Clear();
            foreach (var item in forecastData.list.Take(16))
            {
                DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(item.dt).DateTime;
                double normalizedAqi = NormalizeAQI(item.main.aqi);
                _forecastSeries[0].Values.Add(new ObservablePoint(dateTime.ToOADate(), normalizedAqi));
            }
        }
        private double NormalizeAQI(int aqi)
        {
            return (double)aqi * 100 / 500;
        }

        private void textBox1_TextChanged(object sender, EventArgs e) //ввести широту
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e) //ввести довготу
        {

        }

        private void cartesianChart1_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }
        public class Components
        {
            public double co { get; set; }
            public int no { get; set; }
            public double no2 { get; set; }
            public double o3 { get; set; }
            public double so2 { get; set; }
            public double pm2_5 { get; set; }
            public double pm10 { get; set; }
            public double nh3 { get; set; }
        }
        public class Coord
        {
            public double lon { get;set; }
            public double lat { get;set; }
        }
        public class Main
        {
            public int aqi { get; set; }
        }
        public class List
        {
            public Main main { get; set; }
            public Components components { get; set; }
            public int dt { get; set; }
        }
        public class Root
        {
            public Coord coord { get; set; }
            public List<List> list { get; set; }
        }
        //class Program
        //{
        //    //public static void Main(string[] args)
        //    //{
        //    //    //  string key = "522939462acdeb28a5f79f10077fc4b8";
        //    //    HttpWebRequest reqw = (HttpWebRequest)HttpWebRequest.Create("https://api.openweathermap.org/data/2.5/air_pollution?lat=47.54&lon=33.22&appid=522939462acdeb28a5f79f10077fc4b8");
        //    //    HttpWebResponse resp = (HttpWebResponse)reqw.GetResponse(); //створюємо об'єкт відгуку
        //    //    StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.Default); //створюємо потік для читання відгуку
        //    //    string res = sr.ReadToEnd();
        //    //    Root root = JsonConvert.DeserializeObject<Root>(res);
        //    //    Console.WriteLine(root.list[0].main.aqi);
        //    //    sr.Close();
        //    //}
        //}
    }
}