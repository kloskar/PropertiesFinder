﻿namespace Models
{
    public class PropertyAddress
    {
        public int PropertyAddressID { get; set; }
        /// <summary>
        /// Nazwa miasta. Nie wymagane jeżeli posesja stoi we wsi
        /// </summary>
        public PolishCity City { get; set; }

        
        /// <summary>
        /// Dzielnica w której znajduje się posesja
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// Nazwa ulicy w której znajduje się posesja
        /// </summary>
        public string StreetName { get; set; }

        /// <summary>
        /// Numer budynku/mieszkania
        /// </summary>
        public string DetailedAddress { get; set; }
    }
}