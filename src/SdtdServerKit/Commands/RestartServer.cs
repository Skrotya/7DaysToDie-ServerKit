﻿using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SdtdServerKit.Commands
{
    /// <summary>
    /// Represents a command to restart the server.
    /// </summary>
    public class RestartServer : ConsoleCmdBase
    {
        /// <inheritdoc />
        public override string getDescription()
        {
            return "Restart server, optional parameter -f/force.";
        }

        /// <inheritdoc />
        public override string getHelp()
        {
            return "Usage:\n" +
                "  1. ty-rs" +
                "  2. ty-rs -f" +
                "1. Restart server by shutdown" +
                "2. Force restart server";
        }

        /// <inheritdoc />
        public override string[] getCommands()
        {
            return new[] { "ty-rs", "ty-RestartServer" };
        }

        /// <inheritdoc />
        public override void Execute(List<string> args, CommandSenderInfo senderInfo)
        {
            Log("Server is restarting..., please wait");

            if (args.Count > 0)
            {
                if (string.Equals(args[0], "-f", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(args[0], "-force", StringComparison.OrdinalIgnoreCase))
                {
                    PrepareRestart(true);
                    return;
                }
            }

            PrepareRestart(false);
        }

        private static volatile bool _isRestarting;

        /// <summary>
        /// Prepares the server for restart.
        /// </summary>
        /// <param name="force">Indicates whether to force restart the server.</param>
        public static void PrepareRestart(bool force = false)
        {
            SdtdConsole.Instance.ExecuteSync("sa", ModApi.CmdExecuteDelegate);

            _isRestarting = true;

            if (force)
            {
                Restart();
            }
            else
            {
                SdtdConsole.Instance.ExecuteSync("shutdown", ModApi.CmdExecuteDelegate);
            }
        }

        private static void Restart()
        {
            string? scriptName = null;
            string? serverPath = null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                scriptName = "restart-windows.bat";
                serverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startdedicated.bat");
                string path = Path.Combine(ModApi.ModInstance.Path, scriptName);
                Process.Start(path, string.Format("{0} \"{1}\"", Process.GetCurrentProcess().Id, serverPath));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                scriptName = "restart-linux.sh";
                serverPath = AppDomain.CurrentDomain.BaseDirectory;
                Process.Start("chmod", " +x " + Path.Combine(ModApi.ModInstance.Path, scriptName)).WaitForExit();
                string path = Path.Combine(ModApi.ModInstance.Path, scriptName);
                Process.Start(path, string.Format("{0} {1}", Process.GetCurrentProcess().Id, ModApi.AppSettings.ServerSettingsFileName));
            }
        }

        /// <summary>
        /// Handles the game shutdown event.
        /// </summary>
        private static void OnGameShutdown()
        {
            if (_isRestarting)
            {
                Restart();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestartServer"/> class.
        /// </summary>
        public RestartServer()
        {
            ModEventHub.GameShutdown -= OnGameShutdown;
            ModEventHub.GameShutdown += OnGameShutdown;
        }
    }
}