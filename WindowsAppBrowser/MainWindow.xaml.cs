using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using Microsoft.Management.Infrastructure;

namespace WindowsAppBrowser
{
    /// <summary>
    /// Represents the main window of the application.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            // Extend the window content into the title bar area.
            this.ExtendsContentIntoTitleBar = true;

            // Set the custom title bar element.
            this.SetTitleBar(CustomTitleBar);

            // Attach key down event handler to the address bar.
            AddressBar.KeyDown += AddressBar_KeyDown;

            // Optionally, extend content into the title bar.
            // Note: The method to set up a custom title bar may vary depending on your requirements.

            // Attach key down event handler to the address bar.
            AddressBar.KeyDown += AddressBar_KeyDown;
        }

        /// <summary>
        /// Handles the click event of the "Go" button in the address bar.
        /// Calls ProcessAddress method to validate and mount the CIM virtual disk.
        /// </summary>
        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessAddress();
        }

        /// <summary>
        /// Handles the KeyDown event of the address bar.
        /// Triggers processing when the Enter key is pressed.
        /// </summary>
        private void AddressBar_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ProcessAddress();
            }
        }

        /// <summary>
        /// Processes the address entered in the address bar:
        /// Validate the SMB URL and mount the corresponding CIM virtual disk.
        /// </summary>
        private void ProcessAddress()
        {
            string address = AddressBar.Text.Trim();
            if (string.IsNullOrEmpty(address))
            {
                return;
            }

            // Validate that the address is a valid SMB URL
            if (!IsValidSmbUrl(address))
            {
                // Display error message (for demonstration, simply change the address bar text)
                AddressBar.Text = "Invalid SMB URL";
                return;
            }

            // Attempt to mount the CIM virtual disk using the SMB URL.
            bool mountSuccess = MountCimVirtualDisk(address);
            if (mountSuccess)
            {
                // Create a new TabViewItem showing a success message.
                var newTab = new Microsoft.UI.Xaml.Controls.TabViewItem
                {
                    Header = address,
                    Content = new TextBlock
                    {
                        Text = $"Mounted disk from: {address}",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };

                // Add the new tab to MainTabView and select it.
                MainTabView.TabItems.Add(newTab);
                MainTabView.SelectedItem = newTab;
            }
            else
            {
                // Mounting failed, display error information.
                AddressBar.Text = "Mount failed";
            }
        }

        /// <summary>
        /// Validates whether the provided URL is a legal SMB URL.
        /// </summary>
        /// <param name="url">The URL to validate.</param>
        /// <returns>True if the URL is a valid SMB URL; otherwise, false.</returns>
        private bool IsValidSmbUrl(string url)
        {
            // Check that the url starts with "smb://"
            if (!url.StartsWith("smb://", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Use Uri.TryCreate to further validate the structure.
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult) &&
                   (uriResult.Scheme.Equals("smb", StringComparison.OrdinalIgnoreCase));
        }


// ...

/// <summary>
/// Attempts to mount a CIM virtual disk from the provided SMB URL.
/// This example uses a CIM query to search for a virtual disk instance matching the share name.
/// Adjust the CIM class and query as needed for your environment.
/// </summary>
/// <param name="smbUrl">The SMB URL provided by the user.</param>
/// <returns>True if mount succeeds; otherwise, false.</returns>
private bool MountCimVirtualDisk(string smbUrl)
    {
        try
        {
            // Parse the SMB URL (e.g. "smb://server/share")
            var uri = new Uri(smbUrl);
            string server = uri.Host;
            // Remove leading/trailing '/' from AbsolutePath to extract share name.
            string share = uri.AbsolutePath.Trim('/');

            // Create a CIM session with the remote server.
            using CimSession session = CimSession.Create(server);

            // Construct a query to find a virtual disk instance with a matching share name.
            // Adjust the CIM namespace, class name, and query filter based on your CIM schema.
            string cimNamespace = @"root\microsoft\windows\storage";
            string query = $"SELECT * FROM MSFT_VirtualDisk WHERE ShareName LIKE '%{share}%'";

            // Query instances from the CIM session.
            var instances = session.QueryInstances(cimNamespace, "WQL", query);

            // If a matching instance is found, assume mounting is possible.
            foreach (CimInstance instance in instances)
            {
                // Optionally, invoke a CIM method to "mount" the disk if required.
                // For demonstration, we assume that finding the instance means mount succeeded.
                System.Diagnostics.Debug.WriteLine("Found virtual disk instance: " + instance.CimInstanceProperties["FriendlyName"].Value);
                return true;
            }
        }
        catch (Exception ex)
        {
            // Log the exception or display error details.
            System.Diagnostics.Debug.WriteLine("Error mounting CIM virtual disk: " + ex);
        }
        return false;
    }

}
}
