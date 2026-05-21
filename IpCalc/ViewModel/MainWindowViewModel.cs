using System.Net;
using IpCalc.Model;

namespace IpCalc.ViewModel;

public class MainWindowViewModel : BaseViewModel
{

    public string InputIp
    {
        get => field;
        set
        {
            if (SetField(ref field, value))
            {
                var (address, error) = IpCalculator.ValidateAddress(value);
                ErrorMsg=error;
                if (string.IsNullOrEmpty(error))
                {
                    BinaryIp = address.AsBinary();
                }
                else
                {
                    BinaryIp = string.Empty;
                }
            }
        }
    }


    public string BinaryIp
    {
        get => field;
        set => SetField(ref field, value);
    }
    public string BinaryMask { get; set; }

    public string NetworkAddr { get; set; }
    public string BroadcastAddr { get; set; }
    public string StartAddr { get; set; }
    public string EndAddr { get; set; }
    public string HostCount { get; set; }
    public string NetworkClass { get; set; }

    public string ErrorMsg
    {
        get => field;
        set => SetField(ref field, value);
    }



}