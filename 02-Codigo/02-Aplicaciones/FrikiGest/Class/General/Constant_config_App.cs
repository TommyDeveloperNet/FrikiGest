using System.Configuration;

namespace ContentGest.Class.General
{
    class Constant_config_App
    {
        const string _NameProject = "ContentGest.Properties";
        public static string NameApp { get => ConfigurationManager.AppSettings[Constant_appSettings_Keys.Name_Key]; }
        public static string InfoApp { get => string.Format("{0} - Vers. {1}", ConfigurationManager.AppSettings[Constant_appSettings_Keys.Name_Key], ConfigurationManager.AppSettings[Constant_appSettings_Keys.Version_Key]); }
        public static string CultureES { get => "es-ES"; }
        public static string NameProject { get => _NameProject; }
        public static string BBDDAPPCollector { get => ConfigurationManager.AppSettings[Constant_appSettings_Keys.BBDD_key]; }
        public static string ConnectionMode { get => ConfigurationManager.AppSettings[Constant_appSettings_Keys.ConnectionMode_key]; }
    }
}
