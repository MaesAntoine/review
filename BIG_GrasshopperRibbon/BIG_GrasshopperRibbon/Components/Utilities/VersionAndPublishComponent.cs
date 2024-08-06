using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Rhino;
using Grasshopper.Kernel.Parameters;
using Grasshopper;
using BIG_UTILITY.LOGGER;
using BIG_SQLLogger;
using GH_IO.Serialization;

namespace BIG_GrasshopperRibbon
{
    public class VersionAndPublishComponent : BIG_Component
    {
        public override Guid ComponentGuid => new Guid("f2a31458-57f1-42be-95ec-49ae75bad0cf");

        // GLOBAL VARIABLES
        string FOLDER_NAME = "Releases";
        string USER_PATH = Environment.GetEnvironmentVariable("USERPROFILE");
        string BIG_LOCAL_SHARE = @"Bjarke Ingels Group\COMPUTE - Script Library"; // @ for verbatim string
        string DocumentHeader = "";
        string VersionDescription = "";
       

        // List to hold version history
        List<string> VersionHistory = new List<string>();


        // Make component hidden

        public override GH_Exposure Exposure => GH_Exposure.hidden;

        protected override string Author => "Andreas Bak";

        protected override string CoAuthor => "Antoine Maes";

        // Constructor
        public VersionAndPublishComponent()
          : base("Version and Publish", "V&P",
              "Component that allows you to create release versions of your script.",
              "BIG", "Utilities")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            // Input parameters
            Param_String documentHeaderParam = new Param_String();
            documentHeaderParam.Name = "Document Header";
            documentHeaderParam.NickName = "DH";
            documentHeaderParam.Access = GH_ParamAccess.item;
            documentHeaderParam.Optional = false;
            pManager.AddParameter(documentHeaderParam);

            Param_Boolean createMajorVersionParam = new Param_Boolean();
            createMajorVersionParam.Name = "Create Major Version";
            createMajorVersionParam.NickName = "CMAJ";
            createMajorVersionParam.Access = GH_ParamAccess.item;
            createMajorVersionParam.Optional = true;
            pManager.AddParameter(createMajorVersionParam);

            Param_Boolean createMinorVersionParam = new Param_Boolean();
            createMinorVersionParam.Name = "Create Minor Version";
            createMinorVersionParam.NickName = "CMIN";
            createMinorVersionParam.Access = GH_ParamAccess.item;
            createMinorVersionParam.Optional = true;
            pManager.AddParameter(createMinorVersionParam);

            Param_String versionDescriptionParam = new Param_String();
            versionDescriptionParam.Name = "Version Description";
            versionDescriptionParam.NickName = "D";
            versionDescriptionParam.Access = GH_ParamAccess.item;
            versionDescriptionParam.Optional = true;
            pManager.AddParameter(versionDescriptionParam);

            Param_Boolean publishGrasshopperParam = new Param_Boolean();
            publishGrasshopperParam.Name = "Publish Grasshopper";
            publishGrasshopperParam.NickName = "PG";
            publishGrasshopperParam.Access = GH_ParamAccess.item;
            publishGrasshopperParam.Optional = true;
            pManager.AddParameter(publishGrasshopperParam);

            Param_Boolean publishRhinoParam = new Param_Boolean();
            publishRhinoParam.Name = "Publish Rhino";
            publishRhinoParam.NickName = "PR";
            publishRhinoParam.Access = GH_ParamAccess.item;
            publishRhinoParam.Optional = true;
            pManager.AddParameter(publishRhinoParam);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            // Output parameter
            Param_GenericObject versionHistoryParam = new Param_GenericObject();
            versionHistoryParam.Name = "Version History";
            versionHistoryParam.NickName = "VH";
            versionHistoryParam.Access = GH_ParamAccess.list;
            pManager.AddParameter(versionHistoryParam);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {



  
            bool createMajorVersion = false;
            bool createMinorVersion = false;
            bool publishGrasshopper = false;
            bool publishRhino = false;

            // Retrieve document header
            DA.GetData(0, ref DocumentHeader);

            // Retrieve create major version
            DA.GetData(1, ref createMajorVersion);

            // Retrieve create minor version
            DA.GetData(2, ref createMinorVersion);

            // Retrieve version description
            DA.GetData(3, ref VersionDescription);

            // Retrieve publish Grasshopper
            DA.GetData(4, ref publishGrasshopper);

            // Retrieve publish Rhino
            DA.GetData(5, ref publishRhino);






            // Check if local sharepoint exists and display white bubble if not
            PreCheckLocalSharePoint();


            // Get versions
            var versions = GetVersions();
            if (versions.Count == 0)
            {
                // Create initial version
                AddVersion("Document Creation", true);
                var current_document = OnPingDocument();
                var current_io = new GH_DocumentIO(current_document);
                current_io.SaveAs(); // Optional save
            }

            // if file hasn't been saved, report warning
            if (OnPingDocument().FilePath == null)
            {
                MessageLog.AddWarning("Grasshoper document has not yet been saved and therfore it can not be released.");
                return;
            }

            if (string.IsNullOrEmpty(VersionDescription) || VersionDescription == "Double click to edit panel content…")
            {
                MessageLog.AddWarning("The version description can not be empty.");
            }
           

            // Add new versions
            if (createMinorVersion)
            {
                ReleaseVersion(VersionDescription, false);
            }

            if (createMajorVersion)
            {
                ReleaseVersion(VersionDescription, true);
            }

            // Publish Grasshopper file to SharePoint
            if (publishGrasshopper)
            {
                var message = PublishGrasshopper();
                message += SetToggleToFalse();
                Rhino.UI.Dialogs.ShowMessage(message, "Publish Grasshopper", Rhino.UI.ShowMessageButton.OK, Rhino.UI.ShowMessageIcon.Exclamation);
            }

            // Publish Rhino file to SharePoint
            if (publishRhino)
            {
                var message = PublishRhino();
                message += SetToggleToFalse();
                Rhino.UI.Dialogs.ShowMessage(message, "Publish Rhino", Rhino.UI.ShowMessageButton.OK, Rhino.UI.ShowMessageIcon.Exclamation);
            }

            // Display properties
            DisplayDocumentProperties(50, 50);

            

            // Output
            var VersionHistory = new List<string>();
            foreach (var version in versions)
            {
                if (version != null)
                {
                    var versionName = GetVersionName(version);
                    var versionAuthor = GetVersionAuthor(version);
                    var versionDate = GetVersionDate(version);
                    var versionDescription = GetVersionDescription(version);
                    var versionText = $"{versionName}\n{versionAuthor}\n{versionDate}\n{versionDescription}";
                    VersionHistory.Add(versionText);
                }
                else
                {
                    VersionHistory.Add("No current versions in the document");
                }
            }

            DA.SetDataList("Version History", VersionHistory);
        }

        // Runs when the component is added / present on the canvas
        public override void AddedToDocument(GH_Document document)
        {
            // REgister event to run when file path is changed, ie. file has been saved.
            // You could also do this in the constructor, but I have found that this
            // works better as the constructor is called more than once, whereas
            // this method is called only once.
            document.FilePathChanged += OnFilePathChanged;
            base.AddedToDocument(document);
        }

        private void OnFilePathChanged(Object sender, GH_DocFilePathEventArgs e)
        {
            //Re-run the component
            ExpireSolution(true);

            //Refresh the canvas to remove warning bubble
            Instances.ActiveCanvas.Refresh();
        }


        private string CheckOrMakeFolder()
        {

            

            // Check/makes a "Releases" folder in the GH def folder
            if (OnPingDocument().FilePath != null)
            {
                string folder = Path.GetDirectoryName(OnPingDocument().FilePath);
                string releaseFolder = Path.Combine(folder, FOLDER_NAME);
                if (!Directory.Exists(releaseFolder))
                {
                    Directory.CreateDirectory(releaseFolder);
                }
                return releaseFolder;
            }
            return "";
        }


        private string GetAuthor()
        {
            return CentralSettings.AuthorName;
        }

        private string GetEMail()
        {
            return CentralSettings.AuthorEMail;
        }

        private void AddVersion(string text, bool isRelease)
        {
            // Check if description is empty
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            // Get latest version
            var latestVersion = GetLatestVersion();

            double versionNumber;
            if (latestVersion != null)
            {
                // Get version number
                double latestVersionNumber = GetVersionNumber(latestVersion);

                versionNumber = isRelease ? Math.Ceiling(latestVersionNumber + 0.001) : latestVersionNumber + 0.01;
            }
            else
            {
                // No versions found
                versionNumber = 0.0;
            }

            // Create new version
            var version = new GH_Revision();

            // Set version description
            version.Description = $"Version {versionNumber.ToString("N2")}|Author: {GetAuthor()}|Author E-mail: {GetEMail()}|Description: {text}";

            // Get versions
            var versions = GetVersions();

            // Add version
            versions.Add(version);

            // Update display properties
            DisplayDocumentProperties(50, 50);
        }

        private void ReleaseVersion(string text, bool isRelease)
        {
            AddVersion(text, isRelease);
            TripleQuietSave();
        }

        private void ClearAllFileVersions()
        {
            // This is not in use anymore
            // Get current versions from document
            var versions = GetVersions();

            if (versions.Count > 0)
            {
                // Show warning
                var result = Rhino.UI.Dialogs.ShowMessage("This will clear all version history in the document. Are you sure you want to continue?", "Clear Versions", Rhino.UI.ShowMessageButton.YesNo, Rhino.UI.ShowMessageIcon.Question);

                if (result == Rhino.UI.ShowMessageResult.Yes)
                {
                    // Clear history
                    versions.Clear();
                }
            }
        }

        private GH_Revision GetLatestVersion()
        {
            // Get current versions from document
            var versions = GetVersions();

            if (versions.Count > 0)
            {
                // Get latest version
                var latestVersion = versions[versions.Count - 1];
                return latestVersion;
            }

            return null;
        }

        private GH_Revision GetFirstVersion()
        {
            // Get current versions from document
            var versions = GetVersions();

            if (versions.Count > 0)
            {
                // Get latest version
                var firstVersion = versions[0];
                return firstVersion;
            }

            return null;
        }

        private double GetVersionNumber(GH_Revision version)
        {
            // Get version number from description
            if (version != null)
            {
                string versionDescription = version.Description;
                double versionNumber = double.Parse(versionDescription.Split('|')[0].Split()[1]);
                return versionNumber;
            }
            return 0.0;
        }

        private List<GH_Revision> GetVersions()
        {
            return OnPingDocument().Properties.Revisions;
        }

        private string GetVersionName(GH_Revision version)
        {
            if (version != null)
            {
                return version.Description.Split('|')[0];
            }
            return "";
        }

        private string GetVersionDate(GH_Revision version)
        {
            if (version != null)
            {
                return version.Date.ToLongDateString();
            }
            return "";
        }

        private string GetVersionAuthor(GH_Revision version)
        {
            if (version != null)
            {
                return version.Description.Split('|')[1].Split(':')[1].Trim();
            }
            return "";
        }

        private string GetVersionAuthorEMail(GH_Revision version)
        {
            if (version != null)
            {
                return version.Description.Split('|')[2].Split(':')[1].Trim();
            }
            return "";
        }

        private string GetVersionDescription(GH_Revision version)
        {
            if (version != null)
            {
                return version.Description.Split('|')[3].Split(':')[1].Trim();
            }
            return "";
        }

        private void TripleQuietSave()
        {
            /*
            This saves the current document, save a cleaned copy with version number
            and a cleaned copy without version number.
            "cleaned" means a gh file without this component.
            */
            var current_document = OnPingDocument();
            var current_io = new GH_DocumentIO(current_document);

            
            

            

            // get useful current file info and create subfolder
            var dir_name = Path.GetDirectoryName(current_document.FilePath);
            var file = Path.GetFileName(current_document.FilePath);
            var file_name = Path.GetFileNameWithoutExtension(file);
            var ext = Path.GetExtension(file);

            // get versions and save the current document
            var version = GetLatestVersion();
            current_io.Save(); // save #1

            // check/make the output paths
            var output_folder_path = CheckOrMakeFolder();
            var full_file_name = $"{file_name}_{GetVersionNumber(version).ToString("N2")}{ext}";
            var output_full_path = Path.Combine(output_folder_path, full_file_name);
            var output_versionless_full_path = Path.Combine(output_folder_path, file);

            // make a copy of the current document and prepare new path
            var copy_document = GH_Document.DuplicateDocument(current_document);
            var copy_io = new GH_DocumentIO(copy_document);

            // get the group and remove the elements within that group
            var group = GetGroup(copy_document);
            var objects_in_group = GetGroupObjects(copy_document, group.InstanceGuid);

            copy_document.RemoveObject(group, false);
            copy_document.RemoveObjects(objects_in_group, false); // rhino crashes if True

            // save the cleaned copy document to the new folder
            copy_io.SaveQuiet(output_full_path); // save #2
            copy_io.SaveQuiet(output_versionless_full_path); // save #3
        }

        private GH_Scribble GetScribbleByNickName(string nickName)
        {
            var objects = OnPingDocument().Objects;
            foreach (var obj in objects)
            {
                if (obj is GH_Scribble scribble && scribble.NickName == nickName)
                {
                    return scribble;
                }
            }
            return null;
        }

        private GH_Markup GetMarkUpByNickName(string nickName)
        {
            var objects = OnPingDocument().Objects;
            foreach (var obj in objects)
            {
                if (obj is GH_Markup markup && markup.NickName == nickName)
                {
                    return markup;
                }
            }
            return null;
        }

        private void DisplayDocumentProperties(int x, int y)
        {
            // Set initial pivot position drawing
            var pivot = new PointF(x, y);

            // List to hold document info
            var documentInfo = new List<(string, string)>();

            // Get version information
            var firstVersion = GetFirstVersion();
            var version = GetLatestVersion();

            // Get the name of the Gh file
            var ghName = OnPingDocument().DisplayName;
            if (ghName.EndsWith("*"))
            {
                ghName = ghName.Substring(0, ghName.Length - 1);
            }

            // Get document information
            var document = OnPingDocument();
            documentInfo.Add(("Logo", DocumentHeader));
            documentInfo.Add(("Name", ghName));
            documentInfo.Add(("Creation Date", document.Properties.Date.ToLongDateString()));
            documentInfo.Add(("Author", GetVersionAuthor(firstVersion)));
            documentInfo.Add(("Author E-mail", GetVersionAuthorEMail(firstVersion)));
            documentInfo.Add(("Version", GetVersionNumber(version).ToString("N2")));
            documentInfo.Add(("Version Author", GetVersionAuthor(version)));
            documentInfo.Add(("Version Author E-mail", GetVersionAuthorEMail(version)));
            documentInfo.Add(("Version Date", GetVersionDate(version)));

            // Find existing or create new scribbles
            foreach (var record in documentInfo)
            {
                var key = record.Item1;
                var value = record.Item2;

                var scribble = GetScribbleByNickName(key);

                if (scribble == null)
                {
                    // Create new scribble
                    scribble = new GH_Scribble();

                    // Set nickname of scribble to retrieve it at a later stage
                    scribble.NickName = key;

                    // Add to document
                    document.AddObject(scribble, false);
                }

                // Write text to scribble
                if (key == "Logo")
                {
                    scribble.Text = value;
                }
                else
                {
                    scribble.Text = $"{key}: {value}";
                }

                // Set scribble font
                var font = key == "Logo" ? new System.Drawing.Font("BIG Pixel", 100) :
                    (key == "Name" ? new System.Drawing.Font("Klavika Regular", 32) :
                    new System.Drawing.Font("Klavika Light", 24));
                scribble.Font = font;

                // Set pivot point
                scribble.Attributes.Pivot = pivot;

                // Update position of pivot for next scribble
                pivot.Y += key == "Logo" ? 100 :
                    (key == "Name" ? 65 : 50);

                // Force redraw of scrible
                scribble.Attributes.ExpireLayout();
            }
        }

        private GH_Group GetGroup(GH_Document document)
        {
            // Get the the group this component lives in
            var component = this;
            foreach (var obj in document.Objects)
            {
                if (obj is GH_Group group && group.ObjectIDs.Contains(component.InstanceGuid))
                {
                    return group;
                }
            }
            return null;
        }

        private List<IGH_DocumentObject> GetGroupObjects(GH_Document gh_doc, Guid group_guid)
        {
            // Returns the components of a group
            var group_objects = new List<IGH_DocumentObject>();
            foreach (var obj in gh_doc.Objects)
            {
                if (obj is GH_Group group && group.InstanceGuid == group_guid)
                {
                    foreach (var id in group.ObjectIDs)
                    {
                        var member_obj = gh_doc.FindObject(id, true);
                        if (member_obj != null)
                        {
                            group_objects.Add(member_obj);
                        }
                    }
                    break;
                }
            }
            return group_objects;
        }

        private string ToSharePoint(string copy_from_path)
        {
            string copy_to_dir = Path.Combine(USER_PATH, BIG_LOCAL_SHARE);
            string fileName = Path.GetFileName(copy_from_path);
            var copy_to_path = Path.Combine(copy_to_dir, fileName);

            // Check if synced sharepoint folder exists on the user's machine
            if (!Directory.Exists(copy_to_dir))
            {
                var e = "It seems like the script library from the SharePoint isn't synchronized to your computer.";
                Rhino.RhinoApp.WriteLine(e);
                return "";
            }

            // Try to push the file to sharepoint (and override if needed)
            try
            {
                File.Copy(copy_from_path, copy_to_path, true);

                if (Path.GetExtension(copy_from_path) == ".gh")
                {
                    var archive = GetArchive(copy_to_path);
                    var documentProperties = GetDocumentStatistics(archive);
                    Logger.LogDocumentPublished((string)documentProperties["File Name"], (Guid)documentProperties["Document ID"], (DateTime)documentProperties["Creation Date"]);
                }


                return "Your file has been successfully pushed !";
            }
            catch (Exception e)
            {
                RhinoApp.WriteLine(e.Message+ "\nMake sure to be in the 'Compute Script Admins' group on SharePoint.\nAsk IT about it and restart your computer once the changes are made.");
                return "";
            }
        }

        private GH_Archive GetArchive(string filePath)
        {
            var archive = new GH_Archive();
            archive.ReadFromFile(filePath);
            return archive;
        }

        public static Dictionary<string, object> GetDocumentStatistics(GH_Archive archive)
        {
            var documentStatistics = new Dictionary<string, object>();
            var root = archive.GetRootNode;
            var definition = root.FindChunk("Definition");
            var header = definition.FindChunk("DocumentHeader");
            var properties = definition.FindChunk("DefinitionProperties");
            documentStatistics["Document ID"] = header.GetGuid("DocumentID");
            documentStatistics["Creation Date"] = properties.GetDate("Date");
            documentStatistics["File Name"] = Path.GetFileName(archive.Path);
            return documentStatistics;
        }

        private string PublishGrasshopper()
        {
            // Check if the file has been saved once
            if (OnPingDocument().FilePath == null)
            {
                return "Hold your horses !\nThe Grasshopper file needs to be saved, and released before publishing.";
            }

            // Check if the file has been released once
            var ghDir = Path.GetDirectoryName(OnPingDocument().FilePath);
            var fileName = Path.GetFileName(OnPingDocument().FilePath);
            var absolute_released_path = Path.Combine(ghDir, FOLDER_NAME, fileName);
            if (!File.Exists(absolute_released_path))
            {
                return "It seems your Grasshopper file hasn't been released yet.\nPublishing becomes available after a version is released.";
            }

            // Pop up
            var result = Rhino.UI.Dialogs.ShowMessage("This will publish and overwrite the latest version of the Grasshopper file on Notion.\nDo you wish to continue?", "Publish Grasshopper", Rhino.UI.ShowMessageButton.YesNo, Rhino.UI.ShowMessageIcon.Question);

            // If user confirms, push to sharepoint
            if (result == Rhino.UI.ShowMessageResult.Yes)
            {
                return ToSharePoint(absolute_released_path);
            }
            return "Publication cancelled !";
        }

        private string PublishRhino()
        {
            // Check if file saved
            var rhinoDocPath = Rhino.RhinoDoc.ActiveDoc.Path;
            if (rhinoDocPath == null)
            {
                return "It seems your Rhino file hasn't been saved anywhere yet.\nPublishing a rhino file is only possible once the file exists.";
            }

            // Pop up
            var result = Rhino.UI.Dialogs.ShowMessage("This will publish and overwrite the latest version of the Rhino file on Notion.\nDo you wish to continue?", "Publish Rhino", Rhino.UI.ShowMessageButton.YesNo, Rhino.UI.ShowMessageIcon.Question);

            // If user confirms, push to sharepoint
            if (result == Rhino.UI.ShowMessageResult.Yes)
            {
                return ToSharePoint(rhinoDocPath);
            }
            return "Publication cancelled !";
        }

        private string SetToggleToFalse()
        {
            // Goes over the inputs and resets ALL toggles to False. Then refreshes.
            foreach (var input in Params.Input)
            {
                foreach (var source in input.Sources)
                {
                    if (source is GH_BooleanToggle toggle && toggle.Value)
                    {
                        toggle.Value = false;
                    }
                }
            }

            // This will refresh the component, do not remove any of the four lines
            var gh_doc = OnPingDocument();
            gh_doc.NewSolution(false);
            gh_doc.ExpireSolution();
            gh_doc.NewSolution(false);

            return "\n\n(The toggle has been reset to False, so you don't have to)";
        }

        private void PreCheckLocalSharePoint()
        {
            if (!Directory.Exists(Path.Combine(USER_PATH, BIG_LOCAL_SHARE)))
            {
                var m = "It seems you have not synchronized the Script Library from the SharePoint to your computer.\nThis will be needed to PUBLISH scripts.";
                MessageLog.AddRemark(m);
            }
        }
    }
}
