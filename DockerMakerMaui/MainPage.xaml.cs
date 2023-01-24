using Docker.DotNet;
using Docker.DotNet.Models;
using DockerContainerLogic;
using System.Runtime.InteropServices;

namespace DockerMakerMaui;

public partial class MainPage : ContentPage
{
    int count = 0;
    private readonly Images imagesClient;
    private readonly Containers containersClient;

    public MainPage()
    {
        InitializeComponent();
        // Initialize Docker Client
        this.imagesClient = new();
        this.containersClient = new();
        this.PopulateInfo();
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

    private void PopulateInfo()
    {
        try
        {
            //if (this.imagesClient.CheckDockerService() != true)
            //{
            //    this.AddNotificationMessage("Docker daemon not reachable", true);
            //}

            // Add the list of images and containers to the view data
            var images = this.imagesClient.GetImages();
            var containers = this.containersClient.GetContainers();

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

    private void OnCreateClicked(object sender, EventArgs e)
    {
        var IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        string DockerApiUri()
        {
            if (IsWindows)
                return "npipe://./pipe/docker_engine";

            if (IsLinux)
                return "unix:///var/run/docker.sock";

            throw new Exception(
                "Was unable to determine what OS this is running on, does not appear to be Windows or Linux!?");
        }

        // Agregar el cliente de Docker como un servicio
        var client =  new DockerClientConfiguration(
             new Uri(DockerApiUri()))
              .CreateClient();

        var images = client.Images.ListImagesAsync(new ImagesListParameters()).Result;
        var containers = client.Containers.ListContainersAsync(new ContainersListParameters
        {
            All = true
        }).Result;

        foreach (var container in containers)
        {
            this.ImagePicker.Items.Add(string.Join(',', container.Names));
        }

        foreach (var image in images)
        {
            this.ContainerPicker.Items.Add(image.RepoTags.FirstOrDefault());
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
