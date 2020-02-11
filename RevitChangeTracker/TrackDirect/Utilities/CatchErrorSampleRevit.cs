//using Autodesk.Revit.ApplicationServices;
//using Autodesk.Revit.DB;
//using Autodesk.Revit.UI;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace TrackChanges.RevitUtility
//{
//    class CatchErrorSampleRevit
//    {
//        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//        {
//            UIDocument UIdoc = commandData.Application.ActiveUIDocument;
//            Application app = commandData.Application.Application;
//            Document doc = UIdoc.Document;

//            using (Transaction tx = new Transaction(doc))
//            {
//                try
//                {
//                    //Create a list of category
//                    CategorySet categories = CreateCategoryList(doc, app);

//                    //Retrive the coresponding list of elements
//                    IList<Element> ElementsList = GetElementList(doc, categories);

//                    //Load the interface
//                    PrepareModelInterface properties = new PrepareModelInterface(doc, ElementsList);

//                    if (properties.ShowDialog() == true)
//                    {
//                        tx.Start("Model TimeStamp");

//                        //if only on view, edit the element list
//                        if (properties.InView)
//                        {
//                            properties.ElementList = GetElementList(doc, categories, doc.ActiveView);
//                        }

//                        //Create Shared parameters if necessary
//                        AddSharedParameters(app, doc, categories);

//                        //Apply these values to every elements
//                        ApplyValuesOnElements(properties, doc);

//                        tx.Commit();

//                        // Return Success
//                        return Result.Succeeded;
//                    }
//                    else
//                    {
//                        return Autodesk.Revit.UI.Result.Cancelled;
//                    }
//                }

//                catch (Autodesk.Revit.Exceptions.OperationCanceledException exceptionCanceled)
//                {
//                    message = exceptionCanceled.Message;
//                    if (tx.HasStarted() == true)
//                    {
//                        tx.RollBack();
//                    }
//                    return Autodesk.Revit.UI.Result.Cancelled;
//                }
//                catch (ErrorMessageException errorEx)
//                {
//                    // checked exception need to show in error messagebox
//                    message = errorEx.Message;
//                    if (tx.HasStarted() == true)
//                    {
//                        tx.RollBack();
//                    }
//                    return Autodesk.Revit.UI.Result.Failed;
//                }
//                catch (Exception ex)
//                {
//                    // unchecked exception cause command failed
//                    message = ex.Message;
//                    //Trace.WriteLine(ex.ToString());
//                    if (tx.HasStarted() == true)
//                    {
//                        tx.RollBack();
//                    }
//                    return Autodesk.Revit.UI.Result.Failed;
//                }
//            }
//        }
//    }
//}
