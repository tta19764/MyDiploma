using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.Http;

namespace HumanResourcesApp.Classes
{
    public static class IpAddress
    {
        public static string GetLocalIpAddress()
        {
            try
            {
                // Get the host name of the local machine
                string hostName = Dns.GetHostName();

                // Get the IP addresses associated with the local machine
                var ipAddresses = Dns.GetHostAddresses(hostName);

                // Iterate through the IP addresses and return the first IPv4 address
                foreach (var ip in ipAddresses)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork) // Check for IPv4
                    {
                        return ip.ToString();
                    }
                }

                return "No IPv4 address found.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
        public static async Task<string> GetPublicIpAddressAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Send a request to an external service that returns the public IP
                    string publicIp = await client.GetStringAsync("https://api.ipify.org");
                    return publicIp;
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }

}

