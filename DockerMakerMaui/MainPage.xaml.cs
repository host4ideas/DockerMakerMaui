using Docker.DotNet;
using Docker.DotNet.Models;

namespace DockerMakerMaui;

public partial class MainPage : ContentPage
{
	int count = 0;
    private readonly DockerClient client;

    public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}

    /// <summary>
    /// Method <c>Index</c> executed on view creation.
    /// </summary>
    private void LoadDocker()
    {
        Boolean error = false;
        string message;
        IList<ImagesListResponse> imagesResponse;
        IList<ContainerListResponse> containersResponse;

        try
        {
            // try to ping Doccker daemon through Docker Client --> raises DockerApiException
            client.System.PingAsync().Wait();

            message = "Docker se está ejecutando con normalidad";

            // Obtain the list of images available on the Docker server
            imagesResponse = client.Images.ListImagesAsync(new ImagesListParameters()).Result;

            // Obtain the list of containers on the Docker server
            containersResponse = client.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true
            }).Result;
        }
        catch (Exception ex)
        {
            message = $"No se pudo conectar al Docker daemon. Compruebe que Docker se está ejecutando con normalidad:<br />{ex.Message}";
        }
    }
}

