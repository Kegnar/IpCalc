using System.Net;
using System.Net.Sockets;

namespace IpCalc.Model;

public static class IpCalculator
{
    //public static (string Network, string Broadcast, string StartAddr, string EndAddr, string Hosts, string NetworkClass) Calculate(string ip, string mask)
    //{
    //    if (!IPAddress.TryParse(ip, out var IpAddr)) throw new ArgumentException("Некорректный адрес");
    //    if (!IPAddress.TryParse(mask, out var netMask)) throw new ArgumentException("Некорректная маска");

    //    var ipBytes = IpAddr.GetAddressBytes();
    //    var maskBytes = netMask.GetAddressBytes();

    //    if ()

    //        return (networkStr, broadcastStr, startIpStr, endIpStr, hostsStr, networkClassStr);
    //}
    public static (string binaryAddr, string ErrorMsg) AddressAsBinary(string ip)
    {
        try
        {
            if (!IPAddress.TryParse(ip, out var ipAddress)) throw new ArgumentException();

            return (ipAddress.AsBinary(), String.Empty);
        }
        catch (ArgumentException)
        {
            return (String.Empty, "\uE783");
        }
    }

    public static (IPAddress address, string ErrorMsg) ValidateAddress(string ip)
    {
        try
        {
            if (!IPAddress.TryParse(ip, out var ipAddress)) throw new ArgumentException();

            return (ipAddress, String.Empty);
        }
        catch (ArgumentException)
        {
            return (IPAddress.None, "\uE783");
        }
    }
}