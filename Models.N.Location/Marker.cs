﻿namespace BGBA.Models.N.Location
{
    public class Marker
    {
        public string Size { get; set; }
        public string Color { get; set; }
        public string Location { get; set; }

        public override string ToString()
        {
            return $"size:{Size}|color:{Color}|{Location}"; 
        }
    }
}
