using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace TrackDirect.UI
{
    public class CategoryDataStorageUtil
    {
        public static Guid schemaId = new Guid("{2BC84D29-BC28-452A-A90B-36598C2C493A}");
        public static Schema _schema = Schema.Lookup(schemaId);

        private static string s_CategoryName = "CategoryName";
        private static string s_CategoryId = "CategoryId";
        private static string s_Selected = "Selected";


        public static List<CategoryProperties> GetCategoryPropertiesDataStorage(Document doc)
        {
           List<CategoryProperties> catProps = new List<CategoryProperties>();
            try
            {
                if (null == _schema)
                {
                    _schema = CreateSchema();
                }
                if (null != _schema)
                {
                    IList<DataStorage> savedStorage = AutoTrackDataStorageUtil.GetDataStorageDocSettings(doc, _schema);
                    if (savedStorage.Count > 0)
                    {

                        foreach (DataStorage storage in savedStorage)
                        {
                            Entity entity = storage.GetEntity(_schema);
                            string catName = entity.Get<string>(_schema.GetField(s_CategoryName));
                            ElementId catId = entity.Get<ElementId>(_schema.GetField(s_CategoryId));
                            bool isSelected = entity.Get<bool>(_schema.GetField(s_Selected));
                            var catProperty = new CategoryProperties(Category.GetCategory(doc,catId));
                            catProperty.Selected = isSelected;
                            catProps.Add(catProperty);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to get room elevation properties from data storage.\n" + ex.Message, "Elevation Creator : GetCategoryPropertiesDataStorage", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return catProps; 
        }


        public static bool StoreCategoryProperties(Document doc, ObservableCollection< CategoryProperties> catProps)
        {
            bool saved = false;
            try
            {
                if (null == _schema)
                {
                    _schema = CreateSchema();
                }

                if (null != _schema)
                {

                    IList<DataStorage> savedStorage = AutoTrackDataStorageUtil.GetDataStorageDocSettings(doc, _schema);
                    if (savedStorage.Count > 0)
                    {
                        using (Transaction trans = new Transaction(doc))
                        {
                            trans.Start("Delete Data Storage");
                            try
                            {
                                foreach (DataStorage ds in savedStorage)
                                {
                                    doc.Delete(ds.Id);
                                }
                                trans.Commit();
                            }
                            catch (Exception ex)
                            {
                                trans.RollBack();
                                MessageBox.Show("Failed to delete data storage.\n" + ex.Message, "Elevation Creator : Update Data Storage", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }

                    using (Transaction trans = new Transaction(doc))
                    {
                        trans.Start("Add New Storage");
                        try
                        {
                            foreach (CategoryProperties cProp in catProps)
                            {
                                DataStorage storage = DataStorage.Create(doc);
                                Entity entity = new Entity(schemaId);
                                entity.Set<string>(s_CategoryName, cProp.CategoryName);
                                entity.Set<ElementId>(s_CategoryId, cProp.CategoryId);
                                entity.Set<bool>(s_Selected, cProp.Selected);
                                storage.SetEntity(entity);
                            }

                            trans.Commit();
                            saved = true;
                        }
                        catch (Exception ex)
                        {
                            trans.RollBack();
                            MessageBox.Show("Failed to add data storage.\n" + ex.Message, "AutoTrack : Update Data Storage", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save category properties in data storage.\n" + ex.Message, "AutoTrack : StoreCategoryProperties", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return saved;
        }

        public static bool StoreCategoryProperties(Document doc, CategoryProperties cProp)
        {
            bool saved = false;
            try
            {
                if (null == _schema)
                {
                    _schema = CreateSchema();
                }

                if (null != _schema)
                {
                    IList<DataStorage> savedStorage = AutoTrackDataStorageUtil.GetDataStorageDocSettings(doc, _schema);
                    if (savedStorage.Count > 0)
                    {
                        using (Transaction trans = new Transaction(doc))
                        {
                            trans.Start("Delete Data Storage");
                            try
                            {
                                DataStorage storageToDelete = null;
                                foreach (DataStorage ds in savedStorage)
                                {
                                    Entity entity = ds.GetEntity(_schema);
                                    ElementId catId = entity.Get<ElementId>(_schema.GetField(s_CategoryId));
                                    if (cProp.CategoryId.IntegerValue == catId.IntegerValue)
                                    {
                                        storageToDelete = ds;
                                        break;
                                    }
                                }
                                if (null != storageToDelete)
                                {
                                    doc.Delete(storageToDelete.Id);
                                }

                                trans.Commit();
                            }
                            catch (Exception ex)
                            {
                                trans.RollBack();
                                LogMessageBuilder.AddLogMessage(cProp.CategoryName + " : failed to delete data storage.");
                                LogMessageBuilder.AddLogMessage(ex.Message);
                                //MessageBox.Show("Failed to delete data storage.\n" + ex.Message, "Elevation Creator : Update Data Storage", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }

                    using (Transaction trans = new Transaction(doc))
                    {
                        trans.Start("Add New Storage");
                        try
                        {
                            DataStorage storage = DataStorage.Create(doc);
                            Entity entity = new Entity(schemaId);
                            entity.Set<string>(s_CategoryName, cProp.CategoryName);
                            entity.Set<ElementId>(s_CategoryId, cProp.CategoryId);
                            entity.Set<bool>(s_Selected, cProp.Selected);

                            storage.SetEntity(entity);

                            trans.Commit();
                            saved = true;
                        }
                        catch (Exception ex)
                        {
                            trans.RollBack();
                            LogMessageBuilder.AddLogMessage(cProp.CategoryName + " : failed to add data storage.");
                            LogMessageBuilder.AddLogMessage(ex.Message);
                            //MessageBox.Show("Failed to add data storage.\n" + ex.Message, "Elevation Creator : Update Data Stroage", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessageBuilder.AddLogMessage(cProp.CategoryName + " : failed to save category properties in data storage.");
                LogMessageBuilder.AddLogMessage(ex.Message);
                //MessageBox.Show("Failed to save room elevation properties in data storage.\n" + ex.Message, "Elevation Creator : StoreRoomeElevationProperties", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return saved;
        }

        private static Schema CreateSchema()
        {
            Schema schema = null;
            try
            {
                SchemaBuilder schemaBuilder = new SchemaBuilder(schemaId);
                schemaBuilder.SetSchemaName("CategoriesTrack");
                schemaBuilder.AddSimpleField(s_CategoryName, typeof(string));
                schemaBuilder.AddSimpleField(s_CategoryId, typeof(ElementId));
                schemaBuilder.AddSimpleField(s_Selected, typeof(bool));

                schema = schemaBuilder.Finish();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create schema.\n" + ex.Message, "Create Schema", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return schema;
        }
        public static class LogMessageBuilder
        {
            private static StringBuilder logMessages = new StringBuilder();

            public static void AddLogMessage(string str)
            {
                logMessages.AppendLine(str);
            }

            public static string GetLogMessages()
            {
                return logMessages.ToString();
            }

            public static void RefreshMessages()
            {
                logMessages = new StringBuilder();
            }
        }
    }
}
