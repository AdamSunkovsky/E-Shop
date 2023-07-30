namespace E_Shop.Models
{
    public class WebsiteLog
    {
        //LogID	Info	Time	Type

        public int WebsiteLogID { get; set; }
        public string Info { get; set; }
        public DateTime Time { get; set; }
        public string Type {get; set; }
    }
}
