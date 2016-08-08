using NineWorldsDeep.Core;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Collections.ObjectModel;
using NineWorldsDeep.Sqlite;

namespace NineWorldsDeep.Synergy
{
    public class SynergySqliteDbAdapter
    {
        //methods tightly coupled to db tables are moving to core adapter
        Db.SqliteDbAdapter db = new Db.SqliteDbAdapter();

        public IEnumerable<SynergyList> GetActiveLists()
        {
            return db.GetLists(true);
        }

        public void Save(IEnumerable<SynergyList> _lists)
        {
            try
            {
                using (var conn =
                    new SQLiteConnection(@"Data Source=" +
                        Configuration.GetSqliteDbPath("nwd")))
                {
                    conn.Open();

                    using (var cmd = new SQLiteCommand(conn))
                    {
                        using (var transaction = conn.BeginTransaction())
                        {

                            foreach(var lst in _lists)
                            {
                                int listId = db.EnsureIdForListName(lst.Name, cmd);

                                foreach(var si in lst.Items)
                                {
                                    int itemId = db.EnsureIdForItemValue(si.Item, cmd);

                                    db.UpsertFragment(listId, itemId, si, cmd);
                                }
                            }

                            transaction.Commit();
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                //lets just throw it for now, but put something here eventually
                throw ex;
            }
        }
        
    }
    
    /////////////////////////////////////connection/transaction template
    //    try
    //    {
    //        using (var conn =
    //            new SQLiteConnection(@"Data Source=" +
    //                Configuration.GetSqliteDbPath("nwd")))
    //        {
    //            conn.Open();

    //            using (var cmd = new SQLiteCommand(conn))
    //            {
    //                using (var transaction = conn.BeginTransaction())
    //                {

    //////////////////////////////////////////CODE HERE//////////////////////////////////////

    //                    transaction.Commit();
    //                }
    //            }

    //            conn.Close();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        //lets just throw it for now, but put something here eventually
    //        throw ex;
    //    }
}