using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DatabaseConnection
{
    public class DatabaseContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=StudentK167932;Integrated Security=True");
        }

        public DbSet<Entry> DbEntries {get; set;}
        public DbSet<OfferDetails> DbOfferDetails { get; set; }
        public DbSet<PropertyPrice> DbPropertyPrices { get; set; }
        public DbSet<PropertyDetails> DbPropertyDetails { get; set; }
        public DbSet<PropertyAddress> DbPropertyAddresses { get; set; }
        public DbSet<PropertyFeatures> DbPropertyFeatures { get; set; }
        public DbSet<SellerContact> DbSellerContacts { get; set; }

    }

/*    public class DbEntry : Entry
    {
        public int EntryId { get; set; }
        public DbOfferDetail offerDetail;
    }

    public class DbOfferDetail : OfferDetails
    {
        public int DbdfferdetailId { get; set; }
        *//*public int EntryId { get; set; }
        public Entry Entry { get; set; }*//*
    }

    public class DbPropertyPrice : PropertyPrice
    {
        public int PropertyPriceId { get; set; }
        *//*public int EntryId { get; set; }
        public Entry Entry { get; set; }*//*
    }

    public class DbPropertyDetail : PropertyDetails
    {
        public int PropertyDetailId { get; set; }
        *//*public int EntryId { get; set; }
        public Entry Entry { get; set; }*//*
    }

    public class DbPropertyAddress : PropertyAddress
    {
        public int PropertyAddressId { get; set; }
        *//*public int EntryId { get; set; }
        public Entry Entry { get; set; }*//*
    }

    public class DbPropertyFeature : PropertyFeatures
    {
        public int PropertyFeatureId { get; set; }
        *//*public int EntryId { get; set; }
        public Entry Entry { get; set; }*//*
    }

    public class DbSellerContact : SellerContact
    {
        public int SellerContactId { get; set; }
        *//*public int OfferDetailId { get; set; }
        public DbOfferDetail OfferDetail { get; set; }*//*
    }*/


}


