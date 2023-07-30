namespace E_Shop.Models
{
    public class Image
    {
        public int ImageID { get; set; }
        public int ParentID { get; set; }
        public string ImgURL { get; set; }
        public int ImgPriority { get; set; }
    }
}
