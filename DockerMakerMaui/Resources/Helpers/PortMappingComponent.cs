using DockerContainerLogic.Models;

namespace DockerMakerMaui.Resources.Helpers
{
    internal class PortMappingComponent
    {

        public static Grid GeneratePortMapping(PortMapping portMapping)
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
                //this.ports.Remove(portMapping);
                //this.ShowPortMapping();
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

            return grid;
        }
    }
}
