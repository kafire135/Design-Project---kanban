using IntroSE.Kanban.Backend.BusinessLayer;
using System.Data.SQLite;


namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class BackendInitializer
    {
        public BackendInitializer() { }

        ///<summary>This method loads all persisted data.</summary>
        public string LoadData()
        {
            try
            {
                BusinessLayerFactory.GetInstance().DataCenterManagement.LoadData();
                return JsonEncoder.ConvertToJson(new Response<string>(true, ""));
            }
            catch (SQLiteException e)
            {
                return JsonEncoder.ConvertToJson(new Response<string>(false, e.Message));
            }
        }
    }
}
