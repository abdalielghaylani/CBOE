using System;
using System.Xml;

namespace CambridgeSoft.COE.Registration.WebServices.Contracts
{
    interface ICOERegistrationWebServices
    {
        XmlNode GetXmlPicklist(string criteria);
        XmlNode GetXmlStructure(int structureId);
        XmlNode GetXmlSupportedMimeTypes();
        XmlNode GetXmlUniqueRegistrationTest(string registryXml, string duplicateCheck);

        XmlNode CreateTemporaryRegistration(string registryXml);
        XmlNode ReadTemporaryRegistration(int id, string structureMimeType);
        XmlNode UpdateTemporaryRegistration(string registryXml);
        XmlNode DeleteTemporaryRegistration(int regId);

        XmlNode CreateRegistration(string registryXml, string resolutionAction);
        XmlNode ReadRegistration(string regNum, string structureMimeType);
        XmlNode UpdateRegistration(string registryXml);

    }
}
