using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace InfinityBooksFunctionApp.Helper
{
    public class LogHelper
    {
        public static void LogException(Exception ex)
        {
            string fromMethodName = new StackTrace().GetFrame(1).GetMethod().Name;
            try
            {
                Logger logger = LogManager.GetLogger("databaseLogger");
                LogManager.GetCurrentClassLogger();
                GlobalDiagnosticsContext.Set("methodName", fromMethodName);
                logger.Factory.ThrowExceptions = true;
                logger.Error(ex);
            }
            catch (Exception dbEx)
            {
                Logger logger = LogManager.GetLogger("fileLogger");
                LogManager.GetCurrentClassLogger();
                GlobalDiagnosticsContext.Set("methodName", fromMethodName);
                logger.Error(dbEx);
            }
        }

        public static void TraceActivity(string ex)
        {
            try
            {
                Logger logger = LogManager.GetLogger("databaseLogger");
                logger.Factory.ThrowExceptions = true;
                logger.Warn(ex);
            }
            catch (Exception dbEx)
            {
                Logger logger = LogManager.GetLogger("fileLogger");
                logger.Error(dbEx);
            }
        }
    }
}