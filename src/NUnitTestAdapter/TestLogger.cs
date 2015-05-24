﻿// ****************************************************************
// Copyright (c) 2013 NUnit Software. All rights reserved.
// ****************************************************************

using System;

using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace NUnit.VisualStudio.TestAdapter
{
    /// <summary>
    /// TestLogger wraps an IMessageLogger and adds various
    /// utility methods for sending messages. Since the
    /// IMessageLogger is only provided when the discovery
    /// and execution objects are called, we use two-phase
    /// construction. Until Initialize is called, the logger
    /// simply swallows all messages without sending them
    /// anywhere.
    /// </summary>
    public class TestLogger : IMessageLogger
    {
        private IMessageLogger messageLogger;

        int Verbosity { get; set; }

        public TestLogger()
        {
            Verbosity = 0;
        }

        public TestLogger(int verbosity)
        {
            Verbosity = verbosity;
        }

        public void Initialize(IMessageLogger messageLoggerParam)
        {
            messageLogger = messageLoggerParam;
        }

        public void AssemblyNotSupportedWarning(string sourceAssembly)
        {
            SendWarningMessage("Assembly not supported: " + sourceAssembly);
        }

        public void DependentAssemblyNotFoundWarning(string dependentAssembly, string sourceAssembly)
        {
            SendWarningMessage("Dependent Assembly " + dependentAssembly + " of " + sourceAssembly + " not found. Can be ignored if not a NUnit project.");
        }

        public void UnsupportedFrameworkWarning(string assembly)
        {
            SendWarningMessage("Attempt to load assembly with unsupported test framework in  "+assembly);
        }

        public void LoadingAssemblyFailedWarning(string dependentAssembly, string sourceAssembly)
        {
            SendWarningMessage("Assembly " + dependentAssembly + " loaded through " + sourceAssembly + " failed. Assembly is ignored. Correct deployment of dependencies if this is an error.");
        }

        public void NUnitLoadError(string sourceAssembly)
        {
            SendErrorMessage("NUnit failed to load " + sourceAssembly);
        }

        public void SendErrorMessage(string message)
        {
            SendMessage(TestMessageLevel.Error, message);
        }

        public void SendErrorMessage(string message, Exception ex)
        {
            
            switch (Verbosity)
            {
                case 0:
                    var type = ex.GetType();
                    SendErrorMessage(string.Format("Exception {0}, {1}",type, message));
                    break;
                default:
                    SendMessage(TestMessageLevel.Error, message);
                    SendErrorMessage(ex.ToString());
                    break;
            }
        }

        public void SendWarningMessage(string message)
        {
            SendMessage(TestMessageLevel.Warning, message);
        }

        public void SendWarningMessage(string message,Exception ex)
        {
            switch (Verbosity)
            {
                case 0:
                    var type = ex.GetType();
                    SendMessage(TestMessageLevel.Warning,string.Format("Exception {0}, {1}", type, message));
                    break;
                default:
                    SendMessage(TestMessageLevel.Warning, message);
                    SendMessage(TestMessageLevel.Warning,ex.ToString());
                    break;
            }
            SendMessage(TestMessageLevel.Warning, message);
        }

        public void SendInformationalMessage(string message)
        {
            SendMessage(TestMessageLevel.Informational, message);
        }

        public void SendDebugMessage(string message)
        {
#if DEBUG
            SendMessage(TestMessageLevel.Informational, message);
#endif
        }

        public void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
            if (messageLogger != null)
                messageLogger.SendMessage(testMessageLevel, message);
        }
    }
}
