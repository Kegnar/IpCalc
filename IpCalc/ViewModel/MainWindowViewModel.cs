using System.Net;

namespace IpCalc.ViewModel;

public class MainWindowViewModel : BaseViewModel
{
    public string BinaryIp { get; set; }
    public string BinaryMask { get; set; }
    public string NetworkAddr { get; set; }
    public string BroadcastAddr { get; set; }
    public string StartAddr { get; set; }
    public string EndAddr { get; set; }
    public string HostCount { get; set; }
    public string NetworkClass { get; set; }
    public string IpString { get; set; }
    public string MaskString { get; set; }
}