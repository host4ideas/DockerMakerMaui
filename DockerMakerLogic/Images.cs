using Docker.DotNet;
using Docker.DotNet.Models;

namespace DockerContainerLogic
{
    public class Images
    {
        // Dependency injection
        private readonly DockerClient _client;

        public Images(DockerClient client)
        {
            _client = client;

        }
        public async Task<IList<ImagesListResponse>> GetImages()
        {
            var images = await this._client.Images.ListImagesAsync(new ImagesListParameters());
            return images;
        }
    }
}
