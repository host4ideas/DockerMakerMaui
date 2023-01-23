using System.Runtime.InteropServices;
using Docker.DotNet;

namespace DockerMakerMaui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        //var IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        //var IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        //string DockerApiUri()
        //{
        //    if (IsWindows)
        //        return "npipe://./pipe/docker_engine";

        //    if (IsLinux)
        //        return "unix:///var/run/docker.sock";

        //    throw new Exception(
        //        "Was unable to determine what OS this is running on, does not appear to be Windows or Linux!?");
        //}

        //// Agregar el cliente de Docker como un servicio
        //DockerClient client = new DockerClientConfiguration(
        //    new Uri(DockerApiUri()))
        //     .CreateClient();

        //System.Diagnostics.Debug.WriteLine(client);

        //builder.Services.AddSingleton(client);

        return builder.Build();
	}
}
