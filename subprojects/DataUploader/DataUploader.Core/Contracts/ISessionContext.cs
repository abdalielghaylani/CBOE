using System;
using System.Collections.Generic;
using System.Text;

//TODO: Develop the objects described below. The "{object name}" for each is in curly braces.

namespace CambridgeSoft.COE.DataLoader.Core.Contracts
{
    /// <summary>
    /// The Session Context describes the upload processing at any given point in time.
    /// It will be responsible for maintaining the following information.
    /// <list type="bullet">
    /// <item>
    /// An object {SourceFileInfo} that contains all the relevant information to fetch a
    /// new file parser from the file-parser factory. In fact, this object will itself be the
    /// parameter passed to the factory.
    /// </item>
    /// <item>
    /// An object {IndexRange} that maintains the master data-range being operated on
    /// for the current session. This object will contain methods to re-create the master
    /// data-range based on the input of an 'exclusion list' (a list of record indices of
    /// either'invalid objects' or 'duplicated objects').
    /// </item>
    /// <item>
    /// A list object {DataMappings} that contains the processing-definitions for migrating
    /// data (with or without a conversion step) from an ISourceRecord to an instance of a
    /// destination object. This {DataMapping} list is derived either from the user GUI or
    /// from an xml file on the file-system (using the EntLib Configuration block).
    /// </item>
    /// <item>
    /// An object {LogFileInfo} that contains all the information required to read an activity
    /// log file. Log files are created at the start of specific processing actions and populated
    /// DURING that processing action. Once that action has been completed, the log file is
    /// interactively reviewed by the user OR the program (internally) to determine further
    /// processing actions to take.
    /// </item>
    /// <item>
    /// An COEUser object for permissions-checks and authenticated data-access.
    /// </item>
    /// </list>
    /// </summary>
    interface ISessionContext
    {
        COE.Framework.Common.COEUser CurrentUser { get; }
    }
}
