﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace trident
{
    public class Upload
    {
        private List<string> finalList;
        private int totalUploadCount;
        private Setting setting;
        private static ILog log = LogManager.GetLogger(typeof(Upload));
        private Inventory inventory;

        public Upload(List<string> finalList, Setting setting)
        {
            this.finalList = finalList;
            this.setting = setting;
        }

        public void start()
        {
            //int batchCount=0;
            string lastfile = string.Empty;
            //List<string> inventoryList = new List<string>();
            // iterate loop, call uploadCore to upload one file at a time, commit inventory/log progress at interval of 10 items
            UploadCore uploadCore = new UploadCore(setting);
            inventory = new Inventory(setting);
            foreach (var filePath in finalList)
            {
                // check for the pause signal.
                if (PauseHandler.Pause)
                {
                    break;
                }
                // start a new thread to be able to listen for pause singal from console.
                PauseHandler.SpinUpPause();
                // upload file using UploadCore.
                // if successful upload, do next four lines. 
                if (uploadCore.upload(filePath))
                {
                    //inventoryList.Add(filePath);
                    totalUploadCount++;
                    lastfile = filePath;
                    inventory.commit(filePath);
                }                
            }
            log.Info(string.Format("Upload finish >>>>> SOURCE FOLDER: {0}, COUNT: {1}. LAST FILE: {2}.<<<<<<<<<<<<", setting.sourceFolderPath, totalUploadCount, lastfile));
        }
    }
}
