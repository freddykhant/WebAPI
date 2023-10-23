using Newtonsoft.Json;
using RestSharp;
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

namespace Client
{
    public partial class MainWindow : Window
    {
        private RestClient client;
        private string serverUrl = "http://localhost:5080"; // Replace with your server's URL

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Initialize RestClient with the server URL
                client = new RestClient(serverUrl);

                // Test connection to the server
                var request = new RestRequest("your_endpoint_here", Method.Get); // Replace with an appropriate endpoint
                var response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    statusTextBlock.Text = "Connected successfully!";
                }
                else
                {
                    statusTextBlock.Text = "Failed to connect.";
                }
            }
            catch (Exception ex)
            {
                statusTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private void SendDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Assuming you're sending IP and Port as JSON data
                var requestData = new
                {
                    IPAddress = ipAddressTextBox.Text,
                    Port = int.Parse(portTextBox.Text)
                };

                var request = new RestRequest("your_endpoint_here", Method.Post); // Replace with an appropriate endpoint
                request.AddJsonBody(requestData);
                var response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    statusTextBlock.Text = "Data sent successfully!";
                }
                else
                {
                    statusTextBlock.Text = "Failed to send data.";
                }
            }
            catch (Exception ex)
            {
                statusTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private void ReceiveDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var request = new RestRequest("your_endpoint_here", Method.Get); // Replace with an appropriate endpoint
                var response = client.Execute(request);

                if (response.IsSuccessful)
                {
                    // Deserialize the received data if needed
                    var data = JsonConvert.DeserializeObject(response.Content);
                    statusTextBlock.Text = $"Received data: {data}";
                }
                else
                {
                    statusTextBlock.Text = "Failed to receive data.";
                }
            }
            catch (Exception ex)
            {
                statusTextBlock.Text = $"Error: {ex.Message}";
            }
        }
    }
}
