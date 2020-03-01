using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using TrackDirect.Utilities;

namespace TrackDirect
{
    public enum VCFParameters
    {
        VCF_CreateAt,
        VCF_ModifyAt,
        VCF_ChangeType,
        VCF_ChangeDescription,
        VCF_User

    }


    /// <summary>
    /// Methods for parameters Revit
    /// </summary>
    public class ParameterUtils
    {
        public static void AddSharedParameters(Autodesk.Revit.ApplicationServices.Application app, Document doc, CategorySet myCategorySet)
        {
            bool inserted = false;
            try
            {
                //Save the previous shared param file path
                string previousSharedParam = app.SharedParametersFilename;
                string sharedParamFileName = "VCF_AddinSharedParameters.txt";

                //Extract shared param to a txt file
                string tempPath = System.IO.Path.GetTempPath();
                //string SPPath = Path.Combine(tempPath, "VCF_PARAMETRES_PARTAGES_Réduits.txt");
                string SPPath = Path.Combine(tempPath, sharedParamFileName);

                //Alway export parameter file to temp folder
                if (!File.Exists(SPPath))
                {
                    //extract the familly
                    List<string> files = new List<string>();
                    files.Add(sharedParamFileName);
                    FileUtils.ExtractEmbeddedResource(tempPath, "TrackDirect.Resources", files);
                }

                //set the shared param file
                app.SharedParametersFilename = SPPath;

                //Retrive shared parameters
                DefinitionFile myDefinitionFile = app.OpenSharedParameterFile();

                //DefinitionGroup definitionGroup = myDefinitionFile.Groups.get_Item("VCF_Collaboration");
                DefinitionGroup definitionGroup = myDefinitionFile.Groups.get_Item("VCF_Collaboration");

                foreach (Definition paramDef in definitionGroup.Definitions)
                {
                    // Get the BingdingMap of current document.
                    BindingMap bindingMap = doc.ParameterBindings;

                    //the parameter does not exist
                    if (!bindingMap.Contains(paramDef))
                    {
                        //Create an instance of InstanceBinding
                        InstanceBinding instanceBinding = app.Create.NewInstanceBinding(myCategorySet);
                       bindingMap.Insert(paramDef, instanceBinding, BuiltInParameterGroup.PG_TEXT);
                       
                    }
                    //the parameter is not added to the correct categories
                    else if (bindingMap.Contains(paramDef))
                    {
                        InstanceBinding currentBinding = bindingMap.get_Item(paramDef) as InstanceBinding;
                        currentBinding.Categories = myCategorySet;
                        bindingMap.ReInsert(paramDef, currentBinding, BuiltInParameterGroup.PG_TEXT);
                    }
                }
                //SetAllowVaryBetweenGroups for these parameters
                //string[] parameterNames = { "VCF_CreateAt", "VCF_ModifyAt", "VCF_ChangeDescription", "VCF_ChangeType" };
                string[] parameterNames = { VCFParameters.VCF_CreateAt.ToString(), VCFParameters.VCF_ModifyAt.ToString(), VCFParameters.VCF_ChangeType.ToString(), VCFParameters.VCF_ChangeDescription.ToString() };
                DefinitionBindingMapIterator definitionBindingMapIterator = doc.ParameterBindings.ForwardIterator();

                definitionBindingMapIterator.Reset();

                while (definitionBindingMapIterator.MoveNext())
                {
                    InternalDefinition paramDef = definitionBindingMapIterator.Key as InternalDefinition;
                    if (paramDef != null)
                    {
                        if (parameterNames.Contains(paramDef.Name))
                        {
                            paramDef.SetAllowVaryBetweenGroups(doc, true);
                            
                        }
                    }
                }
              
                //Reset to the previous shared parameters text file
                app.SharedParametersFilename = previousSharedParam;
                File.Delete(SPPath);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            

        }

        public static void WriteOnParam(string paramId, Element e, string value, bool overrideValues)
        {
            IList<Parameter> parameters = e.GetParameters(paramId);
            if (parameters.Count != 0)
            {
                Parameter p = parameters.FirstOrDefault();
                if (!p.IsReadOnly)
                {
                    if (overrideValues)
                    {
                        p.Set(value);
                    }
                    else
                    {
                        string paramValue = p.AsString();
                        if (String.IsNullOrEmpty(paramValue))
                        {
                            p.Set(value);
                        }
                    }
                }
            }
        }

        #region GetProjectParameterId
        public ElementId GetProjectParameterId(Document doc, string name)
        {
            ParameterElement pElem= new FilteredElementCollector(doc)
                        .OfClass(typeof(ParameterElement))
                        .Cast<ParameterElement>()
                        .Where(e => e.Name.Equals(name))
                        .FirstOrDefault();

            return pElem?.Id;
        }
        #endregion //Get project parameter Id



    }
}
