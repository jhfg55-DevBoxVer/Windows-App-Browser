using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;

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

        /// <summary>
        /// Simulates mounting a CIM virtual disk from the provided SMB URL.
        /// Replace with actual mounting logic as needed.
        /// </summary>
        /// <param name="smbUrl">The SMB URL provided by the user.</param>
        /// <returns>True if mount succeeds; otherwise, false.</returns>
        private bool MountCimVirtualDisk(string smbUrl)
        {
            // TODO: Implement the actual mounting logic.
            // This is a placeholder implementation for demonstration.
            return true;
        }
    }
}
