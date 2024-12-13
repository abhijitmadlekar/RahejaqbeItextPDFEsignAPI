namespace DocEsignAPI.Models.RequestModels
{
    public class EsignRM
    {

        public int Page_NO { get; set; }
        public string Reason { get; set; }
        public string Location { get; set; }
        public int Cordinates_X { get; set; }
        public int Cordinates_Y { get; set; }
        public int Cordinates_Width { get; set; }
        public int Cordinates_Height { get; set; }
        public string EsignDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");
    }

}
