﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using log4net;
using System.Configuration;
using System.Reflection;
//using log4net.Config;

namespace trident
{
    class Program
    {
        static List<Setting> syncSettings;
        static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static string settingsFileName = string.Empty;
        static void Main(string[] args)
        {
            try
            {

                logInfo("Starting trident backup...");
                Console.Write("Starting trident...");
                // Check all configurations are in proper state. 
                readAllConfig();
                // load settings.json file and retrieve sync settings
                // iterate through each sync setting item and start sync.
                loadSyncSettings();

                Sync sync = new Sync(syncSettings);
                sync.start();

                logInfo("Backup complete!!!");
                Console.Write("Backup complete!!!");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error: {0}", ex.Message), ex);
            }
        }

        private static void readAllConfig()
        {
            try
            {
                // read and check all the config details are present in app.config file. 
                settingsFileName = ConfigurationManager.AppSettings["SettingsFileName"];
                if (string.IsNullOrEmpty(settingsFileName))
                {
                    throw new InvalidOperationException("Cannot find SettingsFileName property or value in appSettings section of app.config file.");
                }
                string inventoryFolderName = ConfigurationManager.AppSettings["InventoryFolderName"];
                if (string.IsNullOrEmpty(inventoryFolderName))
                {
                    throw new InvalidOperationException("Cannot find InventoryFolderName property or value in appSettings section of app.config file.");
                }
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["AWSProfileName"]))
                {
                    throw new InvalidOperationException("Cannot find AWSProfileName property or value in appSettings section of app.config file.");
                }
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["AWSRegion"]))
                {
                    throw new InvalidOperationException("Cannot find AWSRegion property or value in appSettings section of app.config file.");
                }
                string fileExtensions = ConfigurationManager.AppSettings["FileExtensionExclusions"];
                if (fileExtensions == null) // only check for null, dont check for empty because empty is a valid value.
                {
                    throw new InvalidOperationException("Cannot find FileExtensionExclusions key in appSettings section of app.config file. This app at least requires the 'key' and an empty 'value' present in the app.config. ");
                }
                // find out current executing directory through Assembly class to make sure if console app is started through other process such as task schedular.
                string currentDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                // verify values of the properties are valid. 
                if (!Directory.Exists(currentDirPath + "\\" + inventoryFolderName))
                {
                    // create inventory folder.
                    Directory.CreateDirectory(currentDirPath + "\\" + inventoryFolderName);
                }
                if (!File.Exists(currentDirPath + "\\" + settingsFileName))
                {
                    throw new InvalidOperationException(string.Format("Cannot find file:{0} in the application folder:{1}. ", settingsFileName, currentDirPath));
                }
            }
            catch (ConfigurationErrorsException ex)
            {
                log.Error(ex.Message, ex);
                throw new ConfigurationErrorsException("Error occured during readAllConfig(). ", ex);
            }
            catch (IOException ioex)
            {
                log.Error(ioex.Message, ioex);
                throw new IOException("Error occured during readAllCOnfig() IO Operation. ", ioex);
            }
            catch (UnauthorizedAccessException ex)
            {
                log.Error(ex.Message, ex);
                throw new UnauthorizedAccessException("Error occured during readAllConfig(). The current user does not have sufficient permission to create folder in current directory.", ex);
            }
        }

        private static void loadSyncSettings()
        {
            string strSettings = string.Empty;
            using (StreamReader r = new StreamReader(AppContext.BaseDirectory + "\\" + settingsFileName))
            {
                strSettings = r.ReadToEnd();
            }
            syncSettings = JsonConvert.DeserializeObject<List<Setting>>(strSettings);

            // check and validate values.
            if (syncSettings == null || syncSettings.Count == 0) {
                log.Error("settings.json files contains null or zero settings items.");
                throw new InvalidOperationException("settings.json files contains null or zero settings items.");
            }
            // check for null or empty values in any setting item. 
            foreach (var item in syncSettings)
            {
                if (string.IsNullOrEmpty(item.inventoryFileName))
                {
                    log.Error("One or more inventoryFileName value is empty in the settings.json file. ");
                    throw new InvalidOperationException("One or more inventoryFileName value is empty in the settings.json file. ");
                }
                if (string.IsNullOrEmpty(item.s3BucketName))
                {
                    log.Error("One or more s3BucketName value is empty in the settings.json file. ");
                    throw new InvalidOperationException("One or more s3BucketName value is empty in the settings.json file. ");
                }
                if (string.IsNullOrEmpty(item.sourceFolderPath))
                {
                    log.Error("One or more sourceFolderPath value is empty in the settings.json file. ");
                    throw new InvalidOperationException("One or more sourceFolderPath value is empty in the settings.json file. ");
                }                
            }
        }

        private static void logInfo(string msg)
        {
            if (log.IsInfoEnabled)
            {
                log.Info(msg);
            }
        }
    }
}
