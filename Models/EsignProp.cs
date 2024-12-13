namespace DocEsignAPI.Models
{
    public class EsignProp
    {
        public Cordinates? Cordinates { get; set; }
        public string Reason { get; set; } = "";
        public string Location { get; set; } = "Mumbai";
        public int Page_No { get; set; } = 1;
        public string EsignDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy");
    }

    public class Cordinates
    {

        public Cordinates(int x,int y,int width,int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        
        public int X { get; set; } = 50;
        public int Y { get; set; } = 50;
        public int Width { get; set; } = 100;
        public int Height { get; set; } = 40;
    }
}
