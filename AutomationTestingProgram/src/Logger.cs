// <copyright file="Logger.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram
{
    using System;
    using System.IO;
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
        private static Microsoft.Extensions.Logging.ILogger iLogger;

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
        /// Get Log4Net Logger.
        /// </summary>
        /// <returns>An ILogger.</returns>
        public static ILogger GetLog4NetLogger()
        {
            if (iLogger != null)
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
