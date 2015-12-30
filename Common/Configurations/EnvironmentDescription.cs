namespace Microsoft.Azure.Devices.Applications.PredictiveMaintenance.Common.Configurations
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.XPath;
    using Extensions;

    public class EnvironmentDescription : IDisposable
    {
        bool _isDisposed;
        readonly XmlDocument _document;
        XPathNavigator _navigator;
        readonly string _fileName;
        int _updatedValuesCount;
        const string ValueAttributeName = "value";
        const string SettingXpath = "//setting[@name='{0}']";

        public EnvironmentDescription(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            _fileName = fileName;
            _document = new XmlDocument();
            using (XmlReader reader = XmlReader.Create(fileName))
            {
                _document.Load(reader);
            }
            _navigator = _document.CreateNavigator();
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                _isDisposed = true;
                if (_updatedValuesCount > 0)
                {
                    _document.Save(_fileName);
                    Console.Out.WriteLine("Successfully updated {0} mapping(s) in {1}", _updatedValuesCount, Path.GetFileName(_fileName).Split('.')[0]);
                }
            }
        }

        public bool SettingExists(string settingName)
        {
            return !string.IsNullOrEmpty(GetSetting(settingName, false));
        }

        public string GetSetting(string settingName, bool errorOnNull = true)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new ArgumentNullException("settingName");
            }

            string result = string.Empty;
            XmlNode node = GetSettingNode(settingName.Trim());
            if (node != null)
            {
                result = node.Attributes[ValueAttributeName].Value;
            }
            else
            {
                if (errorOnNull)
                {
                    throw new ArgumentException("{0} was not found".FormatInvariant(settingName));
                }
            }
            return result;
        }

        XmlNode GetSettingNode(string settingName)
        {
            string xpath = SettingXpath.FormatInvariant(settingName);
            return _document.SelectSingleNode(xpath);
        }

        public bool SetSetting(string settingName, string settingValue)
        {
            return SetSetting(GetSettingNode(settingName), settingValue);
        }

        public bool SetSetting(IXPathNavigable node, string settingValue)
        {
            if (node != null)
            {
                ((XmlNode)node).Attributes[ValueAttributeName].Value = settingValue;
                _updatedValuesCount++;
                return true;
            }
            return false;
        }
    }
}