using DockerContainerLogic;
using DockerContainerLogic.Models;

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
            this.AddNotificationMessage($"Unable to reach Docker daemon. Check if it's running: {ex.Message}", true, 10000);
        }
    }

    private async Task<bool> CheckDockerDaemon()
    {
        var result = await this.dockerInstance.CheckDockerService();

        if (result.IsError == true)
        {
            this.AddNotificationMessage($"Unable to reach Docker daemon. Check if it's running: {result.Message}", true, 10000);
            this.serviceAvailable = false;
            return false;
        }
        else
        {
            this.serviceAvailable = true;
            return true;
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

            // Add Images to the Picker
            foreach (var container in containers)
            {
                Debug.WriteLine("*** CONTAINERS ***");
                Debug.WriteLine(string.Join(',', container.Names));
                this.ContainerPicker.Items.Add(string.Join(',', container.Names));
            }
            this.ContainerPicker.SelectedIndex = 0;

            // Add Images to the Picker
            foreach (var image in images)
            {
                Debug.WriteLine("*** IMAGES ***");
                Debug.WriteLine(image.RepoTags.FirstOrDefault());
                this.ImagePicker.Items.Add(image.RepoTags.FirstOrDefault());
            }
            this.ImagePicker.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            this.AddNotificationMessage($"No se pudo recoger la información de imágenes ni contenedores: {ex.Message}", true);
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
            ResultModel result = await containersClient.CreateFormContainer(
                 image: this.ImagePicker.SelectedItem.ToString(),
                 containerName: this.ContainerPicker.SelectedItem.ToString(),
                 mappingPorts: this.ports
             );

            if (result.IsError == true)
            {
                this.AddNotificationMessage($"No se pudo crear el contenedor: {result.Message}", true);
            }

            this.PopulateInfo();
        }
        catch (Exception)
        {
            Debug.WriteLine("Error");
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
                Placeholder = "Eg.: tcp/3000",
            };

            entryContainerPort.TextChanged += (sender, e) => { portMapping.ContainerPort = entryContainerPort.Text; };

            //entryContainerPort.TextChanged += EntryContainerPort_TextChanged;

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
                Placeholder = "Eg.: tcp/3000",
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