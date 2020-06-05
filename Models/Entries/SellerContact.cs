﻿namespace Models
{
    public class SellerContact
    {
        public int SellerContactID { get; set; }
        public string Email { get; set; }

        public string Telephone { get; set; }

        /// <summary>
        /// Imię i nazwisko sprzedwcy
        /// </summary>
        public string Name { get; set; }
    }
}