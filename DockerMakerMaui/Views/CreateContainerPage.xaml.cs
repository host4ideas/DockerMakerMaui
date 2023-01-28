using DockerContainerLogic;
using DockerContainerLogic.Models;
using DockerMakerMaui.Resources.Helpers;

namespace DockerMakerMaui.Views;

public partial class CreateContainerPage : ContentPage
{
    private bool serviceAvailable;
    private readonly DockerInstance dockerInstance;
    private readonly Containers containersClient;
    private readonly Images imagesClient;
    public List<Frame> notifications;
    private List<PortMapping> ports;

    public CreateContainerPage()
    {
        InitializeComponent();
        this.serviceAvailable = false;
        // Instantiate MappingPort list with a Port
        this.ports = new()
        {
            new PortMapping() { ContainerPort = "", HostPort = "" }
        };

        this.ShowPortMapping();
        // Initialize Docker Client
        try
        {
            DockerInstance.Instance.Initialize();
            this.dockerInstance = DockerInstance.Instance;
            // Dependency injection of the DockerInstance's DockerClient
            this.containersClient = new Containers(this.dockerInstance._client);
            this.imagesClient = new Images(this.dockerInstance._client);

            this.PopulateInfo();
        }
        catch (Exception ex)
        {
            AppNotification.AddNotificationMessage($"Unable to reach Docker daemon. Check if it's running: {ex.Message}", true, MessageStack, 10000);
        }
    }

    private async Task<bool> CheckDockerDaemon()
    {
        var result = await this.dockerInstance.CheckDockerService();

        if (result.IsError == true)
        {
            AppNotification.AddNotificationMessage($"Unable to reach Docker daemon. Check if it's running: {result.Message}", true, MessageStack, 10000);
            this.serviceAvailable = false;
            return false;
        }
        else
        {
            this.serviceAvailable = true;
            return true;
        }
    }

    private async void PopulateInfo()
    {
        // Exit if Docker is not running
        if (await this.CheckDockerDaemon() == false)
        {
            return;
        }
        try
        {
            // Add the list of images and containers to the view data
            var images = await this.imagesClient.GetImages();
            var containers = await this.containersClient.GetContainers();

            var containerList = new List<string>();
            var imageList = new List<string>();

            // Add Images to the Picker
            this.ContainerPicker.ItemsSource = null;
            foreach (var container in containers)
            {
                containerList.Add(string.Join(',', container.Names));
                this.ContainerPicker.Items.Add(string.Join(',', container.Names));
            }
            this.ContainerPicker.ItemsSource = containerList;
            this.ContainerPicker.SelectedIndex = 0;

            // Add Images to the Picker
            this.ImagePicker.ItemsSource = null;
            foreach (var image in images)
            {
                this.ImagePicker.Items.Add(image.RepoTags.FirstOrDefault());
                imageList.Add(image.RepoTags.FirstOrDefault());
            }
            this.ImagePicker.ItemsSource = imageList;
            this.ImagePicker.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            AppNotification.AddNotificationMessage($"No se pudo recoger la información de imágenes ni contenedores: {ex.Message}", true, MessageStack);
        }
    }

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        if (await this.CheckDockerDaemon() == false)
        {
            return;
        }
        try
        {
            Debug.WriteLine(this.ports[0].ContainerPort);
            Debug.WriteLine(this.ports[0].HostPort);

            ResultModel result = await containersClient.CreateFormContainer(
                 image: this.ImagePicker.SelectedItem.ToString(),
                 containerName: this.ContainerNameEntry.Text,
                 mappingPorts: this.ports
             );

            if (result.IsError == true)
            {
                AppNotification.AddNotificationMessage($"No se pudo crear el contenedor: {result.Message}", true, MessageStack);
            }

            this.PopulateInfo();
        }
        catch (Exception ex)
        {
            AppNotification.AddNotificationMessage("Error craeting container: " + ex.Message, true, MessageStack);
        }
    }

    private void OnReconnectClicked(object sender, EventArgs e)
    {
        this.PopulateInfo();
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

    private void ShowPortMapping()
    {
        this.PortMappingStack.Children.Clear();
        foreach (var portMapping in this.ports)
        {
            /*
                Grid
             */
            Grid grid = new()
            {
                Margin = 5,
                RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) },
            },
                ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) },
                new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) },
                new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) },
            }
            };

            /*
                Button delete
             */
            Button button = new()
            {
                HeightRequest = 40,
                CornerRadius = 5,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = Colors.Red,
                TextColor = Colors.Black,
                Text = "Delete",
            };
            button.Clicked += delegate (Object o, EventArgs e)
            {
                this.ports.Remove(portMapping);
                this.ShowPortMapping();
            };
            Grid.SetRow(button, 0);
            Grid.SetColumn(button, 0);
            grid.Children.Add(button);

            /*
                Container Port 
            */
            Entry entryContainerPort = new()
            {
                Text = portMapping.ContainerPort,
                HeightRequest = 40,
                Placeholder = "Eg.: 3000/tcp",
            };

            entryContainerPort.TextChanged += (sender, e) => { portMapping.ContainerPort = entryContainerPort.Text; };

            Frame frameContainerPort = new()
            {
                Padding = 0,
                HeightRequest = 50,
                CornerRadius = 5,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Content = entryContainerPort
            };

            Grid.SetRow(frameContainerPort, 0);
            Grid.SetColumn(frameContainerPort, 1);
            grid.Add(frameContainerPort);

            /*
                Host Port 
            */
            Entry entryHostPort = new()
            {
                Text = portMapping.HostPort,
                HeightRequest = 40,
                Placeholder = "Eg.: 8080",
            };

            entryHostPort.TextChanged += (sender, e) => { portMapping.HostPort = entryHostPort.Text; };
            Frame frameHostPort = new()
            {
                Padding = 0,
                HeightRequest = 50,
                CornerRadius = 5,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Content = entryHostPort
            };
            Grid.SetRow(frameHostPort, 0);
            Grid.SetColumn(frameHostPort, 2);
            grid.Add(frameHostPort);

            this.PortMappingStack.Children.Add(grid);
        }
    }

    void OnAddPortMappingClicked(object sender, EventArgs e)
    {
        this.ports.Add(new PortMapping() { ContainerPort = "", HostPort = "" });
        this.ShowPortMapping();
    }
}