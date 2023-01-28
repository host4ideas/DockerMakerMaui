using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockerMakerMaui.Resources.Helpers
{
    internal class AppNotification
    {
        public static async void AddNotificationMessage(string message, bool isError, StackLayout parent, int timeout = 5000)
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
            parent.Children.Add(frame);

            // Delete messsage after timeout
            await Task.Delay(timeout);
            parent.Children.Remove(frame);
        }
    }
}
