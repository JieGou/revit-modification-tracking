﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace TrackChanges
{
    public class ParameterUtils
    {
        public static void AddSharedParameters(Autodesk.Revit.ApplicationServices.Application app, Document doc, CategorySet myCategorySet)
        {
            //Save the previous shared param file path
            string previousSharedParam = app.SharedParametersFilename;

            //Extract shared param to a txt file
            string tempPath = System.IO.Path.GetTempPath();
            string SPPath = Path.Combine(tempPath, "FileProperties.txt");

            if (!File.Exists(SPPath))
            {
                //extract the familly
                List<string> files = new List<string>();
                files.Add("FileProperties.txt");
                Tools.ExtractEmbeddedResource(tempPath, "TimeStamp.Resources", files);
            }

            //set the shared param file
            app.SharedParametersFilename = SPPath;

            //Retrive shared parameters
            DefinitionFile myDefinitionFile = app.OpenSharedParameterFile();

            DefinitionGroup definitionGroup = myDefinitionFile.Groups.get_Item("FileProperties");

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
            string[] parameterNames = { "BIM42_Date", "BIM42_Version", "BIM42_File", "BIM42_Discipline" };

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
