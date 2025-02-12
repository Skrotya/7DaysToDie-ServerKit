﻿namespace SdtdServerKit.Commands
{
    /// <summary>
    /// Sends a message to all connected clients.
    /// </summary>
    public class GlobalMessage : ConsoleCmdBase
    {
        /// <summary>
        /// Gets the description of the command.
        /// </summary>
        /// <returns>The description of the command.</returns>
        public override string getDescription()
        {
            return "Sends a message to all connected clients.";
        }

        /// <summary>
        /// Gets the help text for the command.
        /// </summary>
        /// <returns>The help text for the command.</returns>
        public override string getHelp()
        {
            return "Usage:\n" +
               "  1. ty-gm <Message>\n" +
               "1. Sends a message to all connected clients by sender name.";
        }

        /// <summary>
        /// Gets the list of commands associated with the command.
        /// </summary>
        /// <returns>The list of commands associated with the command.</returns>
        public override string[] getCommands()
        {
            return new string[]
            {
                    "ty-GlobalMessage",
                    "ty-gm",
                    "ty-say"
            };
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="args">The list of arguments passed to the command.</param>
        /// <param name="_senderInfo">The information of the command sender.</param>
        public override void Execute(List<string> args, CommandSenderInfo _senderInfo)
        {
            if (args.Count < 1)
            {
                Log("Wrong number of arguments, expected 1, found " + args.Count + ".");
                return;
            }

            string message = args[0];
            GameManager.Instance.ChatMessageServer(ModApi.CmdExecuteDelegate, EChatType.Global, -1, message, null, EMessageSender.Server);
        }
    }
}