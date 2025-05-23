using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

class EthernetTamperDetector
{
    static string expectedIPAddress = null;
    static string interfaceName = null;

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine(@"                    uuuuuuuuuuuuuuuuuuuuu.
                   .u$$$$$$$$$$$$$$$$$$$$$$$$$$W.
                 u$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$Wu.
               $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$i
              $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
         `    $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
           .i$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$i
           $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$W
          .$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$W
         .$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$i
         #$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$.
         W$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
$u       #$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$~
$#      `""$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
$i        $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
$$        #$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
$$         $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
#$.        $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$#
 $$      $iW$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$!
 $$i      $$$$$$$#"""" `""""""#$$$$$$$$$$$$$$$$$#""""""""""""#$$$$$$$$$$$$$$$W
 #$$W    `$$$#""            ""       !$$$$$`           `""#$$$$$$$$$$#
  $$$     ``                 ! !iuW$$$$$                 #$$$$$$$#
  #$$    $u                  $   $$$$$$$                  $$$$$$$~
   ""#    #$$i.               #   $$$$$$$.                 `$$$$$$
          $$$$$i.                """"""#$$$$i.               .$$$$#
          $$$$$$$$!         .   `    $$$$$$$$$i           $$$$$
          `$$$$$  $iWW   .uW`        #$$$$$$$$$W.       .$$$$$$#
            ""#$$$$$$$$$$$$#`          $$$$$$$$$$$iWiuuuW$$$$$$$$W
               !#""""    """"             `$$$$$$$##$$$$$$$$$$$$$$$$
          i$$$$    .                   !$$$$$$ .$$$$$$$$$$$$$$$#
         $$$$$$$$$$`                    $$$$$$$$$Wi$$$$$$#""#$$`
         #$$$$$$$$$W.                   $$$$$$$$$$$#   ``
          `$$$$##$$$$!       i$u.  $. .i$$$$$$$$$#""""
             ""     `#W       $$$$$$$$$$$$$$$$$$$`      u$#
                            W$$$$$$$$$$$$$$$$$$      $$$$W
                            $$`!$$$##$$$$``$$$$      $$$$!
                           i$"" $$$$  $$#""`  """"""     W$$$$
                                                   W$$$$!
                      uW$$  uu  uu.  $$$  $$$Wu#   $$$$$$
                     ~$$$$iu$$iu$$$uW$$! $$$$$$i .W$$$$$$
             ..  !   ""#$$$$$$$$$$##$$$$$$$$$$$$$$$$$$$$#""
             $$W  $     ""#$$$$$$$iW$$$$$$$$$$$$$$$$$$$$$W
             $#`   `       """"#$$$$$$$$$$$$$$$$$$$$$$$$$$$
                              !$$$$$$$$$$$$$$$$$$$$$#`
                              $$$$$$$$$$$$$$$$$$$$$$!
                            $$$$$$$$$$$$$$$$$$$$$$$`
                             $$$$$$$$$$$$$$$$$$$$""

███    ██ ██  ██████    ███████ ███    ██ ██ ████████  ██████ ██   ██ 
████   ██ ██ ██         ██      ████   ██ ██    ██    ██      ██   ██ 
██ ██  ██ ██ ██         ███████ ██ ██  ██ ██    ██    ██      ███████ 
██  ██ ██ ██ ██              ██ ██  ██ ██ ██    ██    ██      ██   ██ 
██   ████ ██  ██████ ██ ███████ ██   ████ ██    ██     ██████ ██   ██ 
                                                                        v1.0
Network Interface Cable Snitch - Detects Unplug Events + IP Change
-------------------------------------------------------------------
");

        var ethInterface = NetworkInterface
            .GetAllNetworkInterfaces()
            .FirstOrDefault(ni =>
                ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                ni.OperationalStatus == OperationalStatus.Up);

        if (ethInterface == null)
        {
            Console.WriteLine("⚠️ No active Ethernet interface found. Plug in cable to start monitoring.");
        }
        else
        {
            expectedIPAddress = GetIPv4Address(ethInterface);
            interfaceName = ethInterface.Name;
            Console.WriteLine($"Monitoring started on: {interfaceName}");
            Console.WriteLine($"Learned IP: {expectedIPAddress}\n");
        }

        OperationalStatus previousStatus = ethInterface?.OperationalStatus ?? OperationalStatus.Unknown;

        while (true)
        {
            var currentInterface = NetworkInterface
                .GetAllNetworkInterfaces()
                .FirstOrDefault(ni => ni.Name == interfaceName || interfaceName == null &&
                    ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    ni.OperationalStatus != OperationalStatus.Unknown);

            if (currentInterface != null)
            {
                var currentStatus = currentInterface.OperationalStatus;

                if (currentStatus != previousStatus)
                {
                    if (currentStatus == OperationalStatus.Down)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[{DateTime.Now}] {currentInterface.Name}: Cable disconnected!");
                    }
                    else if (currentStatus == OperationalStatus.Up)
                    {
                        var currentIP = GetIPv4Address(currentInterface);

                        if (expectedIPAddress == null)
                        {
                            // First time connect — green
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"[{DateTime.Now}] {currentInterface.Name}: Cable connected. Learned IP: {currentIP}");
                            expectedIPAddress = currentIP;
                            interfaceName = currentInterface.Name;
                        }
                        else if (previousStatus == OperationalStatus.Down)
                        {
                            // Reconnect event — orange
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine($"[{DateTime.Now}] {currentInterface.Name}: Cable reconnected. Current IP: {currentIP}");

                            if (currentIP != expectedIPAddress)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"[{DateTime.Now}] {currentInterface.Name}: WARNING: IP has changed! Expected: {expectedIPAddress}, Now: {currentIP}");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"[{DateTime.Now}] {currentInterface.Name}: IP match confirmed.");
                            }
                        }
                    }

                    Console.ResetColor();
                    previousStatus = currentStatus;
                }
            }

            Thread.Sleep(2000);
        }
    }

    static string GetIPv4Address(NetworkInterface ni)
    {
        return ni.GetIPProperties().UnicastAddresses
                 .FirstOrDefault(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork)?
                 .Address.ToString() ?? "N/A";
    }
}
