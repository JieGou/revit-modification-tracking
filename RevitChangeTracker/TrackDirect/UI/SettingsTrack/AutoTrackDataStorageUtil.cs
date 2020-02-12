﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace TrackDirect.UI
{
    public static class AutoTrackDataStorageUtil
    {
        public static Guid schemaId = new Guid("{12D470A3-9AC0-4E17-95FE-4938B180EFAB}");
        public static Schema _schema = Schema.Lookup(schemaId);
        private static string s_CanAutoRun = "CanAutoRun";
        private static string s_IsAutoTrackEventSave = "IsAutoTrackEventSave";
        private static string s_IsAutoTrackEventDocumentOpen = "IsAutoTrackEventDocumentOpen";
        private static string s_IsAutoTrackEventSynchronize = "AutoTrackEventSynchronize";
        private static string s_IsAutoTrackEventViewActive = "IsAutoTrackEventViewActive";
        private static string s_IsAutoTrackByTime= "IsAutoTrackByTime";
        private static string s_TimeOut = "TimeOut";




        #region DataStorage for Checkbox
        public static AutoTrackSettings GetAutoTrackCreatorSettings(Document doc)
        {
            AutoTrackSettings settings = new AutoTrackSettings();
            try
            {
                if (null == _schema)
                {
                    _schema = CreateSchema();
                }
                if (null != _schema)
                {
                    IList<DataStorage> savedStorage = GetDataStorageDocSettings(doc, _schema);
                    if (savedStorage.Count > 0)
                    {
                        DataStorage storage = savedStorage.First();
                        Entity entity = storage.GetEntity(_schema);
                        settings.CanAutoRun = entity.Get<bool>(_schema.GetField(s_CanAutoRun));
                        settings.IsAutoTrackEventSave = entity.Get<bool>(_schema.GetField(s_IsAutoTrackEventSave));
                        settings.IsAutoTrackEventDocumentOpen = entity.Get<bool>(_schema.GetField(s_IsAutoTrackEventDocumentOpen));
                        settings.IsAutoTrackEventSynchronize = entity.Get<bool>(_schema.GetField(s_IsAutoTrackEventSynchronize));
                        settings.IsAutoTrackEventViewActive = entity.Get<bool>(_schema.GetField(s_IsAutoTrackEventViewActive));
                        settings.IsAutoTrackByTime = entity.Get<bool>(_schema.GetField(s_IsAutoTrackByTime));
                        settings.TimeOut = entity.Get<int>(_schema.GetField(s_TimeOut));

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to get AutoTrack settings from data storage.\n" + ex.Message, "AutoTrack : GetAutoTrackSettings", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return settings;
        }

        public static bool StoreAutoTrackCreatorSettings(Document doc, AutoTrackSettings settings)
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
                    IList<DataStorage> savedStorage = GetDataStorageDocSettings(doc, _schema);
                    if (savedStorage.Count > 0)
                    {
                        using (Transaction trans = new Transaction(doc))
                        {
                            trans.Start("Delete Data Storage");
                            try
                            {
                                DataStorage storageToDelete = savedStorage.First();
                                doc.Delete(storageToDelete.Id);
                                trans.Commit();
                            }
                            catch (Exception ex)
                            {
                                trans.RollBack();
                                MessageBox.Show("Failed to delete data storage of AutoTrack settings.\n" + ex.Message, "AutoTrack : StoreAutoTrackSettings", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                            entity.Set<bool>(s_CanAutoRun, settings.CanAutoRun);
                            entity.Set<bool>(s_IsAutoTrackEventSave, settings.IsAutoTrackEventSave);
                            entity.Set<bool>(s_IsAutoTrackEventDocumentOpen, settings.IsAutoTrackEventDocumentOpen);
                            entity.Set<bool>(s_IsAutoTrackEventSynchronize, settings.IsAutoTrackEventSynchronize);
                            entity.Set<bool>(s_IsAutoTrackEventViewActive, settings.IsAutoTrackEventViewActive);
                            entity.Set<bool>(s_IsAutoTrackByTime, settings.IsAutoTrackByTime);
                            entity.Set<int>(s_TimeOut, settings.TimeOut);

                            storage.SetEntity(entity);
                            trans.Commit();
                            saved = true;
                        }
                        catch (Exception ex)
                        {
                            trans.RollBack();
                            MessageBox.Show("Failed to add data storage of AutoTrack settings.\n" + ex.Message, "AutoTrack : Update Data Storage", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save AutoTrack settings in data storage.\n" + ex.Message, "AutoTrack : StoreAutoTrackSettings", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return saved;
        }


        private static Schema CreateSchema()
        {
            Schema schema = null;
            try
            {
                SchemaBuilder schemaBuilder = new SchemaBuilder(schemaId);
                schemaBuilder.SetSchemaName("AutoTrackCreator2");
                schemaBuilder.AddSimpleField(s_CanAutoRun, typeof(bool));
                schemaBuilder.AddSimpleField(s_IsAutoTrackEventSave, typeof(bool));
                schemaBuilder.AddSimpleField(s_IsAutoTrackEventDocumentOpen, typeof(bool));
                schemaBuilder.AddSimpleField(s_IsAutoTrackEventSynchronize, typeof(bool));
                schemaBuilder.AddSimpleField(s_IsAutoTrackEventViewActive, typeof(bool));
                schemaBuilder.AddSimpleField(s_IsAutoTrackByTime, typeof(bool));
                schemaBuilder.AddSimpleField(s_TimeOut, typeof(int));


                schema = schemaBuilder.Finish();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create schema.\n" + ex.Message, "Create Schema", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return schema;
        }



        public class AutoTrackSettings
        {
            private  bool canAutoRun = true;
            private  bool isAutoTrackEventSave = false;
            private  bool isAutoTrackEventDocumentOpen = false;
            private  bool isAutoTrackEventSynchronize = false;
            private  bool isAutoTrackEventViewActive = false;
            private  bool isAutoTrackByTime = false;
            private  int timeOut = 10; //10 minute

            public bool CanAutoRun { get { return canAutoRun; } set { canAutoRun = value; } }
            public bool IsAutoTrackEventSave { get { return isAutoTrackEventSave; } set { isAutoTrackEventSave = value; } }
            public bool IsAutoTrackEventDocumentOpen { get { return isAutoTrackEventDocumentOpen; } set { isAutoTrackEventDocumentOpen = value; } }
            public bool IsAutoTrackEventSynchronize { get { return isAutoTrackEventSynchronize; } set { isAutoTrackEventSynchronize = value; } }
            public bool IsAutoTrackEventViewActive { get { return isAutoTrackEventViewActive; } set { isAutoTrackEventViewActive = value; } }
            public bool IsAutoTrackByTime{ get { return isAutoTrackByTime; } set { isAutoTrackByTime = value; } }
            public int TimeOut { get { return timeOut; } set { timeOut = value; } }


            public AutoTrackSettings()
            {
            }

            public AutoTrackSettings(AutoTrackSettings settings)
            {
                isAutoTrackEventSave = settings.IsAutoTrackEventSave;
                isAutoTrackEventDocumentOpen = settings.IsAutoTrackEventDocumentOpen;
                isAutoTrackEventSynchronize = settings.IsAutoTrackEventSynchronize;
                isAutoTrackEventViewActive = settings.IsAutoTrackEventViewActive;
                isAutoTrackByTime = settings.IsAutoTrackByTime;
                timeOut = settings.TimeOut;
                canAutoRun = settings.CanAutoRun;

            }
        }

        #endregion


        public static IList<DataStorage> GetDataStorageDocSettings(Document document, Schema schema)
        {
            FilteredElementCollector collector = new FilteredElementCollector(document);
            collector.OfClass(typeof(DataStorage));
            Func<DataStorage, bool> hasTargetData = ds => (ds.GetEntity(schema) != null && ds.GetEntity(schema).IsValid());

            return collector.Cast<DataStorage>().Where<DataStorage>(hasTargetData).ToList<DataStorage>();
        }
    }

    
}