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
                       bindingMap.Insert(paramDef, instanceBinding, BuiltInParameterGroup.PG_IDENTITY_DATA);
                       
                    }
                    //the parameter is not added to the correct categories
                    else if (bindingMap.Contains(paramDef))
                    {
                        InstanceBinding currentBinding = bindingMap.get_Item(paramDef) as InstanceBinding;
                        currentBinding.Categories = myCategorySet;
                        bindingMap.ReInsert(paramDef, currentBinding, BuiltInParameterGroup.PG_IDENTITY_DATA);
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

        /// <summary>
        /// Helper function: return a string form of a given parameter.
        /// </summary>
        /// <param name="param">The param<see cref="Parameter"/></param>
        /// <returns>The <see cref="string"/></returns>
        public static string ParameterToString(Parameter param)
        {
            string val = "none";

            if (param == null)
            {
                return val;
            }

            // To get to the parameter value, we need to parse it depending on its storage type

            switch (param.StorageType)
            {
                case StorageType.Double:
                    double dVal = param.AsDouble();
                    val = dVal.ToString();
                    break;
                case StorageType.Integer:
                    int iVal = param.AsInteger();
                    val = iVal.ToString();
                    break;
                case StorageType.String:
                    string sVal = param.AsString();
                    val = sVal;
                    break;
                case StorageType.ElementId:
                    ElementId idVal = param.AsElementId();
                    val = idVal.IntegerValue.ToString();
                    break;

                case StorageType.None:
                    break;
            }
            return val;
        }

        /// <summary>
        /// The GetAllParametersElement
        /// </summary>
        /// <param name="e">The e<see cref="Element"/></param>
        /// <returns>The <see cref="Dictionary{string, string}"/></returns>
        public static Dictionary<string, string> GetAllParametersElement(Element e)
        {
            //Key is parameter name, value is paramter value
            Dictionary<string, string> dic = new Dictionary<string, string>();

            IList<Parameter> parameters = e.GetOrderedParameters();
            List<string> param_values = new List<string>(parameters.Count);
            foreach (Parameter p in parameters)
            {
                // AsValueString displays the value as the 
                // user sees it. In some cases, the underlying
                // database value returned by AsInteger, AsDouble,
                // etc., may be more relevant.

                param_values.Add(string.Format("{0}={1}",
                  p.Definition.Name, p.AsValueString()));
            }
            foreach (Parameter p in parameters)
            {
                var xxx = p.AsValueString();
                var xx = p.Definition.ParameterType;

                dic.Add(p.Id.ToString(), ParameterToString(p));
            }

            return dic;
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
