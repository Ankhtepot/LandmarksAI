using LandmarksAI.Classes;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
using Microsoft.Azure.CognitiveServices.Vision.CustomVision;

namespace LandmarksAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string url = "https://landmarksresource.cognitiveservices.azure.com/customvision/v3.0/Prediction/76410fb3-efec-4824-b1e0-40699ee10fcc/classify/iterations/LandmarksModel1/image";
        const string predictionKey = "fc73221a1ee4450b868600998f230a47";
        const string fileContentType = "application/octet-stream";
        const string urlContentType = "application/json";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void selectImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.png; *.jpg)|*.png;*.jpg;*.jpeg|All files (*.*)|(*.*)";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if(dialog.ShowDialog() == true)
            {
                string fileName = dialog.FileName;

                selectedImage.Source = new BitmapImage(new Uri(fileName));

                MakePredictionFromFileAsync(fileName);
            }
        }

        private async void MakePredictionFromFileAsync(string fileName)
        {
            var file = File.ReadAllBytes(fileName);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Prediction-Key", predictionKey);
                
                using(var content = new ByteArrayContent(file))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue(fileContentType);
                    var response = await client.PostAsync(url, content);

                    var responseString = await response.Content.ReadAsStringAsync();

                    List<Prediction> predictions = JsonConvert.DeserializeObject<CustomVision>(responseString).Predictions.ToList();

                    PredictionsListView.ItemsSource = predictions;
                }
            }
        }

        private async void MakePredictionFromURLAsync(string imageUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Prediction-Key", predictionKey);

                var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("Url", imageUrl)});
                content.Headers.ContentType = new MediaTypeHeaderValue(urlContentType);

                var response = await client.PostAsync(url, content);

                var responseString = await response.Content.ReadAsStringAsync();

                List<Prediction> predictions = null;

                if (response.IsSuccessStatusCode)
                {
                    predictions = JsonConvert.DeserializeObject<CustomVision>(responseString).Predictions.ToList(); 
                }

                PredictionsListView.ItemsSource = predictions;
            }
        }

        private void selectImageUrlButton_Click(object sender, RoutedEventArgs e)
        {
            MakePredictionFromURLAsync("https://upload.wikimedia.org/wikipedia/commons/0/0c/GoldenGateBridge-001.jpg");
        }
    }
}
