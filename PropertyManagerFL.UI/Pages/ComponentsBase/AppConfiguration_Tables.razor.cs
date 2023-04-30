using PropertyManagerFL.Application.ViewModels.LookupTables;

namespace PropertyManagerFL.UI.Pages.ComponentsBase
{
    /// <summary>
    /// Configuration tables
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class AppConfiguration_Tables<T> where T : new()
    {

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<T> GetDataList(IEnumerable<LookupTableVM> source)
        {
            List<T> lstRecs = new List<T>();
            foreach (var item in source)
            {
                T rec = new T();
                rec.GetType().GetProperty("Codigo").SetValue(rec, item.Id);
                rec.GetType().GetProperty("Descricao").SetValue(rec, item.Descricao);
                lstRecs.Add(rec);
            }
            return lstRecs;
        }
    }

}
