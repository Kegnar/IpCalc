using System.Net;

namespace IpCalc.Model;

public static class IpAddressExtensions
{
/// <summary>
/// Двоичное представление адреса
/// </summary>
    public static string AsBinary(this IPAddress ipAddress)
    {
        byte[] addressBytes = ipAddress.GetAddressBytes();
        return string.Join(".", addressBytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
    }

}