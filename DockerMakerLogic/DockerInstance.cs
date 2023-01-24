using Docker.DotNet;
using DockerContainerLogic.Models;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DockerContainerLogic
{
    public class DockerInstance
    {
        private static DockerInstance _instance;
        public DockerClient _client;

        private DockerInstance()
        {
        }

        public static DockerInstance Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DockerInstance();
                }
                return _instance;
            }
        }

        #region METHODS 
        public DockerClient Initialize()
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

            try
            {
                // Agregar el cliente de Docker como un servicio
                _client = new DockerClientConfiguration(
                     new Uri(DockerApiUri()))
                      .CreateClient();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("****************************************************************");
                Debug.WriteLine(ex);
                Debug.WriteLine("****************************************************************");
            }
            return _client;
        }

        public async Task<ResultModel> CheckDockerService()
        {
            try
            {
                await this._client.System.PingAsync();
                return new ResultModel("Daemon is running");
            }
            catch (Exception ex)
            {
                return new ResultModel(ex.Message, true);
            }
        }
        #endregion
    }
}
