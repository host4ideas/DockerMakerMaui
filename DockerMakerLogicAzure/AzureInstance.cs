using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;

namespace DockerMakerLogicAzure
{
    public class AzureInstance
    {
        private static AzureInstance? _instance;
        public ArmClient? _client;

        private AzureInstance()
        {
        }

        public static AzureInstance Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AzureInstance();
                }
                return _instance;
            }
        }

        #region METHODS 
        public ArmClient Initialize()
        {
            this._client = new ArmClient(new DefaultAzureCredential());
            return this._client;
        }
        #endregion
    }
}
