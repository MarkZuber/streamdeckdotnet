var target = Argument("target", "Default");

Task("BuildStreamDeckCon")
    .Does(() => {
        DotNetCorePublish(
            "./samples/StreamDeckCon/StreamDeckCon.csproj",
            new DotNetCorePublishSettings
            {
                Configuration = "Release",
                Runtime = "linux-arm"
            });
        StartProcess("wsl.exe", new ProcessSettings {
            Arguments = "rsync -rvzh ./samples/StreamDeckCon/bin/Release/netcoreapp3.0/linux-arm/* pi@192.168.2.203:/home/pi/streamdeck"
        });
    });

Task("Default")
    .Does(() =>
    {
        Information("Hello World!");
    });

RunTarget(target);
