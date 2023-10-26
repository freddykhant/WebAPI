using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting; 
using System.Runtime.Remoting.Channels.Http;
using System.ServiceModel;

namespace Client
{
    public partial class MainWindow : Window
    {
        private Thread networkingThread;
        private Thread serverThread;
        private int completedJobsCount = 0;
        private object lockObject = new object();
        private NetworkingThreadStatus networkingStatus = new NetworkingThreadStatus();
        private RestClient client;
        private int? clientId = null;

        public MainWindow()
        {
            InitializeComponent();
            StartThreads();
        }

        private void StartThreads()
        {
            Task.Run(() => NetworkingThreadMethodAsync());

            serverThread = new Thread(new ThreadStart(ServerThreadMethod));
            serverThread.Start();
        }

        private async Task NetworkingThreadMethodAsync()
        {
            while (true)
            {
                try
                {
                    networkingStatus.IsWorking = true;
                    var client = new RestClient("http://localhost:5080");
                    var request = new RestRequest("api/Clients/getAll", Method.Get);
                    var response = await client.ExecuteAsync(request);
                    var clientsList = JsonConvert.DeserializeObject<List<ClientClass>>(response.Content);

                    foreach (var clientInfo in clientsList)
                    {
                        if (clientInfo.Id != clientId) // Ensure we're not checking our own client
                        {
                            // Connect to the .NET Remoting server of the client
                            IRemoteService clientRemoteService = (IRemoteService)Activator.GetObject(
                                typeof(IRemoteService),
                                $"http://{clientInfo.IPAddress}:{clientInfo.Port}/RemoteService");

                            var job = clientRemoteService.GetJob(); 

                            if (job != null && job.Status == "Ready")
                            {
                                string result = ExecutePythonJob(job.Code);
                                UpdateJobStatus(false);

                                // Send the result back to the client that hosted the job
                                ResultTextBox.Text = result;
                                clientRemoteService.SubmitResult(result);

                                // Update the job's status in the database to indicate it has been processed
                                job.Status = "Processed";
                                var updateRequest = new RestRequest($"api/Clients/update/{clientInfo.Id}", Method.Put);
                                updateRequest.AddJsonBody(clientInfo);
                                var updateResponse = client.Execute(updateRequest);
                                // Handle the response, e.g., check if the update was successful
                            }
                        }
                    }
                    networkingStatus.CompletedJobsCount = completedJobsCount;
                }
                catch (Exception ex)
                {
                    LogError($"Error in NetworkingThread: {ex.Message}");
                }
                finally
                {
                    networkingStatus.IsWorking = false;
                }

                await Task.Delay(10000);
            }
        }

        private void ServerThreadMethod()
        {
            try
            {
                // Create and register the channel
                HttpChannel channel = new HttpChannel(int.Parse(portTextBox.Text)); 
                ChannelServices.RegisterChannel(channel, false);

                // Register the RemoteService for .NET Remoting
                RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof(RemoteService),
                    "RemoteService",
                    WellKnownObjectMode.Singleton
                );

                // Keep the server running
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception ex)
            {
                LogError($"Error in ServerThread: {ex.Message}");
            }
        }



        private string ExecutePythonJob(string pythonCode)
        {
            try
            {
                ScriptEngine engine = Python.CreateEngine();
                ScriptScope scope = engine.CreateScope();

                try
                {
                    dynamic result = engine.Execute(pythonCode, scope);
                    return result.ToString();
                }
                catch (Exception ex)
                {
                    return $"Error: {ex.Message}";
                }
            }
            catch (Exception ex)
            {   
                LogError($"Error executing Python code: {ex.Message}");
                return null;
            }
        }

        private void SubmitPythonCodeButton_Click(object sender, RoutedEventArgs e)
        {
            string pythonCode = PythonCodeTextBox.Text;
            string result = ExecutePythonJob(pythonCode);
            ResultTextBox.Text = result;
        }

        private void QueryNetworkingThreadButton_Click(object sender, RoutedEventArgs e)
        {
            if (networkingStatus.IsWorking)
            {
                statusTextBlock.Text = $"Networking thread is currently working. Completed jobs: {networkingStatus.CompletedJobsCount}";
            }
            else
            {
                statusTextBlock.Text = $"Networking thread is idle. Completed jobs: {networkingStatus.CompletedJobsCount}";
            }
        }


        private void UpdateJobStatus(bool isWorking)
        {
            if (isWorking)
            {
                JobStatusTextBox.Text = "Working on a job...";
            }
            else
            {
                completedJobsCount++;
                JobStatusTextBox.Text = $"Completed {completedJobsCount} jobs.";
            }
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string ipAddress = ipAddressTextBox.Text;
            int port = int.Parse(portTextBox.Text);
            JobClass job = new JobClass { Code = PythonCodeTextBox.Text, Status = "Ready" };

            var client = new RestClient("http://localhost:5080");

            // Check if the port is already registered
            var checkPortRequest = new RestRequest($"api/Clients/isPortRegistered/{port}", Method.Get);
            var checkPortResponse = await client.ExecuteAsync(checkPortRequest);
            if (checkPortResponse.IsSuccessful)
            {
                var portStatus = JsonConvert.DeserializeObject<dynamic>(checkPortResponse.Content);
                if (portStatus.status == true)
                {
                    statusTextBlock.Text = $"Error: Port {port} is already registered.";
                    return;
                }
            }
            else
            {
                statusTextBlock.Text = "Error checking port status.";
                return;
            }

            // If the port is not registered, proceed with the registration
            var registerRequest = new RestRequest("api/Clients/register", Method.Post);
            registerRequest.AddJsonBody(new { IPAddress = ipAddress, Port = port, Job = job });

            var registerResponse = client.Execute<ClientClass>(registerRequest);
            if (registerResponse.IsSuccessful)
            {
                clientId = registerResponse.Data.Id;
                statusTextBlock.Text = $"Connected and registered to IP: {ipAddress}, Port: {port}";
            }
            else
            {
                statusTextBlock.Text = "Error registering client.";
            }
        }


        private void SendDataButton_Click(object sender, RoutedEventArgs e)
        {
            string pythonCode = PythonCodeTextBox.Text;
            var client = new RestClient("http://localhost:5080");
            var request = new RestRequest("api/Jobs/submit", Method.Post);
            request.AddJsonBody(new { Code = pythonCode, Status = "Ready" });

            var response = client.Execute(request);
            if (response.IsSuccessful)
            {
                statusTextBlock.Text = "Data sent to the server.";

                var updateClientRequest = new RestRequest($"api/Clients/update/{clientId}", Method.Put);
                updateClientRequest.AddJsonBody(new { Job = new JobClass { Code = pythonCode, Status = "Ready" } });
                var updateResponse = client.Execute(updateClientRequest);
            }
            else
            {
                statusTextBlock.Text = $"Error: {response.StatusCode}. {response.Content}";
            }
        }

        private void ReceiveDataButton_Click(object sender, RoutedEventArgs e)
        {
            var client = new RestClient("http://localhost:5080");
            var request = new RestRequest($"api/Clients/{clientId}", Method.Get); 

            var response = client.Execute<ClientClass>(request);
            if (response.IsSuccessful && response.Data != null && response.Data.Job != null)
            {
                var job = response.Data.Job;
                statusTextBlock.Text = $"Received job with ID {job.Id}: {job.Code}";
                PythonCodeTextBox.Text = job.Code;
            }
            else
            {
                statusTextBlock.Text = "No jobs received from the server.";
            }
        }

        private void LogError(string errorMessage)
        {
            MessageBox.Show(errorMessage);
        }

    }
}
