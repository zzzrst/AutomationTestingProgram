// <copyright file="Logger.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using log4net;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Logger class to be used. \n
    /// Reference 1: https://stackify.com/log4net-guide-dotnet-logging/ \n
    /// Reference 2: https://www.dotnetdepth.in/2018/06/how-to-use-log4net-in-net-core-20.html.
    /// </summary>
    public static class Logger
    {
        private static readonly string LOGCONFIGFILE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/log4net.config";

        private static readonly ILog Log = GetLogger(typeof(Logger));
        private static ILogger iLogger;

        /// <summary>
        /// Allows the user to log a debug message.
        /// </summary>
        /// <param name="message">The debug message to be logged.</param>
        public static void Debug(object message)
        {
            Log.Debug(message);
        }

        /// <summary>
        /// Allows the user to log an error message.
        /// </summary>
        /// <param name="message">The error message to be logged.</param>
        public static void Error(object message)
        {
            Log.Error(message);
        }

        /// <summary>
        /// Allows the user to log a fatal message.
        /// </summary>
        /// <param name="message">The fatal message to be logged.</param>
        public static void Fatal(object message)
        {
            Log.Fatal(message);
        }

        /// <summary>
        /// Allows the user to log an info message.
        /// </summary>
        /// <param name="message">The info message to be logged.</param>
        public static void Info(object message)
        {
            Log.Info(message);
        }

        /// <summary>
        /// Allows the user to log a warning message.
        /// </summary>
        /// <param name="message">The warning message to be logged.</param>
        public static void Warn(object message)
        {
            Log.Warn(message);
        }

        /// <summary>
        /// Logs a line with 4 tabs and Stdout:.
        /// </summary>
        public static void LogStdout()
        {
            Log.Info($"{Tab(4)}Stdout:");
        }

        /// <summary>
        /// Logs the provided 'log' with 5 tabs in front.
        /// </summary>
        /// <param name="log">The log<see cref="string"/>.</param>
        public static void LogWithFiveTabs(string log)
        {
            Log.Info($"{Tab(5)}{log}");
        }

        /// <summary>
        /// Get Log4Net Logger.
        /// </summary>
        /// <returns>An ILogger.</returns>
        public static ILogger GetLog4NetLogger()
        {
            if (iLogger == null)
            {
                ILoggerFactory loggerFactory = new LoggerFactory();
                loggerFactory.AddLog4Net();
                iLogger = loggerFactory.CreateLogger(string.Empty);
            }

            return iLogger;
        }

        /// <summary>
        /// Returns the type for the logger.
        /// </summary>
        /// <param name="type">Class this logger is under.</param>
        /// <returns>ILog instance to interact with.</returns>
        private static ILog GetLogger(Type type)
        {
            ILog logger = LogManager.GetLogger(type);
            SetLog4NetConfiguration();
            return logger;
        }

        /// <summary>
        /// Returns a sequence of whitespaces of fixed length to represent tabs in the print log.
        /// </summary>
        /// <param name="indents">Number of tabs.</param>
        /// <returns>Sequence of tabs represented as whitespaces.</returns>
        public static string Tab(int indents = 1)
        {
            return string.Concat(Enumerable.Repeat("    ", indents));
        }

        /// <summary>
        /// Sets up the Log4Net Configuration.
        /// </summary>
        private static void SetLog4NetConfiguration()
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead(LOGCONFIGFILE));

            var repo = LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
        }
    }
}
