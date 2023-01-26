using DockerContainerLogic;
using DockerContainerLogic.Models;

namespace DockerMakerMaui.Views;

public partial class CreateContainerPage : ContentPage
{
    int count = 0;
    private bool serviceAvailable;
    private readonly DockerInstance dockerInstance;
    private readonly Containers containersClient;
    private readonly Images imagesClient;
    public List<Frame> notifications;

    public CreateContainerPage()
    {
        InitializeComponent();
        this.serviceAvailable = false;
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
            this.AddNotificationMessage($"Unable to reach Docker daemon. Check if it's running: {ex.Message}", true, 3000);
        }
        // Load initial data
        //this.PopulateInfo();
    }

    private async void CheckDockerDaemon()
    {
        var result = await this.dockerInstance.CheckDockerService();

        if (result.IsError == true)
        {
            this.AddNotificationMessage($"Unable to reach Docker daemon. Check if it's running: {result.Message}", true, 3000);
            this.serviceAvailable = false;
        }
        else
        {
            this.serviceAvailable = true;
        }
    }

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
        this.CheckDockerDaemon();
        if (this.serviceAvailable == false)
        {
            return;
        }
        try
        {
            // Add the list of images and containers to the view data
            var images = await this.imagesClient.GetImages();
            var containers = await this.containersClient.GetContainers();

            // Add Images to the Picker
            foreach (var container in containers)
            {
                //this.ContainerPicker.Items.Add(string.Join(',', container.Names));
                Debug.WriteLine(string.Join(',', container.Names));
                this.ContainerPicker.Items.Add(string.Join(',', container.Names));
            }

            // Add Images to the Picker
            foreach (var image in images)
            {
                //this.ImagePicker.Items.Add(image.RepoTags.FirstOrDefault());
                Debug.WriteLine(image.RepoTags.FirstOrDefault());
                this.ImagePicker.Items.Add(image.RepoTags.FirstOrDefault());
            }
        }
        catch (Exception ex)
        {
            this.AddNotificationMessage($"No se pudo recoger la información de imágenes ni contenedores: {ex.Message}", true);
        }
    }

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        this.CheckDockerDaemon();
        if (this.serviceAvailable == false)
        {
            return;
        }
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

    private void OnReconnectClicked(object sender, EventArgs e)
    {
        this.CheckDockerDaemon();
    }

    void OnPickerImagesSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            //monkeyNameLabel.Text = (string)picker.ItemsSource[selectedIndex];
        }
    }

    void OnPickerContainersSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            //monkeyNameLabel.Text = (string)picker.ItemsSource[selectedIndex];
        }
    }
}