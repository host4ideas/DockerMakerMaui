﻿using DockerContainerLogic;
using System.Diagnostics;

namespace DockerMakerMaui;

public partial class MainPage : ContentPage
{
    int count = 0;
    private readonly DockerInstance dockerInstance;
    private readonly Containers containersClient;
    private readonly Images imagesClient;
    public List<Frame> notifications;

    public MainPage()
    {
        InitializeComponent();
        // Initialize Docker Client
        try
        {
            DockerInstance.Instance.Initialize();
            this.dockerInstance = DockerInstance.Instance;
            // Dependency injection of the DockerInstance's DockerClient
            this.containersClient = new Containers(this.dockerInstance._client);
            this.imagesClient = new Images(this.dockerInstance._client);

            this.CheckDockerDaemon();
        }
        catch (Exception ex)
        {
            this.AddNotificationMessage($"Unable to reach Docker daemon. Check if it's running: {ex.Message}", true, 10000);
        }
        // Load initial data
        //this.PopulateInfo();
    }

    private async void CheckDockerDaemon()
    {
        var result = await this.dockerInstance.CheckDockerService();

        if (result.IsError == true)
        {
            this.AddNotificationMessage($"Unable to reach Docker daemon. Check if it's running: {result.Message}", true, 10000);
        }
    }

    //private void OnCounterClicked(object sender, EventArgs e)
    //{
    //    count++;

    //    if (count == 1)
    //        CounterBtn.Text = $"Clicked {count} time";
    //    else
    //        CounterBtn.Text = $"Clicked {count} times";

    //    SemanticScreenReader.Announce(CounterBtn.Text);
    //}

    private async void AddNotificationMessage(string message, bool isError, int timeout = 2000)
    {
        var frame = new Frame()
        {
            BackgroundColor = isError ? Colors.Red : Colors.Green,
            Padding = 10,
            Content = new Label()
            {
                Text = message,
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold,
            }
        };

        // Show message
        MessageStack.Children.Add(frame);

        // Delete messsage after timeout
        await Task.Delay(timeout);
        this.MessageStack.Children.Remove(this.MessageStack.Children[0]);
    }

    private async void PopulateInfo()
    {
        try
        {
            //if (this.imagesClient.CheckDockerService() != true)
            //{
            //    this.AddNotificationMessage("Docker daemon not reachable", true);
            //}

            // Add the list of images and containers to the view data
            var images = await this.imagesClient.GetImages();
            var containers = await this.containersClient.GetContainers();

            foreach (var container in containers)
            {
                this.ImagePicker.Items.Add(string.Join(',', container.Names));
            }

            foreach (var image in images)
            {
                this.ContainerPicker.Items.Add(image.RepoTags.FirstOrDefault());
            }
        }
        catch (Exception ex)
        {
            this.AddNotificationMessage($"No se pudo recoger la información de imágenes ni contenedores:<br />{ex.Message}", true);
        }
    }

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        try
        {
            var containers = await this.containersClient.GetContainers();

            this.PopulateInfo();
        }
        catch (Exception)
        {
            Debug.WriteLine("Error");
        }

        //var mappingPortsObject = JsonConvert.DeserializeObject<PortMapping[]>(mappingPorts)!;

        //ResultModel result = await containersClient.CreateContainer(image: image, containerName: containerName, mappingPorts: mappingPortsObject, ct: ct);

        //if (result.IsError == true)
        //{
        //    ViewData["ErrorMessage"] = $"No se pudo crear el contenedor:<br />{result.Message}";
        //    return View("Index");
        //}

        //// Add the list of images and containers to the view data
        //ViewData["images"] = this.imagesClient.GetImages();
        //ViewData["containers"] = this.containersClient.GetContainers();
        //ViewData["Message"] = result.Message;

        //// Render the view, passing the list of images and containers as arguments
        //return View("Index");
    }
}
