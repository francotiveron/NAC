using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Field.Citect {
    public class CitectServersConfigurationSection : ConfigurationSection {

        private static CitectServersConfigurationSection config = ConfigurationManager.GetSection("CitectServers") as CitectServersConfigurationSection;

        public static CitectServersConfigurationSection Config { get { return config; } }

        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        private CitectServersConfigElements Settings {
            get { return (CitectServersConfigElements)this[""]; }
            set { this[""] = value; }
        }

        public IEnumerable<CitectServersConfigElement> SettingsList {
            get { return Settings.Cast<CitectServersConfigElement>(); }
        }
    }

    public sealed class CitectServersConfigElements : ConfigurationElementCollection {
        protected override ConfigurationElement CreateNewElement() {
            return new CitectServersConfigElement();
        }
        protected override object GetElementKey(ConfigurationElement element) {
            return ((CitectServersConfigElement)element).Machine;
        }
    }

    public sealed class CitectServersConfigElement : ConfigurationElement {
        [ConfigurationProperty("Machine", IsRequired = true)]
        public string Machine {
            get { return (string)base["Machine"]; }
            set { base["Machine"] = value; }
        }

        [ConfigurationProperty("User", IsKey = true, IsRequired = true)]
        public string Username {
            get { return (string)base["User"]; }
            set { base["User"] = value; }
        }

        [ConfigurationProperty("Password", IsRequired = true)]
        public string Password {
            get { return (string)base["Password"]; }
            set { base["Password"] = value; }
        }
    }
}
