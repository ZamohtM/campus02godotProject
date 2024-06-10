using System.Net.NetworkInformation;

public static class Utilities
{
    public static bool IsInternetAvailable()
    {
        try
        {
            using (Ping ping = new Ping())
            {
                PingReply reply = ping.Send("8.8.8.8", 1000); // Ping Google's DNS with a timeout of 1000ms
                return reply.Status == IPStatus.Success;
            }
        }
        catch (PingException)
        {
            return false;
        }
    }
}